using System.Data;
using Moq;
using ProductManager;

namespace ProductManager.Tests;

[TestClass]
public sealed class ProductManagerIntegrationTests
{
    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_ReturnsOnlyRequestedCategory_FromMockedDbRows()
    {
        // Arrange: mocked DB returns 3 rows (Tech, Beauty, Tech)
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        mockReader.SetupSequence(r => r.Read())
            .Returns(true)
            .Returns(true)
            .Returns(true)
            .Returns(false);

        mockReader.SetupSequence(r => r.GetInt32(0))
            .Returns(1)
            .Returns(2)
            .Returns(3);

        mockReader.SetupSequence(r => r.GetString(1)) // Name
            .Returns("iPhone")
            .Returns("Hand cream")
            .Returns("Laptop");

        mockReader.SetupSequence(r => r.GetString(2)) // Category
            .Returns("Tech")
            .Returns("Beauty")
            .Returns("Tech");

        mockReader.SetupSequence(r => r.GetString(3)) // Price
            .Returns("13000")
            .Returns("200")
            .Returns("8000");

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var repository = new ProductRepository(mockConnection.Object);

        // Act
        var result = repository.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(p => p.Category == "Tech"));
        CollectionAssert.AreEqual(
            new[] { "iPhone", "Laptop" },
            result.Select(p => p.Name).ToArray());
    }
}