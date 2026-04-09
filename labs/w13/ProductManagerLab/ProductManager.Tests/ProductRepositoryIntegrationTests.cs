using Npgsql;
using ProductManager;

namespace ProductManager.Tests;

[TestClass]
public class ProductRepositoryIntegrationTests
{
    private const string ConnectionString = 
        "Host=localhost;Port=5433;Username=postgres;Password=postgres;Database=productsdb;SSL Mode=Disable";

    [TestInitialize]
    public void SetUp()
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS products (
                id Serial PRIMARY KEY,
                name TEXT Not NULL UNIQUE,
                category TEXT NOT NULL CHECK (category IN ('Tech', 'Food', 'Beauty', 'Health')),
                price TEXT NOT NULL
            );

            INSERT INTO products (name, category, price)
            VALUES
                ('iPhone 17 Pro', 'Tech', '13000'),
                ('Greek Yogurt', 'Food', '45'),
                ('Vitamin C Serum', 'Beauty', '299'),
                ('Omega 3 Capsules', 'Health', '159')
            ON CONFLICT (name) DO NOTHING;
        ";
        cmd.ExecuteNonQuery();
    }
    
    [TestMethod]
    [TestCategory("Integration")]
    public void GetProductsByCategory_ReturnsOnlyHealthProducts()
    {
        var repository = new ProductRepository();

        var result = repository.GetProductsByCategory("Health");

        Assert.IsTrue(result.Count > 0);
        Assert.IsTrue(result.All(p => p.Category == "Health"));
        Assert.IsTrue(result.Any(p => p.Name == "Omega 3 Capsules"));
    }
}