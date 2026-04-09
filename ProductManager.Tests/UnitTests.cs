using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProductManager;

namespace ProductManager.Tests;

[TestClass]
public class UnitTests
{
    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_WithMockedDatabase_ShouldReturnOnlyTechProducts()
    {
        // Arrange
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        int rowIndex = 0;
        
        mockReader.Setup(r => r.Read()).Returns(() =>
        {
            rowIndex++;
            return rowIndex <= 3;
        });

        mockReader.Setup(r => r.GetString(0)).Returns(() =>
        {
            if (rowIndex == 1) return "Laptop";
            if (rowIndex == 2) return "Banana";
            return "Shampoo";
        });

        mockReader.Setup(r => r.GetString(1)).Returns(() =>
        {
            if (rowIndex == 1) return "Tech";
            if (rowIndex == 2) return "Food";
            return "Beauty";
        });

        mockReader.Setup(r => r.GetString(2)).Returns(() =>
        {
            if (rowIndex == 1) return "999";
            if (rowIndex == 2) return "20";
            return "79";
        });

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var repository = new ProductRepository(mockConnection.Object);

        // Act
        var result = repository.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Laptop", result[0].Name);
        Assert.AreEqual("Tech", result[0].Category);
        Assert.AreEqual("999", result[0].Price);
    }
}