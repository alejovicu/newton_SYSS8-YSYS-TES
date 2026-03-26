namespace UsersApp
{

using Npgsql;
using System.Collections.Generic;
using System.Data;

public class UserRepositoryWithInterface : IUserRepository
{
    private readonly IDbConnection _connection;

    // Constructor for real database connection
    public UserRepositoryWithInterface()
    {
        _connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres");
    }

    // Constructor for dependency injection (mocking)
    public UserRepositoryWithInterface(IDbConnection connection)
    {
        _connection = connection;
    }

    public List<User> GetAllUsers()
    {
        var users = new List<User>();

        _connection.Open();

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT id, name FROM users";
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        _connection.Close();

        return users;
    }

    public void InsertUser(User user)
    {
        _connection.Open();
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "INSERT INTO users (name) VALUES ('" + user.Name + "')";
        using var reader = cmd.ExecuteReader();

        _connection.Close();

    }
}
}