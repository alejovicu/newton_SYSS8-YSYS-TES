using Moq;
using System;
using System.Data;

namespace ProductManager.Tests;

// Unit tests for ProductManager using mocked data.
[TestClass]
public class ProductManagerUnitTests
{
    // Tests that GetProductsByCategory correctly filters products by category.
    // Uses a mocked repository to simulate database responses.
    [TestMethod]
    [TestCategory("UnitTest")]
    public void TestGetProductsByCategory()
    {
        // Arrange
        // Create a mock repository using Moq
        var mockRepo = new Mock<IProductRepository>();

        // Setup the mock to return a predefined list of products when GetByCategory("Tech") is called
        mockRepo.Setup(r => r.GetByCategory("Tech"))
            .Returns(new List<Product>
            {
            new Product { Name = "Switch 2", Category = "Tech", Price = 5400 },
            new Product { Name = "GeForce 2300", Category = "Tech", Price = 6799 },
            new Product { Name = "Pepsi", Category = "Food", Price = 12 }
            });

        // Create ProductManager instance with the mocked repository
        var prodMan = new ProductManager(mockRepo.Object);

        // Act
        // Call the method under test
        var results = prodMan.GetProductsByCategory("Tech");

        // Assert
        // Verify that all returned products belong to the "Tech" category
        Assert.IsTrue(results.All(c => c.Category == "Tech"));

        // Verify that specific products are included
        Assert.IsTrue(results.Any(c => c.Name == "Switch 2"));
        Assert.IsTrue(results.Any(c => c.Name == "GeForce 2300"));

        // Verify that products from other categories are excluded
        Assert.IsFalse(results.Any(c => c.Name == "Pepsi"));
    }
}