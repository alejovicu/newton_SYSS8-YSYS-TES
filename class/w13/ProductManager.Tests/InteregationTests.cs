using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
namespace ProductManager.Tests;

[TestClass]
public class IntegrationTests
{
    // Connection string to the PostgreSQL test database
    private readonly string conn =
        "Host=localhost;Port=5500;Username=postgres;Password=mysecretpassword;Database=productsdb";

    // Integration test for GetProductsByCategory.
    [TestMethod]
    [TestCategory("Integration")]
    public void GetProductsByCategory()
    {
        // Arrange: fetch products in the "Tech" category
        var repo = new ProductRepository(conn);
        var manager = new ProductManager(repo);

        // Act: fetch products in the "Tech" category
        var result = manager.GetProductsByCategory("Tech");

        // Assert: validate that all returned products are actually in the "Tech" category
        Assert.IsTrue(result.Count > 0);
        Assert.IsTrue(result.All(p => p.Category == "Tech"));
    }
}
