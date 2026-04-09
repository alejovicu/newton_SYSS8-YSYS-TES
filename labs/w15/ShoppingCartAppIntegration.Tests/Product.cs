using System.Text;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ShoppingCartAppIntegration.Tests;

[TestClass]
public class Product
{
    // Local Host and URL
    private readonly string appUrl = "http://localhost:8000";
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
        var product = "product_" + randomString(8);

            // LOGIN, Only Admin can add.
        var responseLogin = await _httpClient.PostAsync($"{appUrl}/login",
            new StringContent(JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
            ));

        var loginJson = JsonDocument.Parse(await responseLogin.Content.ReadAsStringAsync());
        var adminToken = loginJson.RootElement.GetProperty("access_token").GetString();

        // Create Product
        var request = new HttpRequestMessage(HttpMethod.Post, $"{appUrl}/product");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
        request.Content = new StringContent(JsonSerializer.Serialize(new { product }), Encoding.UTF8, "application/json");

        var responseCreateProduct = await _httpClient.SendAsync(request);

        var productJson = JsonDocument.Parse(await responseCreateProduct.Content.ReadAsStringAsync());
        var productName = productJson.RootElement.GetProperty("name").GetString();

        // Act

            // Get product list.
        var responseCatalogue = await _httpClient.GetAsync($"{appUrl}/products");

        var cartJson = JsonDocument.Parse(await responseCatalogue.Content.ReadAsStringAsync());
        var products = cartJson.RootElement.GetProperty("products");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, responseLogin.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, responseCreateProduct.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, responseCatalogue.StatusCode);

        bool productExists = products.EnumerateArray()
            .Any(p => p.GetProperty("name").GetString() == productName);

        Assert.IsTrue(productExists);
    }


    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange
        var username = "admin";
        var password = "admin";
        var productName = "product_" + randomString(8);

        //Login, Admin must be logged into add/delete a product.
        var responseLogin = await _httpClient.PostAsync($"{appUrl}/login",
            new StringContent(JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
            ));

        var loginJson = JsonDocument.Parse(await responseLogin.Content.ReadAsStringAsync());
        var adminToken = loginJson.RootElement.GetProperty("access_token").GetString();

        //Create Product, a product must exist for admin to delete it.
        var request = new HttpRequestMessage(HttpMethod.Post, $"{appUrl}/product");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
        request.Content = new StringContent(JsonSerializer.Serialize(new { productName }), Encoding.UTF8, "application/json");

        var responseCreateProduct = await _httpClient.SendAsync(request);

        var productJson = JsonDocument.Parse(await responseCreateProduct.Content.ReadAsStringAsync());
        var productId = productJson.RootElement.GetProperty("id").GetInt32();

        // Act
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"{appUrl}/product/{productId}");
        deleteRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
        var responseDeleteProduct = await _httpClient.SendAsync(deleteRequest);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, responseLogin.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, responseCreateProduct.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, responseDeleteProduct.StatusCode);
    }
}
