using System.Data;
using Moq;
using Npgsql;
using ProductManager;

namespace ProductManager.Tests;

[TestClass]
public sealed class Test1
{
    public void EmptyWarehouse()
    {
        using var connection = new NpgsqlConnection("Host = localhost; Port = 5432; Username = postgres; Password = mysecretpassword; Database = postgres");
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM products";
        cmd.ExecuteNonQuery();

        connection.Close();
    }

    [TestMethod, TestCategory("Integration")]
    public void GetProductsByCategoryRealDbTest()
    {
        // Arrange
        EmptyWarehouse();

        var productRepository = new ProductRepository();
        var products = new List<Product>
        {
            //TECH
            new Product{Name = "Laptop", Category = "Tech", Price = "10 kr"},
            new Product{Name = "Mobile", Category = "Tech", Price = "15 kr"},

            //FOOD
            new Product{Name = "Apple", Category = "Food", Price = "20 kr"},
            new Product{Name = "Orange", Category = "Food", Price = "25 kr"},

            //Health
            new Product{Name = "Syringe", Category = "Health", Price = "30 kr"},
            new Product{Name = "Painkillers", Category = "Health", Price = "35 kr"},

            //Beauty
            new Product{Name = "Mascara", Category = "Beauty", Price = "40 kr"},
            new Product{Name = "Blush", Category = "Beauty", Price = "45 kr"}
        };

        var expectedStock = 2;
        foreach (var product in products)
        {
            productRepository.InsertProduct(product);
        }

        //Categories
        string category1 = "Tech";
        string category2 = "Food";
        string category3 = "Health";
        string category4 = "Beauty";

        // Act
        var resultTech = productRepository.GetProductsByCategory(category1);
        var resultFood = productRepository.GetProductsByCategory(category2);
        var resultHealth = productRepository.GetProductsByCategory(category3);
        var resultBeauty = productRepository.GetProductsByCategory(category4);

        // Assert
        Assert.AreEqual(expectedStock, resultTech.Count());
        Assert.AreEqual(expectedStock, resultFood.Count());
        Assert.AreEqual(expectedStock, resultHealth.Count());
        Assert.AreEqual(expectedStock, resultBeauty.Count());
    }

    [TestMethod, TestCategory("UnitTest")]
    public void GetProductsByCategoryMockTest()
    {
        // Arrange

        // Create a Mocked DB
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();

        mockCommand.Setup(c => c.ExecuteReader()).Returns(() =>
        {
            var mockReader = new Mock<IDataReader>();

            // Populate the data it will be returned when a query is executed towards the DB
            // Set up Read() to return true for each row, then false
            mockReader.SetupSequence(r => r.Read())
                      .Returns(true)   // first row
                      .Returns(true)   // second row
                      .Returns(true)   // third row
                      .Returns(true)   // fourth row
                      .Returns(false); // end

            // Set up first row values
            mockReader.SetupSequence(r => r.GetInt32(0))
                      .Returns(1)     // first row Id
                      .Returns(2)     // second row Id
                      .Returns(3)     // third row Id
                      .Returns(4);    // fourth row Id

            mockReader.SetupSequence(r => r.GetString(1))
                      .Returns("iPhone")      // first row Name
                      .Returns("Hand cream") // second row Name
                      .Returns("Laptop") // third row Name
                      .Returns("Lipstick"); // fourth row Name

            mockReader.SetupSequence(r => r.GetString(2))
                      .Returns("Tech")    // first row Category
                      .Returns("Beauty") // second row Category
                      .Returns("Tech") // third row Category
                      .Returns("Beauty"); // fourth row Category

            mockReader.SetupSequence(r => r.GetString(3))
                      .Returns("13000") // first row Price
                      .Returns("200")  // second row Price
                      .Returns("15500") // third row Price
                      .Returns("60"); // fourth row Price

            return mockReader.Object;
        });
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var category1 = "Tech";
        var category2 = "Beauty";

        // Create repository with mocked connection for testing
        var productRepository = new ProductRepository(mockConnection.Object);


        // Act
        var result1 = productRepository.GetProductsByCategory(category1);
        var result2 = productRepository.GetProductsByCategory(category2);

        // Assert

        // tech
        Assert.AreEqual(2, result1.Count());

        // beauty
        Assert.AreEqual(2, result2.Count());
    }
}