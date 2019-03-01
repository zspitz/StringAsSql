using System.Data.Common;
using static System.Data.ConnectionState;
using static System.Data.CommandType;
using System.Data;

namespace StringAsSql.Util {
    public static class DbConnectionExtensions {
        public static bool TryOpen(this DbConnection conn) {
            var ret = false;
            if (conn.State == Closed) {
                conn.Open();
                ret = true;
            }
            return ret;
        }

        public static DbCommand CreateCommand(this DbConnection conn, string text, object @params = null, CommandType commandType = Text) {
            var cmd = conn.CreateCommand();
            cmd.CommandText = text;
            cmd.CommandType = commandType;
            cmd.AddParameters(@params);
            return cmd;
        }

    }
}
