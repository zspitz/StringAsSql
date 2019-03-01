# StringAsSql
This library enables you to use SQL statements without a mess of multiple objects and `using` blocks.

Setup the `ConnectionFactory`:
```csharp
// using System.Data.OleDb;
// using static StringAsSql.SqlString;

// Define a connection string against an Access database
var connectionString = new OleDbConnectionStringBuilder {
    // ...
}.ToString();
ConnectionFactory = () => new OleDbConnection(connectionString);
```

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
> Note that OLE DB only supports [positional parameters](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/configuring-parameters-and-parameter-data-types#working-with-parameter-placeholders), so we're only passing in values for the parameters. For other providers which support named parameters, the parameter object can be a an object of a named or anonymous type;  the public properties/fields and their values will be used as parameter names and values.
>
> Alternatively, the parameter object can be a collection of any of the following:
> * `KeyValuePair<string, T>`
> * `Tuple<string, T ...>`
> * `ValueTuple<string, T ...>`
>
> For information about whether a given provider supports named or positional parameters, see [here](http://bobby-tables.com/adodotnet#placeholder-syntax).

Get a scalar value:
```csharp
int count = "SELECT COUNT(*) FROM Persons".AsSql().ExecuteScalar<int>();
```

Get a list of objects:
```csharp
// using static System.Data.CommandType;
List<Person> persons = "Persons".AsSql(TableDirect).ToList<Person>();
```