using Moq;
using System.Data;

namespace ProductManager.Tests;

[TestClass]
public class UnitTestProductRepository
{
    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_ReturnsOnlyMatchingCategory()
    {
        // arrange
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

        mockReader.SetupSequence(r => r.GetString(1))
            .Returns("iPhone 17 Pro")
            .Returns("Banana")
            .Returns("MacBook Pro");

        mockReader.SetupSequence(r => r.GetString(2))
            .Returns("Tech")
            .Returns("Food")
            .Returns("Tech");

        mockReader.SetupSequence(r => r.GetString(3))
            .Returns("13000")
            .Returns("10")
            .Returns("25000");

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var repo = new ProductRepository(mockConnection.Object);

        // act
        var result = repo.GetProductsByCategory("Tech");

        // assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Tech", result[0].Category);
        Assert.AreEqual("Tech", result[1].Category);
    }
}
