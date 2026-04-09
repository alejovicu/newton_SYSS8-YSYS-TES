namespace ProductManager.Tests;
using Moq;
using Npgsql;
using System.Data;


[TestClass]
public class ProductIntegrationTests
{
    private void ProductsTable()
    {
        using var connection = new NpgsqlConnection(
           "Host=localhost;Port=9999;Username=testuser;Password=mysecretpassword;Database=testdb");

        connection.Open();

        // 1. Rensa tabellen
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "DELETE FROM products";
            cmd.ExecuteNonQuery();
        }

        // 2. Lägg in testdata
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
            INSERT INTO products(name, category, price) VALUES
            ('iPhone 17 pro', 'Tech', '13000'),
            ('Pizza', 'Food', '120'),
            ('Lipstick', 'Beauty', '150')
        ";
            cmd.ExecuteNonQuery();
        }

        connection.Close();
    }



    [TestMethod]
    [TestCategory("Integration")]
    public void GetProductsByCategory_RealDb()
    {
        ProductsTable();

        var repo = new ProductRepositoryWithInterface();

        var result = repo.GetProductsByCategory("Tech");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Tech", result[0].Category);
    }



    [TestMethod]
    [TestCategory("UnitTest")]
    public void GetProductsByCategory_WithMock()
    {
        //  Arrange 

        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        var readCallCount = 0;

        // Simulate one row
        mockReader.SetupSequence(r => r.Read())
            .Returns(true)
            .Returns(true)
            .Returns(false);
        mockReader.SetupSequence(r => r.GetString(0))
        .Returns("iPhone")
        .Returns("Pizza");

        mockReader.SetupSequence(r => r.GetString(1))
            .Returns("Tech")
            .Returns("Food");

        mockReader.SetupSequence(r => r.GetString(2))
            .Returns("13000")
            .Returns("120");

        // Important setups
        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockCommand.Setup(c => c.CreateParameter()).Returns(Mock.Of<IDbDataParameter>());
        mockCommand.Setup(c => c.Parameters).Returns(Mock.Of<IDataParameterCollection>());

        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
        mockConnection.Setup(c => c.Open());   
        mockConnection.Setup(c => c.Close());  

        var repo = new ProductRepositoryWithInterface(mockConnection.Object);

        //Act
        var result = repo.GetProductsByCategory("Tech");

        //Assert 
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Tech", result[0].Category);
    }
}
