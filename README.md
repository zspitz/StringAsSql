# StringAsSql

[![AppVeyor](https://img.shields.io/appveyor/ci/zspitz/StringAsSql/master.svg?label=appveyor)](https://ci.appveyor.com/project/zspitz/stringassql)
[![NuGet](https://img.shields.io/nuget/v/StringAsSql.svg)](https://www.nuget.org/packages/StringAsSql/)

This library enables you to use SQL statements without a mess of multiple objects and `using` blocks. For example, you can write this:
```csharp
// setup ConnectionFactory -- not shown
// optionally, setup ParameterNameBuilder -- not shown

List<Person> persons = "SELECT * FROM Persons WHERE LastName LIKE ? + '%'".AsSql("A").ToList<Person>();
```
instead of this:
```csharp
var persons = new List<Person>();
using (var conn = new OleDbConnection(connectionString)) {
    conn.Open();
    var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT * FROM Persons WHERE LastName LIKE ? + '%'";
    cmd.Parameters.Add(
        new OleDbParameter("Parameter1", "A")
    );
    using (var rdr = cmd.ExecuteReader()) {
        while (rdr.Read()) {
            persons.Add(
                new Person {
                    ID = (int)rdr["ID"],
                    LastName = (string)rdr["LastName"],
                    FirstName = (string)rdr["FirstName"]
                }
            );
        }
    }
}
```
## Setting up the `ConnectionFactory`
Before using the `.AsSql` method, or creating an instance of `SqlString`, set the global `ConnectionFactory`:
```csharp
// using System.Data.OleDb;
// using static StringAsSql.SqlString;

// Define a connection string against an Access database
var connectionString = new OleDbConnectionStringBuilder {
    // ...
}.ToString();
ConnectionFactory = () => new OleDbConnection(connectionString);
```
See the [wiki](https://github.com/zspitz/StringAsSql/wiki) for more information.

## Some more examples:

Create a table:
```csharp
@"CREATE TABLE Persons (
  ID COUNTER PRIMARY KEY, 
  LastName TEXT,
  FirstName TEXT
)".AsSql().Execute();
```

Insert rows using parameters, via a collection of values:
```csharp
var insertSql = "INSERT INTO Persons (FirstName, LastName) VALUES (?, ?)";
insertSql.AsSql(new [] {"Artie", "Choke"}).Execute();
insertSql.AsSql(new [] {"Anna", "Lytics"}).Execute();
insertSql.AsSql(new [] {"Gerry", "Atric"}).Execute();
```

Get a scalar value:
```csharp
int count = "SELECT COUNT(*) FROM Persons".AsSql().ExecuteScalar<int>();
```

Get a list of objects, using a `TableDirect` command type:
```csharp
// using static System.Data.CommandType;
List<Person> persons = "Persons".AsSql(TableDirect).ToList<Person>();
```