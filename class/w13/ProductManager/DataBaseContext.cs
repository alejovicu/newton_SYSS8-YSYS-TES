using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace ProductManager;

public class DatabaseContext : IDatabaseContext
{
    private readonly IDbConnection? _connection;
    private readonly string? _connectionString;

    // För integration tests - string connection
    public DatabaseContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    // För unit tests - injicerad IDbConnection (mockat)
    public DatabaseContext(IDbConnection connection)
    {
        _connection = connection;
    }

    public IEnumerable<Product> GetAllProducts()
    {
        var products = new List<Product>();
        
        IDbConnection connection = _connection ?? new NpgsqlConnection(_connectionString);
        connection.Open();

        var query = "SELECT Id, Name, Category, Price FROM Products";
        using var command = connection.CreateCommand();
        command.CommandText = query;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Category = reader.GetString(2),
                Price = reader.GetString(3)
            });
        }

        connection.Close();
        return products;
    }
}

