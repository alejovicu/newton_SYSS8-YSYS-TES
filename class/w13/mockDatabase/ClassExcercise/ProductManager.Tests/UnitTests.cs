using System.Data;
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
        // Skapar mock-objekt för att simulera databasen
        var mockConnection = new Mock<IDbConnection>(); // Fake databas anslutning 
        var mockCommand = new Mock<IDbCommand>(); // SQL Kommand
        var mockReader = new Mock<IDataReader>(); // resultat av SQL

        // Variabel som används för att simulera vilken rad vi är på i "databasen"
        int rowIndex = 0;

        // Simulerar reader.Read()
        // Returnerar true 3 gånger (3 rader), sedan false (slut på data)
        mockReader.Setup(r => r.Read()).Returns(() =>
        {
            rowIndex++;
            return rowIndex <= 4;
        });

        // Simulerar att första kolumnen Id returneras
        mockReader.Setup(r => r.GetInt32(0)).Returns(() => rowIndex);

        // Simulerar kolumn 1 Name
        mockReader.Setup(r => r.GetString(1)).Returns(() =>
        {
            return rowIndex switch
            {
                1 => "Iphone 17 Pro",
                2 => "Organic Apple",
                3 => "Hand Cream",
                4 => "Gaming Laptop",
                _ => ""
            };
        });

        // Simulerar kolumn 2 Category
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

        // Simulerar kolumn 3 Price
        mockReader.Setup(r => r.GetString(3)).Returns(() =>
        {
            return rowIndex switch
            {
                1 => "13000",
                2 => "55",
                3 => "150",
                4 => "18500",
                _ => ""
            };
        });

        // När ExecuteReader() anropas ska den returnera vår fake reader
        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);

        // När CreateCommand() anropas ska den returnera vårt fake-command
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        // Simulerar att anslutningen kan öppnas och stängas utan riktig databas
        mockConnection.Setup(c => c.Open());
        mockConnection.Setup(c => c.Close());

        // Skapar repository med mockad connection istället för riktig databas
        var repository = new PostgresProductRepository(mockConnection.Object);

        // Act
        var result = repository.GetProductsByCategory("Tech");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(p => p.Category == "Tech"));
        
        Assert.AreEqual("Iphone 17 Pro", result[0].Name);
        Assert.AreEqual("Gaming Laptop", result[1].Name);
    }
}