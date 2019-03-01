using System.Collections.Generic;
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
            string datatableSql
        ) : base(fixture, createTableSql, insertSql, personsSql, countSql, distinctSql, datatableSql) { }

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
                {"LastName", "Yitzchak" }
            }
        );
    }
}
