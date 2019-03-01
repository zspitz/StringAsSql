using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StringAsSql.Util {
    public static class ObjectExtensions {
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

        public static List<(string key, object value)> MemberNamesValues(this object o) {
            var ret = new List<(string, object)>();
            var t = o.GetType();
            t.GetProperties().Where(x => x.GetIndexParameters().None()).Select(p => (p.Name, p.GetValue(o))).AddRangeTo(ret);
            t.GetFields().Select(f => (f.Name, f.GetValue(o))).AddRangeTo(ret);
            return ret;
        }

        public static List<(string key, object value)> ParseObject(this object obj) {
            if (obj is IEnumerable enumerable && !(obj is string)) {
                return enumerable.Cast<object>().Select((x, index) =>
                    tryGetKeyValue(x, out var kv) ? kv :
                    ($"Parameter{index}", x)
                ).ToList();
            }

            return obj.MemberNamesValues();
        }
    }
}
