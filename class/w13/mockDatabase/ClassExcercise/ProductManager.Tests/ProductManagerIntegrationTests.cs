using Npgsql;
using System.Data;
using System.Runtime.InteropServices.Marshalling;

namespace ProductManager.Tests;

[TestClass]
public sealed class ProductManagerIntegrationTests
{
    public void CleanUpProducts()
    {
        using var connection = new NpgsqlConnection("Host=localhost;Port=9999;Username=testuser;Password=mysecretpassword;Database=testdb");
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM product WHERE name LIKE 'TESTPROD_%'";
        cmd.ExecuteNonQuery();
        connection.Close();
    }

    [TestCleanup]
    public void CleanUp()
    {
        // Removes the test products from the db AFTER the test has run.
        CleanUpProducts();
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void TestGetProductsByCategory()
    {
        // Arrange        
        var productManager = new ProductManager();

        // Test products
        var prod1 = new Product { Name = "TESTPROD_Apple", Category = "Food", Price = 25 };
        var prod2 = new Product { Name = "TESTPROD_Kiwi", Category = "Food", Price = 35 };
        var prod3 = new Product { Name = "TESTPROD_Snes", Category = "Tech", Price = 2000 };
        
        // Add test products to the db
        productManager.AddProduct(prod1);
        productManager.AddProduct(prod2);
        productManager.AddProduct(prod3);

        // Act
        var results = productManager.GetProductsByCategory("Food");        

        // Assert
        Assert.IsTrue(results.All(c => c.Category == "Food"));
        Assert.IsTrue(results.Any(c => c.Name == "TESTPROD_Apple"));
        Assert.IsTrue(results.Any(c => c.Name == "TESTPROD_Kiwi"));
        Assert.IsFalse(results.Any(c => c.Name == "TESTPROD_Snes"));
    }
}
