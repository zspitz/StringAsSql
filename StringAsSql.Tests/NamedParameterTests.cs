using System;
using System.Collections.Generic;
using System.Data.Common;
using Xunit;
using Xunit.Extensions.Ordering;

namespace StringAsSql.Tests {
    public abstract class NamedParameterTests : BaseTests {
        public NamedParameterTests(
            BaseFixture fixture,
            string createTableSql,
            string insertSql,
            string personsSql,
            string countSql,
            string distinctSql,
            string datatableSql,
            DbParameter[] parameters
        ) : base(fixture, createTableSql, insertSql, personsSql, countSql, distinctSql, datatableSql, parameters) { }

        [Fact, Order(1)]
        protected void InsertWithNamedType() => Execute(
            insertSql,
            new Person { LastName = "HaIvri", FirstName = "Avraham" }
        );

        [Fact, Order(1)]
        protected void InsertWithAnonymousType() => Execute(
            insertSql,
            new { FirstName = "Yitzchak", LastName = "Avinu" }
        );

        [Fact, Order(1)]
        protected void InsertWithDictionary() => Execute(
            insertSql,
            new Dictionary<string, string>() {
                {"FirstName", "Yaakov" },
                {"LastName", "Avinu" }
            }
        );

        [Fact, Order(1)]
        public void InsertDbParameter() {
            parameters[0].ParameterName = "@FirstName";
            parameters[0].Value = "Hillel";
            parameters[1].ParameterName = "@LastName";
            parameters[1].Value = DBNull.Value;
            Execute(
                insertSql,
                parameters
            );
        }

        [Fact, Order(1)]
        public void InsertWithTupleParameter() => Execute(
            insertSql,
            Tuple.Create("FirstName", "Dovid"),
            Tuple.Create("LastName", "HaMelech")
        );

    }
}
