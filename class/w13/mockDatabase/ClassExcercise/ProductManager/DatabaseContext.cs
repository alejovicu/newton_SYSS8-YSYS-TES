namespace ProductManager;

using System;
using Npgsql;
using System.Data;

public class DatabaseContext : IDatabaseContext
{
    private readonly IDbConnection? _connection;
    private readonly string? _connectionString;

    public DatabaseContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DatabaseContext(IDbConnection connection)
    {
        _connection = connection;
    }

    public IEnumerable<Product> GetAllProducts()
    {
        var products = new List<Product>();

        IDbConnection connection = _connection ?? new NpgsqlConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, name, category, price FROM products";

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
