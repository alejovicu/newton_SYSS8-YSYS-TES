namespace ProductManager.Tests;

using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Linq;

[TestClass]
public class UnitTestProductRepository
{
    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_WithMockedDb_ReturnsOnlyMatchingCategory()
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

        mockReader.SetupSequence(r => r.GetString(0))
            .Returns("iPhone 17 pro")
            .Returns("Protein Bar")
            .Returns("MacBook Air");

        mockReader.SetupSequence(r => r.GetString(1))
            .Returns("Tech")
            .Returns("Food")
            .Returns("Tech");

        mockReader.SetupSequence(r => r.GetDecimal(2))
            .Returns(13000m)
            .Returns(25m)
            .Returns(9000m);

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var repo = new ProductRepository(mockConnection.Object);

        // Act
        var result = repo.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(p => p.Category == "Tech"));
        CollectionAssert.AreEquivalent(new[] { "iPhone 17 pro", "MacBook Air" }, result.Select(r => r.Name).ToArray());
    }
}
