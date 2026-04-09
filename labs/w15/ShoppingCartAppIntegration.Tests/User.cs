using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ShoppingCartAppIntegration.Tests;

[TestClass]
public class User
{
    private static readonly HttpClient client = new HttpClient();

    /*
    Given: I am a new potential customer
    When: I sign up and log in to the app
    Then: I should be registered and receive an access token
    */
    [TestMethod]
    [TestCategory("Integration")]
    public async Task RegisterNewCustomer()
    {
        // Arrange
        var username = $"customer_{Guid.NewGuid().ToString("N")[..8]}";
        var credentials = new { username, password = "1234" };

        // Act
        var signupResponse = await client.PostAsync($"{GlobalContext.appUrl}/signup", new StringContent(
            JsonSerializer.Serialize(credentials),
            Encoding.UTF8,
            "application/json"
        ));
        var signupBody = JsonDocument.Parse(await signupResponse.Content.ReadAsStringAsync()).RootElement;

        var loginResponse = await client.PostAsync($"{GlobalContext.appUrl}/login", new StringContent(
            JsonSerializer.Serialize(credentials),
            Encoding.UTF8,
            "application/json"
        ));
        var loginBody = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync()).RootElement;

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, signupResponse.StatusCode);
        Assert.AreEqual(username, signupBody.GetProperty("username").GetString());
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.IsFalse(string.IsNullOrEmpty(loginBody.GetProperty("access_token").GetString()));
    }

    /*
    Given: I am a registered customer
    When: I list the products in my cart
    Then: I should see an empty cart
    */
    [TestMethod]
    [TestCategory("Integration")]
    public async Task CustomerListsProductsInCart()
    {
        // Arrange
        var username = $"customer_{Guid.NewGuid().ToString("N")[..8]}";
        var credentials = new { username, password = "1234" };

        await client.PostAsync($"{GlobalContext.appUrl}/signup", new StringContent(
            JsonSerializer.Serialize(credentials),
            Encoding.UTF8,
            "application/json"
        ));

        var loginResponse = await client.PostAsync($"{GlobalContext.appUrl}/login", new StringContent(
            JsonSerializer.Serialize(credentials),
            Encoding.UTF8,
            "application/json"
        ));
        var loginBody = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync()).RootElement;
        var token = loginBody.GetProperty("access_token").GetString();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/user");
        request.Headers.Add("Authorization", $"bearer {token}");
        var userResponse = await client.SendAsync(request);
        var userBody = JsonDocument.Parse(await userResponse.Content.ReadAsStringAsync()).RootElement;

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, userResponse.StatusCode);
        Assert.AreEqual(0, userBody.GetProperty("products").GetArrayLength());
    }
}
