namespace ProductManager.Tests;

using Moq;
using System.Data;

[TestClass]
public class UnitTestProductRepository
{
    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_ReturnsTechProducts_WhenCategoryIsTech()
    {
        // Arrange
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        // Mock DB contains 4 products from different categories:
        // | Id | Name              | Category | Price |
        // |----|-------------------|----------|-------|
        // | 1  | iPhone 17 Pro     | Tech     | 13000 |
        // | 2  | Salmon Fillet     | Food     | 120   |
        // | 3  | Gaming Laptop     | Tech     | 25000 |
        // | 4  | Face Cream        | Beauty   | 350   |

        mockReader.SetupSequence(r => r.Read())
            .Returns(true)   // Row 1
            .Returns(true)   // Row 2
            .Returns(true)   // Row 3
            .Returns(true)   // Row 4
            .Returns(false); // End

        mockReader.SetupSequence(r => r.GetInt32(0))
            .Returns(1)
            .Returns(2)
            .Returns(3)
            .Returns(4);

        mockReader.SetupSequence(r => r.GetString(1))
            .Returns("iPhone 17 Pro")
            .Returns("Salmon Fillet")
            .Returns("Gaming Laptop")
            .Returns("Face Cream");

        mockReader.SetupSequence(r => r.GetString(2))
            .Returns("Tech")
            .Returns("Food")
            .Returns("Tech")
            .Returns("Beauty");

        mockReader.SetupSequence(r => r.GetString(3))
            .Returns("13000")
            .Returns("120")
            .Returns("25000")
            .Returns("350");

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var repository = new ProductRepository(mockConnection.Object);

        // Act
        var techProducts = repository.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(2, techProducts.Count);
        Assert.AreEqual("iPhone 17 Pro", techProducts[0].Name);
        Assert.AreEqual("Tech", techProducts[0].Category);
        Assert.AreEqual("13000", techProducts[0].Price);
        Assert.AreEqual("Gaming Laptop", techProducts[1].Name);
        Assert.AreEqual("Tech", techProducts[1].Category);
        Assert.AreEqual("25000", techProducts[1].Price);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_ReturnsFoodProducts_WhenCategoryIsFood()
    {
        // Arrange
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        // Same 4 products
        mockReader.SetupSequence(r => r.Read())
            .Returns(true)
            .Returns(true)
            .Returns(true)
            .Returns(true)
            .Returns(false);

        mockReader.SetupSequence(r => r.GetInt32(0))
            .Returns(1).Returns(2).Returns(3).Returns(4);

        mockReader.SetupSequence(r => r.GetString(1))
            .Returns("iPhone 17 Pro")
            .Returns("Salmon Fillet")
            .Returns("Gaming Laptop")
            .Returns("Face Cream");

        mockReader.SetupSequence(r => r.GetString(2))
            .Returns("Tech")
            .Returns("Food")
            .Returns("Tech")
            .Returns("Beauty");

        mockReader.SetupSequence(r => r.GetString(3))
            .Returns("13000")
            .Returns("120")
            .Returns("25000")
            .Returns("350");

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var repository = new ProductRepository(mockConnection.Object);

        // Act
        var foodProducts = repository.GetProductsByCategory("Food");

        // Assert
        Assert.AreEqual(1, foodProducts.Count);
        Assert.AreEqual("Salmon Fillet", foodProducts[0].Name);
        Assert.AreEqual("Food", foodProducts[0].Category);
        Assert.AreEqual("120", foodProducts[0].Price);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_ReturnsEmpty_WhenCategoryHasNoProducts()
    {
        // Arrange
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        // Same 4 products (no Health category products)
        mockReader.SetupSequence(r => r.Read())
            .Returns(true)
            .Returns(true)
            .Returns(true)
            .Returns(true)
            .Returns(false);

        mockReader.SetupSequence(r => r.GetInt32(0))
            .Returns(1).Returns(2).Returns(3).Returns(4);

        mockReader.SetupSequence(r => r.GetString(1))
            .Returns("iPhone 17 Pro")
            .Returns("Salmon Fillet")
            .Returns("Gaming Laptop")
            .Returns("Face Cream");

        mockReader.SetupSequence(r => r.GetString(2))
            .Returns("Tech")
            .Returns("Food")
            .Returns("Tech")
            .Returns("Beauty");

        mockReader.SetupSequence(r => r.GetString(3))
            .Returns("13000")
            .Returns("120")
            .Returns("25000")
            .Returns("350");

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var repository = new ProductRepository(mockConnection.Object);

        // Act
        var healthProducts = repository.GetProductsByCategory("Health");

        // Assert
        Assert.AreEqual(0, healthProducts.Count);
    }
}
