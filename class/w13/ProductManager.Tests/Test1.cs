namespace ProductManager.Tests;

[TestClass]
public sealed class ProductManagerIntegrationTests
{
    private const string ConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=password123;Database=postgres";
    
    [TestMethod]
    [TestCategory("Integration")]
    public void GetProductsByCategory_TechCategory_ReturnsProducts()
    {
        // Arrange
        IDatabaseContext context = new DatabaseContext(ConnectionString);
        var productManager = new ProductManager(context);

        // Act
        var products = productManager.GetProductsByCategory("Tech");

        // Assert
        Assert.IsNotNull(products);
        Assert.IsNotEmpty(products);
        Assert.AreEqual("Tech", products[0].Category);
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void GetProductsByCategory_FoodCategory_ReturnsProducts()
    {
        // Arrange
        IDatabaseContext context = new DatabaseContext(ConnectionString);
        var productManager = new ProductManager(context);

        // Act
        var products = productManager.GetProductsByCategory("Food");

        // Assert
        Assert.IsNotNull(products);
        Assert.IsNotEmpty(products);
        Assert.AreEqual("Food", products[0].Category);
    }
}