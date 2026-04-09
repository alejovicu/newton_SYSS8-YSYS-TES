using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data;
using ProductManager;

namespace ProductManager.Tests;

[TestClass]
public class IntegrationTestsProductManager
{
    [TestMethod]
    [TestCategory("Integration")]
    public void TestGetProductsByCategoryRealDb()
    {
        // Arrange
        var productRepository = new ProductRepository();

        // Act
        var result = productRepository.GetProductsByCategory("Tech");

        // Assert
        Assert.IsTrue(result.Count > 0);
    }
}
