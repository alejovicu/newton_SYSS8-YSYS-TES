using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data;
using ProductManager;

namespace ProductManager.Tests;

[TestClass]
public class GetProductsByCategory
{
    [TestMethod]
    [TestCategory("UnitTest")]
    public void TestGetProductsByCategoryWithMock()
    {
        // Arrange
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        mockReader.SetupSequence(r => r.Read())
                    .Returns(true)
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);

        // Set up first row values
        mockReader.SetupSequence(r => r.GetString(0))
                    .Returns("Headphones")
                    .Returns("Airpods")
                    .Returns("Apple");

        mockReader.SetupSequence(r => r.GetString(1))
                    .Returns("Tech")
                    .Returns("Tech")
                    .Returns("Food");

        mockReader.SetupSequence(r => r.GetString(2))
                    .Returns("450")
                    .Returns("700")
                    .Returns("12");

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var productRepository = new ProductRepository(mockConnection.Object);

        // Act
        var result = productRepository.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(2, result.Count);
    }
}