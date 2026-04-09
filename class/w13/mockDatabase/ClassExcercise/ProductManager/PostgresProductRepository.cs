using System.Data;
using Npgsql;

namespace ProductManager;

public class PostgresProductRepository
{
    private readonly IDbConnection _connection;

    public PostgresProductRepository()
    {
        _connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=testuser;Password=mysecretpassword;Database=testdb");
    }
    
    public PostgresProductRepository(IDbConnection connection)
    {
       _connection = connection;
    }   

    public List<Product> GetAllProducts()
    {
        var products = new List<Product>();


        _connection.Open();

        // hämtar alla produkter från databasen.
        const string sql = "SELECT Id, Name, Category, Price FROM Product";

        using var command = _connection.CreateCommand();
        command.CommandText = sql;
        
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

        _connection.Close();
        
        return products;
    }
    
    public List<Product> GetProductsByCategory(string category)
    {
        var allProducts = GetAllProducts();

        return allProducts.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}