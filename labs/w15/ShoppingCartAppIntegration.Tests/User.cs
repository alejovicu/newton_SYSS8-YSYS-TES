namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

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
    public async Task RegisterNewCustomer()
    {
        // Arrange
        // Create random username and a password for the new customer
        var username = $"customer_{Guid.NewGuid().ToString("N")[..8]}";
        var credentials = new { username, password = "1234" };

        // Act
        // Call signup endpoint
        var signupResponse = await client.PostAsync($"{GlobalContext.appUrl}/signup", new StringContent(
            JsonSerializer.Serialize(credentials),
            Encoding.UTF8,
            "application/json"
        ));
        var signupBody = JsonDocument.Parse(await signupResponse.Content.ReadAsStringAsync()).RootElement;

        // Call login endpoint with the created credentials
        var loginResponse = await client.PostAsync($"{GlobalContext.appUrl}/login", new StringContent(
            JsonSerializer.Serialize(credentials),
            Encoding.UTF8,
            "application/json"
        ));
        var loginBody = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync()).RootElement;

        // Assert
        // Check that the signup was successful and returned the correct username
        Assert.AreEqual(HttpStatusCode.OK, signupResponse.StatusCode);
        Assert.AreEqual(username, signupBody.GetProperty("username").GetString());

        // Check that the login was successful and returned an access token
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.IsFalse(string.IsNullOrEmpty(loginBody.GetProperty("access_token").GetString()));
    }

    /*
    Given: I am a registered customer
    When: I list the products in my cart
    Then: I should see an empty cart
    */
    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        // Arrange
        // Create and register a new customer
        var username = $"customer_{Guid.NewGuid().ToString("N")[..8]}";
        var credentials = new { username, password = "1234" };
        await client.PostAsync($"{GlobalContext.appUrl}/signup", new StringContent(
            JsonSerializer.Serialize(credentials),
            Encoding.UTF8,
            "application/json"
        ));

        // Log in to get an access token
        var loginResponse = await client.PostAsync($"{GlobalContext.appUrl}/login", new StringContent(
            JsonSerializer.Serialize(credentials),
            Encoding.UTF8,
            "application/json"
        ));
        var loginBody = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync()).RootElement;
        var token = loginBody.GetProperty("access_token").GetString();

        // Act
        // Call user endpoint to list products in cart
        var request = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/user");
        request.Headers.Add("Authorization", $"bearer {token}");
        var userResponse = await client.SendAsync(request);
        var userBody = JsonDocument.Parse(await userResponse.Content.ReadAsStringAsync()).RootElement;

        // Assert
        // Check that login and user requests were successful
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, userResponse.StatusCode);

        // Check that the cart is empty for a new customer
        Assert.AreEqual(0, userBody.GetProperty("products").GetArrayLength());
    }
}
