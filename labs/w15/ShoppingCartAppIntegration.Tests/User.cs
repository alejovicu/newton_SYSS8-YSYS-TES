namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

[TestClass]
public class User
{
    private static readonly HttpClient client = new HttpClient();

    [TestMethod]
    public async Task RegisterNewCustomer()
    {
        // Arrange
        var username = "customer_" + Guid.NewGuid().ToString("N");
        var password = "1234";
        var body = new { username, password };
        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync($"{GlobalContext.appUrl}/signup", content);
        var json = await response.Content.ReadAsStringAsync();
        var data = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(username, data.GetProperty("username").GetString());
    }

    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        // Arrange
        var username = "customer_" + Guid.NewGuid().ToString("N");
        var password = "1234";
        var credentials = new { username, password };
        var signupContent = new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, "application/json");
        await client.PostAsync($"{GlobalContext.appUrl}/signup", signupContent);

        var loginContent = new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, "application/json");
        var loginResponse = await client.PostAsync($"{GlobalContext.appUrl}/login", loginContent);
        var loginJson = await loginResponse.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(loginJson).RootElement.GetProperty("access_token").GetString();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/user");
        request.Headers.Add("Authorization", $"bearer {token}");
        var userResponse = await client.SendAsync(request);
        var userJson = await userResponse.Content.ReadAsStringAsync();
        var userData = JsonDocument.Parse(userJson).RootElement;

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, userResponse.StatusCode);
        Assert.AreEqual(0, userData.GetProperty("products").GetArrayLength());
    }
}