using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProductManager;

namespace ProductManager.Tests;

[TestClass]
public class ProductRepositoryUnitTests
{
    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_Filters_Correctly_From_Mocked_Db_Response()
    {
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
            .Returns("Gaming Laptop");

        mockReader.SetupSequence(r => r.GetString(2))
            .Returns("Tech")
            .Returns("Food")
            .Returns("Tech");

        mockReader.SetupSequence(r => r.GetString(3))
            .Returns("13000")
            .Returns("20")
            .Returns("22000");

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var repository = new ProductRepository(mockConnection.Object);

        var result = repository.GetProductsByCategory("Tech");

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(p => p.Category == "Tech"));
        Assert.AreEqual("iPhone 17 Pro", result[0].Name);
        Assert.AreEqual("Gaming Laptop", result[1].Name);
    } 
}