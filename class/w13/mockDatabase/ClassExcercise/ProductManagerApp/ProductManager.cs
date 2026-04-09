namespace ProductManagerApp;

using Npgsql;
using System.Data;

public class ProductManager
{
    private readonly IDbConnection _connection;

    public ProductManager(IDbConnection connection)
    {
        _connection = connection;
    }

    public ProductManager() 
        : this(new NpgsqlConnection("Host=localhost;Port=5431;Username=postgres;Password=mysecretpassword;Database=postgres"))
    {
    }

    public List<Product> GetProductsByCategory(string category)
    {
        var products = new List<Product>();
        var filteredProducts = new List<Product>();

        _connection.Open();
        try
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT id, name, category, price FROM products";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id       = reader.GetInt32(0),
                    Name     = reader.GetString(1),
                    Category = reader.GetString(2),
                    Price    = reader.GetDecimal(3)
                });
            }
        }
        finally
        {
            _connection.Close();
        }

        foreach (var product in products)
        {
            if (product.Category == category)
            {
                filteredProducts.Add(product);
            }
        }

        return filteredProducts;
    }
}