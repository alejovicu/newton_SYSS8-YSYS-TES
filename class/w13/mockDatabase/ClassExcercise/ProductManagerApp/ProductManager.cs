namespace ProductManagerApp;

using Npgsql;

public class ProductManager
{
    private const string ConnectionString = 
        "Host=localhost;Port=5431;Username=postgres;Password=mysecretpassword;Database=postgres";

    public List<Product> GetProductsByCategory(string category)
    {
        var products = new List<Product>();

        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, name, category, price FROM products WHERE category = @category::product_category";
        cmd.Parameters.AddWithValue("@category", category);

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

        return products;
    }

    public void InsertProduct(Product product)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO products (name, category, price) VALUES (@name, @category::product_category, @price)";
        cmd.Parameters.AddWithValue("@name",     product.Name);
        cmd.Parameters.AddWithValue("@category", product.Category);
        cmd.Parameters.AddWithValue("@price",    product.Price);

        cmd.ExecuteNonQuery();
    }
}