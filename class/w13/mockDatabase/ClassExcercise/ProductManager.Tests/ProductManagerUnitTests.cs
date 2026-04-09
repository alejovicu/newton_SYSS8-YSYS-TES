using Moq;
using System;
using System.Data;

namespace ProductManager.Tests;

[TestClass]
public class ProductManagerUnitTests
{
	[TestMethod]
    [TestCategory("UnitTest")]
    public void TestGetProductsByCategory()
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

        // Set up row values
        mockReader.SetupSequence(r => r.GetInt32(0))
                    .Returns(1)
                    .Returns(2)
                    .Returns(3);

        mockReader.SetupSequence(r => r.GetString(1))
                    .Returns("Xbox")
                    .Returns("Snes")
                    .Returns("Snus");

        mockReader.SetupSequence(r => r.GetString(2))
                    .Returns("Tech")
                    .Returns("Tech")
                    .Returns("Food");

        mockReader.SetupSequence(r => r.GetInt32(3))
                    .Returns(5600)
                    .Returns(2500)
                    .Returns(65);

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        //Setting the DB as source
        var prodMan = new ProductManager(mockConnection.Object);

        // Act

        var results = prodMan.GetProductsByCategory("Tech");

        // Assert
        Assert.IsTrue(results.All(c => c.Category == "Tech"));
        Assert.IsTrue(results.Any(c => c.Name == "Xbox"));
        Assert.IsTrue(results.Any(c => c.Name == "Snes"));
        Assert.IsFalse(results.Any(c => c.Name == "Snus"));
    }
}
