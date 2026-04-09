using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ShoppingCartAppIntegration.Tests;

[TestClass]
public class Product
{
    // Local Host and URL
    private readonly string _appUrl = "http://localhost:8000";
    private readonly HttpClient _httpClient = new HttpClient();

    private string randomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {
        // Arrange
        var username = "admin";
        var password = "admin";
        var productName = "product_" + randomString(8);

            // LOGIN, Only Admin can add.
        var responseLogin = await _httpClient.PostAsync($"{_appUrl}/login",
            new StringContent(JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
            ));

        var loginJson = JsonDocument.Parse(await responseLogin.Content.ReadAsStringAsync());
        var userId = loginJson.RootElement.GetProperty("id").GetInt32();

            // Create Product
        var responseProduct = await _httpClient.PostAsync($"{_appUrl}/product",
            new StringContent(JsonSerializer.Serialize(new { productName }),
            Encoding.UTF8,
            "application/json"
            ));

        var productJson = JsonDocument.Parse(await responseProduct.Content.ReadAsStringAsync());
        var productId = productJson.RootElement.GetProperty("id").GetInt32();

        // Act

            // Get product list.
        var responseCatalogue = await _httpClient.GetAsync($"{_appUrl}/products");

        var cartJson = JsonDocument.Parse(await responseCatalogue.Content.ReadAsStringAsync());
        var products = cartJson.RootElement.GetProperty("products");

        // Assert
        Assert.AreEqual(HttpStatusCode.Ok, responseProduct.StatusCode);
        Assert.AreEqual(HttpStatusCode.Ok, responseCatalogue.StatusCode);
        Assert.IsTrue(products > 0);
    }


    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange
        var username = "admin";
        var password = "admin";
        var productName = "product_" + randomString(8);

        //Login, Admin must be logged into add/delete a product.
        var responseLogin = await _httpClient.PostAsync($"{_appUrl}/login",
            new StringContent(JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
            ));

        //Create Product, a product must exist for admin to delete it.
        var responseCreateProduct = await _httpClient.PostAsync($"{_appUrl}/product",
            new StringContent(JsonSerializer.Serialize(new { productName }),
            Encoding.UTF8,
            "application/json"
            ));

        var productJson = JsonDocument.Parse(await responseCreateProduct.Content.ReadAsStringAsync());
        var productId = productJson.RootElement.GetProperty("id").GetInt32();

        // Act
        var responseDeleteProduct = await _httpClient.DeleteAsync($"{_appUrl}/product/{productId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.Ok, responseDeleteProduct.StatusCode);
    }
}
