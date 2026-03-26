namespace ProductManager.Tests;

using ProductManagerApp;
using Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class IntegrationTestsProductManager
{
    [TestInitialize]
    public void Initialize()
    {
        // Cleanup the products table before each test
        using var connection = new NpgsqlConnection("Host=localhost;Port=5431;Username=postgres;Password=mysecretpassword;Database=postgres");
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM products";
        cmd.ExecuteNonQuery();
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void TestGetProductsByCategory()
    {
        // Arrange
        var productManager = new ProductManager();
        productManager.InsertProduct(new Product { Name = "iPhone 15", Category = "Tech", Price = 999.99m });

        // Act
        var results = productManager.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual("Tech", results[0].Category);
    }
}