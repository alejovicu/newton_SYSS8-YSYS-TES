namespace ProductManager.Tests;

using Moq;
using System.Data;

[TestClass]
public class UnitTests
{
    private (Mock<IDbConnection>, Mock<IDbCommand>, Mock<IDataReader>) CreateMockDb()
    {
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        int rowIndex = 0;
        mockReader.Setup(r => r.Read()).Returns(() => rowIndex++ < 4);

        mockReader.Setup(r => r.GetInt32(0)).Returns(() => rowIndex);
        mockReader.Setup(r => r.GetString(1)).Returns(() => rowIndex switch
        {
            1 => "Samsung Galaxy S25",
            2 => "Banana Smoothie",
            3 => "Lip Balm",
            4 => "Dell XPS 15",
            _ => ""
        });
        mockReader.Setup(r => r.GetString(2)).Returns(() => rowIndex switch
        {
            1 => "Tech",
            2 => "Food",
            3 => "Beauty",
            4 => "Tech",
            _ => ""
        });
        mockReader.Setup(r => r.GetString(3)).Returns(() => rowIndex switch
        {
            1 => "9999",
            2 => "35",
            3 => "89",
            4 => "18500",
            _ => ""
        });

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        return (mockConnection, mockCommand, mockReader);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_TechCategory_ReturnsTwoProducts()
    {
        var (mockConnection, _, _) = CreateMockDb();
        var productManager = new ProductManager(new DatabaseContext(mockConnection.Object));

        var result = productManager.GetProductsByCategory("Tech");

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(p => p.Category == "Tech"));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_FoodCategory_ReturnsOneProduct()
    {
        var (mockConnection, _, _) = CreateMockDb();
        var productManager = new ProductManager(new DatabaseContext(mockConnection.Object));

        var result = productManager.GetProductsByCategory("Food");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Banana Smoothie", result[0].Name);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_NonExistentCategory_ReturnsEmptyList()
    {
        var (mockConnection, _, _) = CreateMockDb();
        var productManager = new ProductManager(new DatabaseContext(mockConnection.Object));

        var result = productManager.GetProductsByCategory("Health");

        Assert.AreEqual(0, result.Count);
    }
}