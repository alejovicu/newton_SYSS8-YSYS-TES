namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Text.Json;
using System.Net;
using System.Text;

[TestClass]
public class User
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
    public async Task RegisterNewCustomer()
    {
        //Arrange 
        var username = "customer_" + randomString(10);
        var password = "Password123!";

        //Act
        var response = await _httpClient.PostAsync($"{_appUrl}/signup", new StringContent(
           JsonSerializer.Serialize(new { username, password }),
           Encoding.UTF8,
           "application/json"
       ));

        //Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonDocument.Parse(responseContent).RootElement;
        Assert.AreEqual(username, jsonResponse.GetProperty("username").GetString());

        var responseLogin = await _httpClient.PostAsync($"{_appUrl}/login", new StringContent(
            JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
        ));
        Assert.AreEqual(HttpStatusCode.OK, responseLogin.StatusCode);
    }

    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        // Arrange
        var username = "customer_" + randomString(10);
        var password = "Password123!";

        var responseCreate = await _httpClient.PostAsync($"{_appUrl}/signup", new StringContent(
            JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
        ));
        Assert.AreEqual(HttpStatusCode.OK, responseCreate.StatusCode);

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
        var response = await _httpClient.GetAsync($"{_appUrl}/user");
        
        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonDocument.Parse(responseContent).RootElement;
        Assert.AreEqual(username, jsonResponse.GetProperty("username").GetString());

        var products = jsonResponse.GetProperty("products");
        Assert.AreEqual(0, products.GetArrayLength());
    }
}
