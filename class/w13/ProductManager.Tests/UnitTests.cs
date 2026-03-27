using System.Data;
using Moq;

namespace ProductManager.Tests;

[TestClass]
public class ProductManagerUnitTests
{
    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_TechCategory_ReturnsTechProductsOnly()
    {
        // Arrange - Mocka databasen på låg nivå
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        int rowIndex = 0;

        // Simulera Read() - returnera true för 4 rader, sedan false
        mockReader.Setup(r => r.Read()).Returns(() =>
        {
            rowIndex++;
            return rowIndex <= 4;
        });

        // Kolumn 0: Id
        mockReader.Setup(r => r.GetInt32(0)).Returns(() => rowIndex);

        // Kolumn 1: Name
        mockReader.Setup(r => r.GetString(1)).Returns(() =>
        {
            return rowIndex switch
            {
                1 => "iPhone 17 Pro",
                2 => "Organic Apple",
                3 => "Hand Cream",
                4 => "MacBook Pro",
                _ => ""
            };
        });

        // Kolumn 2: Category
        mockReader.Setup(r => r.GetString(2)).Returns(() =>
        {
            return rowIndex switch
            {
                1 => "Tech",
                2 => "Food",
                3 => "Beauty",
                4 => "Tech",
                _ => ""
            };
        });

        // Kolumn 3: Price
        mockReader.Setup(r => r.GetString(3)).Returns(() =>
        {
            return rowIndex switch
            {
                1 => "13000",
                2 => "55",
                3 => "150",
                4 => "25000",
                _ => ""
            };
        });

        // Setup command och connection
        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
        mockConnection.Setup(c => c.Open());
        mockConnection.Setup(c => c.Close());

        // Act
        var context = new DatabaseContext(mockConnection.Object);
        var allProducts = context.GetAllProducts().ToList();
        var techProducts = allProducts.Where(p => p.Category == "Tech").ToList();

        // Assert
        Assert.IsNotNull(techProducts);
        Assert.AreEqual(2, techProducts.Count, "Expected 2 Tech products");
        Assert.IsTrue(techProducts.All(p => p.Category == "Tech"));
        Assert.AreEqual("iPhone 17 Pro", techProducts[0].Name);
        Assert.AreEqual("MacBook Pro", techProducts[1].Name);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_FoodCategory_ReturnsFoodProductsOnly()
    {
        // Arrange
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        int rowIndex = 0;

        mockReader.Setup(r => r.Read()).Returns(() =>
        {
            rowIndex++;
            return rowIndex <= 4;
        });

        mockReader.Setup(r => r.GetInt32(0)).Returns(() => rowIndex);
        mockReader.Setup(r => r.GetString(1)).Returns(() =>
        {
            return rowIndex switch { 1 => "iPhone 17 Pro", 2 => "Organic Apple", 3 => "Hand Cream", 4 => "MacBook Pro", _ => "" };
        });

        mockReader.Setup(r => r.GetString(2)).Returns(() =>
        {
            return rowIndex switch { 1 => "Tech", 2 => "Food", 3 => "Beauty", 4 => "Tech", _ => "" };
        });

        mockReader.Setup(r => r.GetString(3)).Returns(() =>
        {
            return rowIndex switch { 1 => "13000", 2 => "55", 3 => "150", 4 => "25000", _ => "" };
        });

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
        mockConnection.Setup(c => c.Open());
        mockConnection.Setup(c => c.Close());

        // Act
        var context = new DatabaseContext(mockConnection.Object);
        var allProducts = context.GetAllProducts().ToList();
        var foodProducts = allProducts.Where(p => p.Category == "Food").ToList();

        // Assert
        Assert.IsNotNull(foodProducts);
        Assert.AreEqual(1, foodProducts.Count);
        Assert.AreEqual("Organic Apple", foodProducts[0].Name);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_BeautyCategory_ReturnsBeautyProductsOnly()
    {
        // Arrange
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        int rowIndex = 0;

        mockReader.Setup(r => r.Read()).Returns(() =>
        {
            rowIndex++;
            return rowIndex <= 4;
        });

        mockReader.Setup(r => r.GetInt32(0)).Returns(() => rowIndex);
        mockReader.Setup(r => r.GetString(1)).Returns(() =>
        {
            return rowIndex switch { 1 => "iPhone 17 Pro", 2 => "Organic Apple", 3 => "Hand Cream", 4 => "MacBook Pro", _ => "" };
        });

        mockReader.Setup(r => r.GetString(2)).Returns(() =>
        {
            return rowIndex switch { 1 => "Tech", 2 => "Food", 3 => "Beauty", 4 => "Tech", _ => "" };
        });

        mockReader.Setup(r => r.GetString(3)).Returns(() =>
        {
            return rowIndex switch { 1 => "13000", 2 => "55", 3 => "150", 4 => "25000", _ => "" };
        });

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
        mockConnection.Setup(c => c.Open());
        mockConnection.Setup(c => c.Close());

        // Act
        var context = new DatabaseContext(mockConnection.Object);
        var allProducts = context.GetAllProducts().ToList();
        var beautyProducts = allProducts.Where(p => p.Category == "Beauty").ToList();

        // Assert
        Assert.IsNotNull(beautyProducts);
        Assert.AreEqual(1, beautyProducts.Count);
        Assert.AreEqual("Hand Cream", beautyProducts[0].Name);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_HealthCategory_ReturnsHealthProductsOnly()
    {
        // Arrange
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        int rowIndex = 0;

        mockReader.Setup(r => r.Read()).Returns(() =>
        {
            rowIndex++;
            return rowIndex <= 4;
        });

        mockReader.Setup(r => r.GetInt32(0)).Returns(() => rowIndex);
        mockReader.Setup(r => r.GetString(1)).Returns(() =>
        {
            return rowIndex switch { 1 => "iPhone 17 Pro", 2 => "Organic Apple", 3 => "Hand Cream", 4 => "MacBook Pro", _ => "" };
        });

        mockReader.Setup(r => r.GetString(2)).Returns(() =>
        {
            return rowIndex switch { 1 => "Tech", 2 => "Food", 3 => "Beauty", 4 => "Tech", _ => "" };
        });

        mockReader.Setup(r => r.GetString(3)).Returns(() =>
        {
            return rowIndex switch { 1 => "13000", 2 => "55", 3 => "150", 4 => "25000", _ => "" };
        });

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
        mockConnection.Setup(c => c.Open());
        mockConnection.Setup(c => c.Close());

        // Act
        var context = new DatabaseContext(mockConnection.Object);
        var allProducts = context.GetAllProducts().ToList();
        var healthProducts = allProducts.Where(p => p.Category == "Health").ToList();

        // Assert
        Assert.IsNotNull(healthProducts);
        Assert.AreEqual(0, healthProducts.Count);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_NonExistentCategory_ReturnsEmptyList()
    {
        // Arrange
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        int rowIndex = 0;

        mockReader.Setup(r => r.Read()).Returns(() =>
        {
            rowIndex++;
            return rowIndex <= 4;
        });

        mockReader.Setup(r => r.GetInt32(0)).Returns(() => rowIndex);
        mockReader.Setup(r => r.GetString(1)).Returns(() =>
        {
            return rowIndex switch { 1 => "iPhone 17 Pro", 2 => "Organic Apple", 3 => "Hand Cream", 4 => "MacBook Pro", _ => "" };
        });

        mockReader.Setup(r => r.GetString(2)).Returns(() =>
        {
            return rowIndex switch { 1 => "Tech", 2 => "Food", 3 => "Beauty", 4 => "Tech", _ => "" };
        });

        mockReader.Setup(r => r.GetString(3)).Returns(() =>
        {
            return rowIndex switch { 1 => "13000", 2 => "55", 3 => "150", 4 => "25000", _ => "" };
        });

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
        mockConnection.Setup(c => c.Open());
        mockConnection.Setup(c => c.Close());

        // Act
        var context = new DatabaseContext(mockConnection.Object);
        var allProducts = context.GetAllProducts().ToList();
        var nonExistentProducts = allProducts.Where(p => p.Category == "NonExistent").ToList();

        // Assert
        Assert.IsNotNull(nonExistentProducts);
        Assert.AreEqual(0, nonExistentProducts.Count);
    }
}


