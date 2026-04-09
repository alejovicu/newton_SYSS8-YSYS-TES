using Npgsql;

namespace ProductManager.Tests;

[TestClass]
public class IntegrationTestProductRepository
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
    public void GetProductsByCategory_ReturnsProductsFromDatabase()
    {
        CleanUpProductsTable();

        // arrange
        var repo = new ProductRepository();
        var product1 = new Product { Name = "iPhone 17 Pro", Category = "Tech", Price = "13000" };
        var product2 = new Product { Name = "Banana", Category = "Food", Price = "10" };
        repo.InsertProduct(product1);
        repo.InsertProduct(product2);

        // act
        var result = repo.GetProductsByCategory("Tech");

        // assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Tech", result[0].Category);
    }
}
