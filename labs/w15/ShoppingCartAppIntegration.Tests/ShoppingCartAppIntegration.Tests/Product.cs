using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static ShoppingCartAppIntegration.Tests.GlobalContext;

namespace ShoppingCartAppIntegration.Tests;

[TestClass]
public class Product
{
    private static readonly HttpClient client = new HttpClient();

    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {
        // Arrange
        string productName = $"Test Product {Guid.NewGuid():N}".Substring(0, 20);
        var jsonProduct = JsonSerializer.Serialize(new { name = productName });
        var contentProduct = new StringContent(jsonProduct, Encoding.UTF8, "application/json");

        var payload = new
        {
            username = "admin",
            password = "admin"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var tryLogin = await client.PostAsync($"{appUrl}/login", content);
        var loginBody = await tryLogin.Content.ReadAsStringAsync();

        Console.WriteLine($"Login response body: {loginBody}");
        using var doc = JsonDocument.Parse(loginBody);
        var root = doc.RootElement;

        if (!root.TryGetProperty("access_token", out var tokenElement))
        {
            Assert.Fail($"access_token not found in login response: {loginBody}");
        }

        var accessToken = tokenElement.GetString();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        // Act
        var response = await client.PostAsync($"{appUrl}/product", contentProduct);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Product creation failed");
        var listOfProductsResponse = await client.GetAsync($"{appUrl}/products");
        bool found = listOfProductsResponse.Content.ReadAsStringAsync().Result.Contains(productName);
        Assert.IsTrue(found, "Product does not exist in list of products");
    }


    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange
        string productName = $"Test Product {Guid.NewGuid():N}".Substring(0, 20);
        var jsonProduct = JsonSerializer.Serialize(new { name = productName });
        var contentProduct = new StringContent(jsonProduct, Encoding.UTF8, "application/json");

        var payload = new
        {
            username = "admin",
            password = "admin"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var tryLogin = await client.PostAsync($"{appUrl}/login", content);
        var loginBody = await tryLogin.Content.ReadAsStringAsync();

        Console.WriteLine($"Login response body: {loginBody}");
        using var doc = JsonDocument.Parse(loginBody);
        var root = doc.RootElement;

        if (!root.TryGetProperty("access_token", out var tokenElement))
        {
            Assert.Fail($"access_token not found in login response: {loginBody}");
        }

        var accessToken = tokenElement.GetString();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        // Act
        var createNewProduct = await client.PostAsync($"{appUrl}/product", contentProduct);
        var createdProductBody = await createNewProduct.Content.ReadAsStringAsync();
        var productId = JsonDocument.Parse(createdProductBody).RootElement.GetProperty("id").GetInt32();

        // Act
        var response = await client.DeleteAsync($"{appUrl}/product/{productId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Product deletion failed");
        var listOfProductsResponse = await client.GetAsync($"{appUrl}/products");
        bool found = listOfProductsResponse.Content.ReadAsStringAsync().Result.Contains(productName);
        Assert.IsFalse(found, "Product still exists in list of products");
    }
}
