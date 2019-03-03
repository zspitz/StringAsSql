using System.Data.SqlServerCe;
using Xunit;
using static StringAsSql.SqlString;

namespace StringAsSql.Tests {
    public class SqlCeFixtureBase : BaseFixture {
        public SqlCeFixtureBase(bool sharedConnection) : base("test.sdf", "test.ldf") {
            var connectionString = new SqlCeConnectionStringBuilder() {
                DataSource = dbFilePath
            }.ToString();

            // create the SqlCe file
            var engine = new SqlCeEngine(connectionString);
            engine.CreateDatabase();

            if (sharedConnection) {
                Connection = new SqlCeConnection(connectionString);
            } else {
                ConnectionFactory = () => new SqlCeConnection(connectionString);
            }
        }
    }

    public class SqlCeFixture : SqlCeFixtureBase {
        public SqlCeFixture() : base(false) { }
    }
    public class SqlCeFixture1Connection : SqlCeFixtureBase {
        public SqlCeFixture1Connection() : base(true) { }
    }

    public abstract class SqlCeBase : NamedParameterTests {
        private readonly SqlCeFixtureBase fixture;
        public SqlCeBase(SqlCeFixtureBase fixture) : base(
            fixture,
            @"CREATE TABLE Persons (
                    ID INT IDENTITY PRIMARY KEY, 
                    LastName NVARCHAR(50),
                    FirstName NVARCHAR(50)
            )",
            @"INSERT INTO Persons (LastName, FirstName)
                VALUES (@LastName, @FirstName)",
            "Persons",
            "SELECT COUNT(*) FROM Persons",
            "SELECT DISTINCT LastName FROM Persons",
            "SELECT ID, LastName, FirstName FROM Persons"
        ) {
            this.fixture = fixture;
            // if tests are being run in parallel, this is intended to ensure that the global connection factory is set for these tests
            // not sure if it works that way, other stuff might happen between this line and the individual tests
            ConnectionFactory = fixture.ConnectionFactory;
        }
    }

    public class SqlCe : SqlCeBase, IClassFixture<SqlCeFixture> {
        public SqlCe(SqlCeFixture fixture) : base(fixture) { }
    }
    public class SqlCe1Connection : SqlCeBase, IClassFixture<SqlCeFixture1Connection> {
        public SqlCe1Connection(SqlCeFixture1Connection fixture) : base(fixture) { }
    }

}
