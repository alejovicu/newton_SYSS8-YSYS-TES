namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Text.Json;
using System.Net;
using System.Text;

[TestClass]
public class Product
{
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
        var name = "product_" + randomString(10);

        var responseLogin = await _httpClient.PostAsync($"{_appUrl}/login", new StringContent(
            JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
        ));
        Assert.AreEqual(HttpStatusCode.OK, responseLogin.StatusCode);

        var loginContent = await responseLogin.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginContent).RootElement;
        var token = loginJson.GetProperty("access_token").GetString();

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var responseProduct = await _httpClient.PostAsync($"{_appUrl}/product", new StringContent(
            JsonSerializer.Serialize(new { name }),
            Encoding.UTF8,
            "application/json"
        ));
        Assert.AreEqual(HttpStatusCode.OK, responseProduct.StatusCode);

        // Assert (POST)
        var responseContent = await responseProduct.Content.ReadAsStringAsync();
        var jsonResponse = JsonDocument.Parse(responseContent).RootElement;
        Assert.AreEqual(name, jsonResponse.GetProperty("name").GetString());

        // Assert (GET)
        var response = await _httpClient.GetAsync($"{_appUrl}/products");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var productsContent = await response.Content.ReadAsStringAsync();
        var products = JsonDocument.Parse(productsContent).RootElement;
        var productExists = products.EnumerateArray().Any(p => p.GetProperty("name").GetString() == name);
        Assert.IsTrue(productExists);
    }

    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange
        var username = "admin";
        var password = "admin";
        var name = "product_" + randomString(10);

        var responseLogin = await _httpClient.PostAsync($"{_appUrl}/login", new StringContent(
            JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
        ));
        Assert.AreEqual(HttpStatusCode.OK, responseLogin.StatusCode);

        var loginContent = await responseLogin.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginContent).RootElement;
        var token = loginJson.GetProperty("access_token").GetString();

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var responseProduct = await _httpClient.PostAsync($"{_appUrl}/product", new StringContent(
            JsonSerializer.Serialize(new { name }),
            Encoding.UTF8,
            "application/json"
        ));
        Assert.AreEqual(HttpStatusCode.OK, responseProduct.StatusCode);

        // Assert (POST /product)
        var responseContent = await responseProduct.Content.ReadAsStringAsync();
        var jsonResponse = JsonDocument.Parse(responseContent).RootElement;
        Assert.AreEqual(name, jsonResponse.GetProperty("name").GetString());
        var productId = jsonResponse.GetProperty("id").GetInt32();

        // (DELETE /product/{id})
        var responseDelete = await _httpClient.DeleteAsync($"{_appUrl}/product/{productId}");
        Assert.AreEqual(HttpStatusCode.OK, responseDelete.StatusCode);

        // Assert (GET /products)
        var response = await _httpClient.GetAsync($"{_appUrl}/products");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var productsContent = await response.Content.ReadAsStringAsync();
        var products = JsonDocument.Parse(productsContent).RootElement;
        var productExists = products.EnumerateArray().Any(p => p.GetProperty("name").GetString() == name);
        Assert.IsFalse(productExists);
    }
}
