using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;
using Xunit.Extensions.Ordering;
using static System.Data.CommandType;


namespace StringAsSql.Tests {
    public abstract class BaseTests {
        private BaseFixture fixture;
        public BaseTests(
            BaseFixture fixture,
            string createTableSql,
            string insertSql,
            string personsSql,
            string countSql,
            string distinctSql,
            string datatableSql
        ) {
            this.fixture = fixture;
            this.createTableSql = createTableSql;
            this.insertSql = insertSql;
            this.personsSql = personsSql;
            this.countSql = countSql;
            this.distinctSql = distinctSql;
            this.datatableSql = datatableSql;
        }

        protected void Execute(string sql, object parameters = null) {
            if (fixture.Connection != null) {
                sql.AsSql(parameters).Execute(fixture.Connection);
            } else {
                sql.AsSql(parameters).Execute();
            }
        }

        private string createTableSql;
        [Fact, Order(0)]
        public void CreateTable() => Execute(createTableSql);

        protected string insertSql;

        private string personsSql;
        [Fact, Order(2)]
        public void GetList() {
            var expected = new List<Person> {
                new Person {LastName="Avinu", FirstName="Yaakov"},
                new Person {LastName = "HaIvri", FirstName="Avraham"},
                new Person {LastName = "Avinu", FirstName="Yitzchak"}
            };
            var actual = fixture.Connection != null ?
                "Persons".AsSql(TableDirect).ToList<Person>(fixture.Connection) :
                "Persons".AsSql(TableDirect).ToList<Person>();
            foreach (var p in expected) {
                Assert.Contains(actual, p1 => p1.LastName == p.LastName && p1.FirstName == p.FirstName);
            }
            Assert.Equal(expected.Count, actual.Count);
        }

        private string countSql;
        [Fact, Order(3)]
        public void GetScalar() {
            var actual = fixture.Connection != null ?
                countSql.AsSql().ToScalar<int>(fixture.Connection) :
                countSql.AsSql().ToScalar<int>();
            Assert.Equal(3, actual);
        }

        private string distinctSql;
        [Fact, Order(3)]
        public void GetDistinctList() {
            var actual = fixture.Connection != null ?
                distinctSql.AsSql().ToList<string>(fixture.Connection).ToHashSet() :
                distinctSql.AsSql().ToList<string>().ToHashSet();
            var expected = new HashSet<string> { "HaIvri", "Avinu" };
            Assert.Equal(expected, actual);
        }

        private string datatableSql;
        [Fact, Order(3)]
        public void GetDataTable() {
            var dt = fixture.Connection != null ?
                datatableSql.AsSql().ToDataTable(fixture.Connection) :
                datatableSql.AsSql().ToDataTable();
            Assert.Collection(
                dt.Columns.Cast<DataColumn>().ToList(),
                c => {
                    Assert.Equal("ID", c.ColumnName);
                    Assert.Equal(typeof(int), c.DataType);
                },
                c => {
                    Assert.Equal("LastName", c.ColumnName);
                    Assert.Equal(typeof(string), c.DataType);
                },
                c => {
                    Assert.Equal("FirstName", c.ColumnName);
                    Assert.Equal(typeof(string), c.DataType);
                }
            );
        }


    }
}
