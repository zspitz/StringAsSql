using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Xunit;
using Xunit.Extensions.Ordering;

namespace StringAsSql.Tests {
    public abstract class PositionalParameterTests : BaseTests {
        public PositionalParameterTests(
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
        public void InsertWithParameterList() => Execute(
            insertSql,
            new List<string> { "HaIvri", "Avraham" }
        );

        [Fact, Order(1)]
        public void InsertWithParameterList2() => Execute(
            insertSql,
            new ArrayList { "Avinu", "Yitzchak" }
        );

        [Fact, Order(1)]
        public void InsertWithParameterArrayOfValueTuple() => Execute(
            insertSql,
            new[] {
                ("LastName", "Avinu"),
                ("FirstName", "Yaakov")
            }
        );

        [Fact, Order(1)]
        public void InsertDbParameter() {
            parameters[0].ParameterName = "Parameter";
            parameters[0].Value = "HaMelech";
            parameters[1].ParameterName = "Parameter";
            parameters[1].Value = "Dovid";
            Execute(
                insertSql,
                parameters[0],
                parameters[1]
            );
        }

        [Fact, Order(1)]
        public void InsertWithNull() => Execute(
            insertSql,
            null,
            "Hillel"
        );
    }
}
