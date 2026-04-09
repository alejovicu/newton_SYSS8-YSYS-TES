namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Text.Json;
using System.Net;
using System.Text;

[TestClass]
public class User
{
    private static readonly HttpClient client = new HttpClient();
    private static string? token;
    private static string? username;

    private string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    [TestMethod]
    public async Task RegisterNewCustomer()
    {
        // Arrange
        username = "customer_" + RandomString(8);
        var password = "Password123!";

        // Act (Signup
        var response = await client.PostAsync($"{GlobalContext.appUrl}/signup", new StringContent(
            JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
        ));

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        Assert.AreEqual(username, json.GetProperty("username").GetString());

        // Act (Login
        var loginResponse = await client.PostAsync($"{GlobalContext.appUrl}/login", new StringContent(
            JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
        ));

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginJson = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync()).RootElement;
        token = loginJson.GetProperty("access_token").GetString();
        Assert.IsNotNull(token);
    }

    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        // Arrange
        var loginResponse = await client.PostAsync($"{GlobalContext.appUrl}/login", new StringContent(
            JsonSerializer.Serialize(new { username = "admin", password = "admin" }),
            Encoding.UTF8,
            "application/json"
        ));
        var loginJson = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync()).RootElement;
        var adminToken = loginJson.GetProperty("access_token").GetString();

        // Act (Get products
        var request = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/products");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
        var response = await client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        Assert.AreEqual(JsonValueKind.Array, json.ValueKind);
    }
}