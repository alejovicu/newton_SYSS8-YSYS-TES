using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProductManager;

namespace ProductManager.Tests;

[TestClass]
public class ProductManagerServiceTests
{
    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_ShouldReturnOnlyTechProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Name = "iPhone 17 Pro", Category = "Tech", Price = 13000 },
            new Product { Name = "Banana", Category = "Food", Price = 25 },
            new Product { Name = "Laptop", Category = "Tech", Price = 9000 }
        };

        var mockRepo = new Mock<IProductRepository>();
        mockRepo.Setup(r => r.GetAllProducts()).Returns(products);

        var service = new ProductManagerService(mockRepo.Object);

        // Act
        var result = service.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(p => p.Category == "Tech"));
    }
    
    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_NoMatch_ShouldReturnEmptyList()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Name = "Banana", Category = "Food", Price = 25 }
        };

        var mockRepo = new Mock<IProductRepository>();
        mockRepo.Setup(r => r.GetAllProducts()).Returns(products);

        var service = new ProductManagerService(mockRepo.Object);

        // Act
        var result = service.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_WithMultipleCategories_ShouldFilterCorrectly()
    {
        // Arrange 
        var products = new List<Product>
        {
            new Product { Name = "iPhone", Category = "Tech", Price = 13000 },
            new Product { Name = "Banana", Category = "Food", Price = 25 },
            new Product { Name = "Lipstick", Category = "Beauty", Price = 200},
            new Product { Name = "Laptop", Category = "Tech", Price = 9000}
        };

        var mockRepo = new Mock<IProductRepository>();
        mockRepo.Setup(r => r.GetAllProducts()).Returns(products);

        var service = new ProductManagerService(mockRepo.Object);
        
        // Act
        var result = service.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(p => p.Category == "Tech"));
    }
    
}