using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductManager;

namespace ProductManager.Tests;

public class FakeProductRepository : IProductRepository
{
    public List<Product> GetAllProducts()
    {
        return new List<Product>
        {
            new Product { Name = "iPhone 17 Pro", Category = "Tech", Price = 13000 },
            new Product { Name = "Banana", Category = "Food", Price = 25 },
            new Product { Name = "Lipstick", Category = "Beauty", Price = 199 }
        };
    }
}

[TestClass]
public class ProductManagerIntegrationTests
{
    [TestMethod]
    [TestCategory("Integration")]
    public void GetProductsByCategory_WithRepository_ShouldReturnMatchingProducts()
    {
        // Arrange
        var repository = new FakeProductRepository();
        var service = new ProductManagerService(repository);

        // Act
        var result = service.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("iPhone 17 Pro", result[0].Name);
    }
}