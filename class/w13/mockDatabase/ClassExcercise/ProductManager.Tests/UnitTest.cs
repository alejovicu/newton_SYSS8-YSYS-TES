using Moq; 
using System.Data;

namespace ProductManager.Tests;

[TestClass]
public class UnitTest
{
    [TestMethod]
    [TestCategory("UnitTest")]
    public void TestGetProductsByCategory()
    {
        // Arrange 
        var mockConnection = new Mock<IDbConnection>();
        var mockCmd = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();
        var mockParameter = new Mock<IDbDataParameter>();
        var mockParams = new Mock<IDataParameterCollection>();
        
        mockReader.SetupSequence(r => r.Read())
            .Returns(true)
            .Returns(true)
            .Returns(false);

        int count = 0;
        mockReader.Setup(r => r.GetString(2)).Returns(() =>
        {
            return count++ == 0 ? "Tech" : "Beauty";
        });
        
        // mapping up the columns
        mockReader.Setup(r => r.GetInt32(0)).Returns(99);
        mockReader.Setup(r => r.GetString(1)).Returns("iPhone 17 Pro");
        mockReader.Setup(r => r.GetString(3)).Returns("13000");
        
        mockCmd.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockCmd.Setup(c => c.CreateParameter()).Returns(mockParameter.Object);
        mockCmd.Setup(c => c.Parameters).Returns(mockParams.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCmd.Object);
        
        var repo = new ProductRepository(mockConnection.Object);
        
        // Act 
        var result = repo.GetProductsByCategory("Tech");
        
        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("iPhone 17 Pro", result[0].Name);
        Assert.AreEqual("Tech", result[0].Category);
    }
}