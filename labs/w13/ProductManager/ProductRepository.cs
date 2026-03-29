namespace ProductManager;

using Npgsql;
using System.Data;

public class ProductRepository : IProductRepository
{
    private readonly IDbConnection _connection;

    // Constructor for real database connection
    public ProductRepository()
    {
        _connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres");
    }

    // Constructor for dependency injection (mocking)
    public ProductRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public List<Product> GetAllProducts()
    {
        var products = new List<Product>();

        _connection.Open();

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT name, category, price FROM products";
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            products.Add(new Product
            {
                Name = reader.GetString(0),
                Category = reader.GetString(1),
                Price = reader.GetDecimal(2)
            });
        }

        _connection.Close();

        return products;
    }

    // Filter by category is done in C#, not in SQL (as per lab requirement)
    public List<Product> GetProductsByCategory(string category)
    {
        var allProducts = GetAllProducts();
        return allProducts.Where(p => p.Category == category).ToList();
    }
}
