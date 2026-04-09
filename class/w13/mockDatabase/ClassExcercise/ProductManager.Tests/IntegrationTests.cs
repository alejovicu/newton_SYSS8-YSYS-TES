using System.Data;
using Npgsql;


namespace ProductManager.Tests;

// Markerar att detta är en testklass (MSTest)
[TestClass]
public class IntegrationTests
{
    // Connection string till ding PosgreSQL - Container
    private const string ConnectionString = 
        "Host=localhost;Port=5432;Username=testuser;Password=mysecretpassword;Database=testdb";
    
    // Körs före varje test -> används för att förbererda databasen
    [TestInitialize]
    public void Setup()
    {
        // Skapar en koppling till databasen
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        
        // SQL som:
        // 1, Skapar tabellen om den inte finns
        // 2, Rensar tabellen (Så testet alltid startar "rent")
        // 3, Lägger in testdata
        var sql = @"
            CREATE TABLE IF NOT EXISTS Product (
                Id SERIAL PRIMARY KEY,
                Name VARCHAR(250) NOT NULL,
                Category VARCHAR(50) NOT NULL CHECK (Category IN ('Tech', 'Food', 'Beauty', 'Health')),
                Price VARCHAR(50) NOT NULL
            );

            TRUNCATE TABLE Product RESTART IDENTITY;

            INSERT INTO Product(Name, Category, Price) VALUES
            ('Iphone 17 Pro', 'Tech', '13000'),
            ('Organic Apple', 'Food', '55'),
            ('Hand Cream', 'Beauty', '150');
        ";
        
        // Skickar SQL-Kommandot till databasen
        using var command = new NpgsqlCommand(sql, connection);
        command.ExecuteNonQuery(); // Kör SQL (Ingen Data returneras)
    }
    
    // Markerar detta som test
    [TestMethod]

    // Märker testet som "Integration" så vi kan filtrera det
    [TestCategory("Integration")]
    public void GetProductsByCategory_ShouldReturn_TechProducts()
    {
        // ARRANGE 
        // Skapar repository som pratar med riktig databas
        var productManager = new PostgresProductRepository();
        
        // ACT 
        // Kör metoden vi vill testa
        var filteredProducts = productManager.GetProductsByCategory("Tech");
        
        // ASSERT 
        // Kontrollera att resultatet inte är null
        Assert.IsNotNull(filteredProducts);

        // Kontrollera att vi fick exakt 1 produkt
        Assert.HasCount(1, filteredProducts);

        // Kontrollera att rätt produkt returneras
        Assert.AreEqual("Iphone 17 Pro", filteredProducts[0].Name);

        // Kontrollera att kategorin är korrekt
        Assert.AreEqual("Tech", filteredProducts[0].Category);
    }
}

