using System.Data;
using Npgsql;

namespace ProductManager;

public class ProductRepository : IProductRepository
{
    private readonly IDbConnection _connection;

    public ProductRepository()
    {
        _connection =
            new NpgsqlConnection("Host=localhost;Port=5433;Username=postgres;Password=postgres;Database=productsdb");
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
            var product = new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Category = reader.GetString(2),
                Price = reader.GetString(3)
            };

            if (product.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            {
                products.Add(product);
            }
        }
        
        _connection.Close();
        
        return products;
    }
}