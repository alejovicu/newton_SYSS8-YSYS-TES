namespace UsersApp;

using Npgsql;
using System.Collections.Generic;

public class UserRepository
{
    private readonly string _connectionString ;

    public UserRepository()
    {
        _connectionString = "Host=localhost;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres";
    }

    public List<User> GetAllUsers()
    {
        var users = new List<User>();

        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand("SELECT id, name FROM users", connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        return users;
    }
}