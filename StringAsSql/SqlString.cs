using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using static System.Data.CommandType;
using StringAsSql.Util;

namespace StringAsSql {
    public class SqlString {
        public static Func<DbConnection> ConnectionFactory;

        readonly string sql;
        readonly object @params;
        readonly Dictionary<string, object> paramsDictionary;
        readonly CommandType commandType;

        public SqlString(CommandType commandType, string sql, object @params = null) {
            this.commandType = commandType;
            this.sql = sql;
            this.@params = @params;
        }

        public DataTable ToDataTable(DbConnection conn) {
            var ret = new DataTable();
            conn.TryOpen();
            using (var cmd = conn.CreateCommand(sql, @params, commandType))
            using (var rdr = cmd.ExecuteReader()) {
                ret.Load(rdr);
            }
            return ret;
        }
        public DataTable ToDataTable() {
            using (var conn = ConnectionFactory()) {
                return ToDataTable(conn);
            }
        }

        public List<T> ToList<T>(DbConnection conn, Func<DbDataRecord, T> selector = null) {
            conn.TryOpen();
            var ret = new List<T>();
            if (selector == null) {
                List<PropertyInfo> props = null;
                selector = row => row.To<T>(ref props);
            }
            using (var cmd = conn.CreateCommand(sql, @params, commandType))
            using (var rdr = cmd.ExecuteReader()) {
                ret = rdr.Cast<DbDataRecord>().Select(selector).ToList();
            }
            return ret;
        }
        public List<T> ToList<T>(Func<DbDataRecord, T> selector = null) {
            using (var conn = ConnectionFactory()) {
                return ToList(conn, selector);
            }
        }

        public T ToScalar<T>(DbConnection conn) {
            T ret = default(T);
            conn.TryOpen();
            using (var cmd = conn.CreateCommand(sql, @params, commandType)) {
                var result = cmd.ExecuteScalar();
                if (result != DBNull.Value) { ret = (T)result; }
            }
            return ret;
        }
        public T ToScalar<T>() {
            using (var conn = ConnectionFactory()) {
                return ToScalar<T>(conn);
            }
        }

        public void WithRows(DbConnection conn, Action<DbDataRecord> action) {
            conn.TryOpen();
            using (var cmd = conn.CreateCommand(sql, @params, commandType))
            using (var rdr = cmd.ExecuteReader()) {
                rdr.Cast<DbDataRecord>().ForEach(action);
            }
        }
        public void WithRows(DbConnection conn, Action<DbDataRecord, int> action) {
            conn.TryOpen();
            using (var cmd = conn.CreateCommand(sql, @params, commandType))
            using (var rdr = cmd.ExecuteReader()) {
                rdr.Cast<DbDataRecord>().ForEach(action);
            }
        }
        public void WithRows(Action<DbDataRecord> action) {
            using (var conn = ConnectionFactory()) {
                WithRows(conn, action);
            }
        }

        public int Execute(DbConnection conn) {
            int ret;
            conn.TryOpen();
            using (var cmd = conn.CreateCommand(sql, @params)) {
                ret = cmd.ExecuteNonQuery();
            }
            return ret;
        }
        public int Execute() {
            using (var conn = ConnectionFactory()) {
                return Execute(conn);
            }
        }
    }

    public static class StringAsSqlExtensions {
        public static SqlString AsSql(this string sql, object @params) =>
            new SqlString(Text, sql, @params);
        public static SqlString AsSql(this string sql, CommandType commandType = Text, object @params = null) =>
            new SqlString(commandType, sql, @params);
    }
}
