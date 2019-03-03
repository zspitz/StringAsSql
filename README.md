# StringAsSql

[![AppVeyor](https://img.shields.io/appveyor/ci/zspitz/StringAsSql/master.svg?label=appveyor)](https://ci.appveyor.com/project/zspitz/stringassql)
[![NuGet](http://img.shields.io/nuget/v/StringAsSql.svg)](https://www.nuget.org/packages/StringAsSql/)

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
## Parameter generation
A **parameter object** can be passed into the `AsSql` method, and `DbParameter`-derived instances will be added to the relevant comand's `Parameters` collection. The parameter object can be a collection of any of the following:
* Provider-specific `DbParameter`-derived class
* `KeyValuePair<string, T>`
* `Tuple<string, T ...>`
* `ValueTuple<string, T ...>`

In addition, if the provider supports [**named parameters**](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/configuring-parameters-and-parameter-data-types#working-with-parameter-placeholders), the parameter object can be a single object of a named or anonymous type;  the public properties/fields and their values will be used as parameter names and values.
```csharp
var parameterObject = new Person {
    LastName = "Smith",
    FirstName = "John"
};
int countOfSmith = "SELECT COUNT(*) FROM Persons WHERE LastName = @LastName AND FirstName = @FirstName".AsSql(parameterObject).ToList<Person>();
```
On the other hand, if the provider supports **positional parameters**,  the collection can contain primitive values -- `string`, `DateTime`, numeric types -- and the parameter names will be ignored.

(Note that if the provider only supports positional parameters, passing in a plain object is not a good idea, since there is no guarentee of the order in which the properties will be returned, and thus no control over which parameter has whcih value.)

For information about whether a given provider supports named or positional parameters, see [here](http://bobby-tables.com/adodotnet#placeholder-syntax).

## Reusing the same connection

The above code opens a new connection and command for each SQL statement. If you want to reuse the same connection, you have two choices:

* Set the `ConnectionFactory` to return the same connection each time, instead of a new connection
  ```csharp
  using (var conn = new OleDbConnection(connectionString)) {
      ConnectionFactory = () => conn;
      
      @"CREATE TABLE Persons (
          ID COUNTER PRIMARY KEY, 
          LastName TEXT,
          FirstName TEXT
      )".AsSql().Execute();
      
      // ...
  }
  ```
  
* Explicitly pass in the connection, as described in [the next section](https://github.com/zspitz/StringAsSql/blob/master/README.md#working-with-multiple-connections----explicitly-passing-the-connection)


## Working with multiple connections -- explicitly passing the connection

You can also pass in the connection on which to execute the given SQL:
```csharp
using (var conn1 = new OleDbConnection(connectionString1)) {
    using (var conn2 = new OleDbConnection(connectionString2)) {
    
        @"CREATE TABLE Persons (
            ID COUNTER PRIMARY KEY, 
            LastName TEXT,
            FirstName TEXT
        )".AsSql().Execute(conn2);
        
        // ...
    }
}
```

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