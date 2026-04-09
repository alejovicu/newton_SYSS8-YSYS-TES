namespace ProductManager
{

using Npgsql;
using System.Collections.Generic;
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

        return products;
    }

    /// <summary>
    /// Returns products filtered by category.
    /// The filtering is done in C# (not in SQL) as required by the assignment.
    /// </summary>
    public List<Product> GetProductsByCategory(string category)
    {
        var allProducts = GetAllProducts();
        return allProducts.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
}
