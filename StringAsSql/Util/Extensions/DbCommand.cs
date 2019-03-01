using System;
using System.Collections.Generic;
using System.Data.Common;

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

        public static void AddParameters(this DbCommand cmd, object @params) {
            if (@params == null) { return; }
            @params.ParseObject().ForEachT((key, value) => {
                var param = cmd.CreateParameter();
                param.ParameterName = getParameterName(cmd, key);
                param.Value = value ?? DBNull.Value;
                cmd.Parameters.Add(param);
            });
        }
    }
}
