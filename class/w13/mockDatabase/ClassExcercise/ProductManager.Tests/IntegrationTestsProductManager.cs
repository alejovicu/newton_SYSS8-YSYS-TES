namespace ProductManager.Tests;

using ProductManagerApp;
using Npgsql;
using System.Linq;

[TestClass]
public class IntegrationTestsProductManager
{
    private const string ConnectionString = 
        "Host=localhost;Port=5431;Username=postgres;Password=mysecretpassword;Database=postgres";

    private const string TestProductName1 = "TEST_Android Galaxy 15";
    private const string TestProductName2 = "TEST_MacBook Pro";
    private const string TestProductName3 = "TEST_Margherita Pizza";

    private static void InsertProduct(NpgsqlCommand cmd, string name, string category, double price)
    {
        cmd.CommandText = "INSERT INTO products (name, category, price) VALUES (@name, @category::product_category, @price)";
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@category", category);
        cmd.Parameters.AddWithValue("@price", price);
        cmd.ExecuteNonQuery();
        cmd.Parameters.Clear();
    }

    [TestCleanup]
    public void Cleanup()
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM products WHERE name LIKE 'TEST_%'";
        cmd.ExecuteNonQuery();
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void TestGetProductsByCategory()
    {
        // Arrange
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        using var cmd = connection.CreateCommand();

        InsertProduct(cmd, TestProductName1, "Tech", 999.99);
        InsertProduct(cmd, TestProductName2, "Tech", 799.99);
        InsertProduct(cmd, TestProductName3, "Food", 12.99);

        var productManager = new ProductManager();

        // Act
        var results = productManager.GetProductsByCategory("Tech");

        // Assert
        Assert.IsTrue(results.All(p => p.Category == "Tech"));
        Assert.IsTrue(results.Any(p => p.Name == TestProductName1));
        Assert.IsTrue(results.Any(p => p.Name == TestProductName2));
        Assert.IsFalse(results.Any(p => p.Name == TestProductName3));
    }
}