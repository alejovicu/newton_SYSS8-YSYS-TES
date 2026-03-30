namespace DefaultNamespace;

public class ProductRepository
{
    private readonly IDbConnection _connection;

    public ProductRepository()
    {
        _connection = new NpgsqlConnection(
            "Host=localhost;Port=9999;Username=testuser;Password=mysecretpassword;Database=testdb");
    }

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

    public List<Product> GetProductsByCategory(string category)
    {
        var allProducts = GetAllProducts();

        return allProducts
            .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}