using Microsoft.Office.Interop.Access.Dao;
using System.Data.OleDb;
using Xunit;
using static Microsoft.Office.Interop.Access.Dao.DatabaseTypeEnum;
using static StringAsSql.SqlString;

namespace StringAsSql.Tests {
    public class MSAccessFixtureBase : BaseFixture {
        const string dbLangGeneral = ";LANGID=0x0409;CP=1252;COUNTRY=0";

        public MSAccessFixtureBase(bool sharedConnection) : base("test.mdb", "test.ldb") {
            // create the MDB file
            var engine = new DBEngine();
            var dbs = engine.CreateDatabase(dbFilePath, dbLangGeneral, dbVersion40);
            dbs.Close();
            dbs = null;

            var connectionString = new OleDbConnectionStringBuilder() {
                Provider = "Microsoft.Jet.OLEDB.4.0",
                DataSource = dbFilePath
            }.ToString();

            if (sharedConnection) {
                Connection = new OleDbConnection(connectionString);
            } else {
                ConnectionFactory = () => new OleDbConnection(connectionString);
            }
        }
    }

    public class MSAccessFixture : MSAccessFixtureBase {
        public MSAccessFixture() : base(false) { }
    }
    public class MSAccessFixture1Connection : MSAccessFixtureBase {
        public MSAccessFixture1Connection() : base(true) { }
    }

    public abstract class MSAccessBase : PositionalParameterTests {
        private readonly MSAccessFixtureBase fixture;
        public MSAccessBase(MSAccessFixtureBase fixture) : base(
            fixture,
            @"CREATE TABLE Persons (
                    ID COUNTER PRIMARY KEY, 
                    LastName TEXT,
                    FirstName TEXT
            )",
            @"INSERT INTO Persons (LastName, FirstName)
                VALUES (?, ?)",
            "Persons",
            "SELECT COUNT(*) FROM Persons",
            "SELECT DISTINCT LastName FROM Persons",
            "SELECT ID, LastName, FirstName FROM Persons",
            new[] {
                new OleDbParameter(),
                new OleDbParameter()
            }
        ) {
            this.fixture = fixture;
            // if tests are being run in parallel, this is intended to ensure that the global connection factory is set for these tests
            // not sure if it works that way, other stuff might happen between this line and the individual tests
            ConnectionFactory = fixture.ConnectionFactory;
        }
    }

    public class MSAccess : MSAccessBase, IClassFixture<MSAccessFixture> {
        public MSAccess(MSAccessFixture fixture) : base(fixture) { }
    }
    public class MSAccess1Connection : MSAccessBase, IClassFixture<MSAccessFixture1Connection> {
        public MSAccess1Connection(MSAccessFixture1Connection fixture) : base(fixture) { }
    }
}
