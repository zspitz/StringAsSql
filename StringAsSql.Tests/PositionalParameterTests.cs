using System.Collections;
using System.Collections.Generic;
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
            string datatableSql
        ) : base(fixture, createTableSql, insertSql, personsSql, countSql, distinctSql, datatableSql) { }

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
    }
}
