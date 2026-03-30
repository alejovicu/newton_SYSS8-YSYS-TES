using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using ProductManager;

[TestClass]
public class ProductRepositoryIntegrationTests
{
    // Connection string to the test database (Docker PostgreSQL)
    private const string ConnectionString =
        "Host=localhost;Port=9999;Username=testuser;Password=mysecretpassword;Database=testdb";

    [TestInitialize]
    public void Setup()
    {
        // This method runs BEFORE each test
        // We reset the database state to ensure the test is repeatable
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        // 1. Clean the table to remove any previous test data
        using var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM products";
        deleteCmd.ExecuteNonQuery();
        // 2. Insert known test data
        // This guarantees that the test always runs with the same dataset
        using var insertCmd = connection.CreateCommand();
        insertCmd.CommandText = @"
            INSERT INTO products (name, category, price) VALUES
            ('iPhone 17 Pro', 'Tech', '13000'),
            ('Banana', 'Food', '20'),
            ('Lipstick', 'Beauty', '199'),
            ('Gaming Laptop', 'Tech', '22000')";
        insertCmd.ExecuteNonQuery();
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void GetProductsByCategory_Returns_Only_Tech()
    {
        var repository = new ProductRepository();

        var result = repository.GetProductsByCategory("Tech");

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(p => p.Category == "Tech"));
    }
}