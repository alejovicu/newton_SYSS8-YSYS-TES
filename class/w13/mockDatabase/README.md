# Lab 2: Mocking

In this assignment you will learn different ways to mock

- Pieces of your own code
- External API calls
- Database calls


## Database

Create Database:

```shell
docker run -d \
  --name postgres-test \
  -e POSTGRES_USER=testuser \
  -e POSTGRES_PASSWORD=testpass \
  -e POSTGRES_DB=testdb \
  -p 5432:5432 \
  postgres:latest
```

Login into the database and create the table:
```shell
docker exec -it postgres-test psql -U testuser -d testdb
```

```SQL
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL
);

INSERT INTO users (name) VALUES
('Alice'),
('Bob'),
('Charlie');
```

Validate the data
```SQL
SELECT * FROM users;
```


## Application

From a terminal run:
```shell
# Create Project
dotnet new sln -n MockDBCall
cd MockDBCall

# Create a Library
dotnet new classlib -n UserApp
dotnet sln add UserApp

# Create the Tests for the library
dotnet new mstest -n UserApp.Tests
dotnet sln add UserApp.Tests

# Link the tests with the Library
dotnet add UserApp.Tests reference UserApp

#Install library
dotnet add package Npgsql
```

## Create the App project files:

Create the model file `UserApp/User.cs`
```C#
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

Create repository that calls PostgreSQL `UserApp/UserRepository.cs`
```C#
using Npgsql;
using System.Threading.Tasks;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<User> GetUserById(int id)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT id, name FROM users WHERE id = @id",
            conn
        );

        cmd.Parameters.AddWithValue("id", id);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            };
        }

        return null;
    }
}
```

Create service `UserApp/UserService.cs`
```C#
using System.Threading.Tasks;

public class UserService
{
    private readonly UserRepository _repo;

    public UserService(UserRepository repo)
    {
        _repo = repo;
    }

    public async Task<string> GetUserName(int id)
    {
        var user = await _repo.GetUserById(id);
        return user?.Name;
    }
}
```


## Create Test Project Files

Let's create an MSTest **integration** test

```C#
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

[TestClass]
public class UserServiceTests
{
    private const string ConnString =
        "Host=localhost;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres";

    [TestMethod]
    public async Task GetUserName_ReturnsUserFromDatabase()
    {
        // Arrange
        var repo = new UserRepository(ConnString);
        var service = new UserService(repo);

        // Act
        var name = await service.GetUserName(1);

        // Assert
        Assert.AreEqual("Alice", name);
    }
}

```


Start a PostgresDB with Docker
```
docker run --name postgres -e POSTGRES_PASSWORD=mysecretpassword -d postgres
```