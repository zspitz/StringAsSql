using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace StringAsSql.Util {
    public static class DbCommandExtensions {
        private static Dictionary<string, Func<string, string>> parameterNameBuilders = new Dictionary<string, Func<string, string>>() {
            {"System.Data.SqlClient.SqlCommand", name => "@" + name },
            {"System.Data.SqlServerCe.SqlCeCommand", name => "@" + name}
        };

        public static Func<DbCommand, string, string> ParameterNameBuilderExt;

        private static string getParameterName(DbCommand command, string propertyName) {
            if (parameterNameBuilders.TryGetValue(command.GetType().ToString(), out var builder)) {
                return builder(propertyName);
            }
            if (ParameterNameBuilderExt != null) {
                return ParameterNameBuilderExt(command, propertyName);
            }
            return propertyName;
        }

        private static bool tryGetKeyValue(object o, out (string key, object val) keyValue) {
            keyValue = ("", null);
            var type = o.GetType();
            if (!type.IsGenericType) { return false; }
            var args = type.GetGenericArguments();
            if (args.First() != typeof(string)) { return false; }

            var definition = type.GetGenericTypeDefinition();
            if (definition == typeof(KeyValuePair<,>)) {
                var memberNamesValues = o.MemberNamesValues().ToDictionary();
                keyValue = (memberNamesValues["Key"] as string, memberNamesValues["Value"]);
                return true;
            }

            if (definition.IsTupleType()) {
                var memberNamesValues = o.MemberNamesValues().ToDictionary();
                keyValue = (memberNamesValues["Item1"] as string, memberNamesValues["Item2"]);
                return true;
            }

            return false;
        }

        private static void AddKeyValueParameter(DbCommand cmd, string key, object value) {
            var prm = cmd.CreateParameter();
            prm.ParameterName = getParameterName(cmd, key);
            prm.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(prm);
        }

        public static void AddParameters(this DbCommand cmd, params object[] @params) {
            if (@params == null) { return; }
            foreach (var o in @params) {
                if (o is IEnumerable enumerable && !(o is string)) {
                    cmd.AddParameters(enumerable.Cast<object>().ToArray());
                    continue;
                }

                // DbParameter
                if (o is DbParameter parameter) {
                    cmd.Parameters.Add(parameter);
                    continue;
                }

                var type = o?.GetType();
                if (o == null || type.IsPrimitive || type.In(typeof(string), typeof(DateTime), typeof(TimeSpan))) {
                    // no name; positional parameter
                    AddKeyValueParameter(cmd, $"Parameter{cmd.Parameters.Count}", o);
                    continue;
                }

                if (tryGetKeyValue(o, out var keyValue)) {
                    // supports key-value -- keyvaluepair, tuple or value tuple
                    AddKeyValueParameter(cmd, keyValue.key, keyValue.val);
                    continue;
                }

                // object - members + values
                foreach (var (key, val) in o.MemberNamesValues()) {
                    AddKeyValueParameter(cmd, key, val);
                }
            }
        }
    }
}
