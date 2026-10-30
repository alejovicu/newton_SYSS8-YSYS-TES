using System.Data;
using Moq;

namespace LibraryProject.MSTests;

[TestClass]
public class GetProducts
{
    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_ReturnsOnlyMatchingCategory()
    {

        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        mockReader.SetupSequence(r => r.Read())
            .Returns(true).Returns(true).Returns(true).Returns(false);

        mockReader.SetupSequence(r => r.GetString(0))
            .Returns("iPhone 17 pro").Returns("Banan").Returns("MacBook");
        mockReader.SetupSequence(r => r.GetString(1))
            .Returns("Tech").Returns("Food").Returns("Tech");
        mockReader.SetupSequence(r => r.GetString(2))
            .Returns("13000").Returns("10").Returns("20000");

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var manager = new ProductManager(mockConnection.Object);

        // Act
        var result = manager.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(p => p.Category == "Tech"));
    }
}
