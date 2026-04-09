namespace ProductManager.Tests;

[TestClass]
public class IntegrationTests
{
    private const string ConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres";
    
    [TestMethod]
    [TestCategory("Integration")]
    public void GetProductsByCategory_TechCategory_ReturnsOnlyTechProducts()
    {
        // Arrange
        var context = new DatabaseContext(ConnectionString);
        var productManager = new ProductManager(context);

        // Act
        var result = productManager.GetProductsByCategory("Tech");

        // Assert
        Assert.IsTrue(result.Count > 0);
        Assert.IsTrue(result.All(p => p.Category == "Tech"));
    }
}
