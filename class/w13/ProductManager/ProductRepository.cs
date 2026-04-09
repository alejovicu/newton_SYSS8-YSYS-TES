using Npgsql;

namespace ProductManager;

// Repository class responsible for accessing products in PostgreSQL database.
public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    // Constructor that accepts a PostgreSQL connection string.
    public ProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Retrieves all products from the database in a specific category.
    public List<Product> GetByCategory(string category)
    {
        var products = new List<Product>();

        // Create and open a new connection to the database
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        // Create a SQL command to fetch products in the given category
        var cmd = new NpgsqlCommand(
            "SELECT name, category, price FROM products WHERE category = @category",
            conn
        );
        cmd.Parameters.AddWithValue("category", category);

        // Execute the command and read the results
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            products.Add(new Product
            {
                Name = reader.GetString(0),
                Category = reader.GetString(1),
                Price = int.Parse(reader.GetString(2))
            });
        }

        return products;
    }
}
