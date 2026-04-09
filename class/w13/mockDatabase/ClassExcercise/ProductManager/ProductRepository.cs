namespace ProductManager;

using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class ProductRepository
{
    private readonly IDbConnection _connection;

    public ProductRepository()
    {
        _connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres");
    }

    public ProductRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public List<Product> GetProductsByCategory(string category)
    {
        var products = new List<Product>();
        _connection.Open();
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT id, name, category, price FROM products";
        using var reader = cmd.ExecuteReader();
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
        _connection.Close();
        return products.Where(p => p.Category == category).ToList();
    }

    public void InsertProduct(Product product)
    {
        _connection.Open();
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "INSERT INTO products (name, category, price) VALUES ('" + product.Name + "', '" + product.Category + "', '" + product.Price + "')";
        using var reader = cmd.ExecuteReader();
        _connection.Close();
    }
}
