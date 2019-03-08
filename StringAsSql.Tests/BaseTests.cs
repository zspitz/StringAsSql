using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
            string datatableSql,
            DbParameter[] parameters
        ) {
            this.fixture = fixture;
            this.createTableSql = createTableSql;
            this.insertSql = insertSql;
            this.personsSql = personsSql;
            this.countSql = countSql;
            this.distinctSql = distinctSql;
            this.datatableSql = datatableSql;
            this.parameters = parameters;
        }

        protected void Execute(string sql, params object[] parameters) {
            if (fixture.Connection != null) {
                sql.AsSql(parameters).Execute(fixture.Connection);
            } else {
                sql.AsSql(parameters).Execute();
            }
        }

        private readonly string createTableSql;
        [Fact, Order(0)]
        public void CreateTable() => Execute(createTableSql);

        protected string insertSql;
        protected DbParameter[] parameters;

        private string personsSql;
        readonly List<Person> expected = new List<Person> {
            new Person {LastName="Avinu", FirstName="Yaakov"},
            new Person {LastName = "HaIvri", FirstName="Avraham"},
            new Person {LastName = "Avinu", FirstName="Yitzchak"},
            new Person {LastName = "HaMelech", FirstName="Dovid"},
            new Person {FirstName = "Hillel"}
        };
        [Fact, Order(2)]
        public void GetList() {
            var actual = fixture.Connection != null ?
                personsSql.AsSql(TableDirect).ToList<Person>(fixture.Connection) :
                personsSql.AsSql(TableDirect).ToList<Person>();
            foreach (var p in expected) {
                Assert.Contains(actual, p1 => p1.LastName == p.LastName && p1.FirstName == p.FirstName);
            }
            Assert.Equal(expected.Count, actual.Count);
        }

        [Fact, Order(2)]
        public void GetDynamicList() {
            var actual = fixture.Connection != null ?
                personsSql.AsSql(TableDirect).ToList<dynamic>(fixture.Connection) :
                personsSql.AsSql(TableDirect).ToList<dynamic>();
            foreach (var p in expected) {
                Assert.Contains(actual, p1 => p1.LastName == p.LastName && p1.FirstName == p.FirstName);
            }
            Assert.Equal(expected.Count, actual.Count);
        }

        [Fact, Order(2)]
        public void GetObjectList() {
            var actual = fixture.Connection != null ?
                personsSql.AsSql(TableDirect).ToList<object>(fixture.Connection) :
                personsSql.AsSql(TableDirect).ToList<object>();
            foreach (var p in expected) {
                Assert.Contains(actual, (dynamic p1) => p1.LastName == p.LastName && p1.FirstName == p.FirstName);
            }
            Assert.Equal(expected.Count, actual.Count);
        }

        [Fact, Order(2)]
        public void GetAnonymousTypeList() {
            var typer = new {
                LastName = "",
                FirstName = ""
            };
            var actual = fixture.Connection != null ?
                personsSql.AsSql(TableDirect).ToList(fixture.Connection, typer) :
                personsSql.AsSql(TableDirect).ToList(typer);
            foreach (var p in expected) {
                Assert.Contains(actual, p1 => p1.LastName == p.LastName && p1.FirstName == p.FirstName);
            }
            Assert.Equal(expected.Count, actual.Count);
        }

        private string countSql;
        [Fact, Order(2)]
        public void GetScalar() {
            var actual = fixture.Connection != null ?
                countSql.AsSql().ToScalar<int>(fixture.Connection) :
                countSql.AsSql().ToScalar<int>();
            Assert.Equal(5, actual);
        }

        private string distinctSql;
        [Fact, Order(2)]
        public void GetDistinctList() {
            var actual = fixture.Connection != null ?
                distinctSql.AsSql().ToList<string>(fixture.Connection).ToHashSet() :
                distinctSql.AsSql().ToList<string>().ToHashSet();
            var expected = new HashSet<string> { "HaIvri", "Avinu", "HaMelech", null };
            Assert.Equal(expected, actual);
        }

        private string datatableSql;
        [Fact, Order(2)]
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
