using Moq;
using Npgsql;
using System.Data;
using System.Security.Principal;


namespace ProductManager.Tests;

[TestClass]
public class TestProductRepo
{
    public void CleanUpProductsTable()
    {
        using var connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres");
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM products";
        cmd.ExecuteNonQuery();
        connection.Close();
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void TestGetProductsByCategoryRealDb()
    {
        CleanUpProductsTable();

        // Arrange
        var productRepository = new ProductRepository();

        var product = new Product
        {
            Name = "iPhone 17 Pro",
            Category = "Tech",
            Price = "13000"
        };
        var product2 = new Product
        {
            Name = "Banana",
            Category = "Food",
            Price = "2"
        };
        var expectedCount = 1;
        var expectedCategory = "Tech";
        productRepository.InsertProduct(product);
        productRepository.InsertProduct(product2);

        // Act
        var requestedProducts = productRepository.GetProductsByCategory(expectedCategory);

        // Assert
        Assert.AreEqual(expectedCount, requestedProducts.Count);
        Assert.AreEqual(expectedCategory, requestedProducts[0].Category);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void TestGetProductsByCategoryWithMock()
    {
        // Arrange
        var expectedCount = 1;
        var expectedCategory = "Tech";

        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();
        int readCallCount = 0;

        mockConnection.Setup(c => c.Open());
        mockConnection.Setup(c => c.Close());

        mockReader.Setup(r => r.Read()).Returns(() => readCallCount++ == 0);
        //Id
        mockReader.Setup(r => r.GetInt32(0)).Returns(1);
        //Name
        mockReader.Setup(r => r.GetString(1)).Returns("iPhone 17 Pro");
        //Category
        mockReader.Setup(r => r.GetString(2)).Returns("Tech");
        //Price
        mockReader.Setup(r => r.GetString(3)).Returns("13000");

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var productRepository = new ProductRepository(mockConnection.Object);

        // Act
        var requestedProducts = productRepository.GetProductsByCategory(expectedCategory);

        // Assert
        Assert.AreEqual(expectedCount, requestedProducts.Count);
        Assert.AreEqual(expectedCategory, requestedProducts[0].Category);
    }
}