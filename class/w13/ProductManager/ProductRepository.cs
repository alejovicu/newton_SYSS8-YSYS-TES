using System.Data;
using Npgsql;

namespace ProductManager;

public class ProductRepository : IProductRepository
{
    private readonly IDbConnection _connection;

    public ProductRepository()
    {
        _connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=testuser;Password=123password;Database=testdb");
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

        return products
            .Where(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}