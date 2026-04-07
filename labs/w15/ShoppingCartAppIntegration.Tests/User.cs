namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

[TestClass]
public class User
{
    private static readonly HttpClient client = new HttpClient();

    [TestMethod]
    public async Task RegisterNewCustomer()
    {
        // Arrange
        var newCustomer = $"customer_{Guid.NewGuid():N}".Substring(0, 20);

        var signupBody = new
        {
            username = newCustomer,
            password = "1234"
        };

        // Act
        var signupResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/signup", signupBody);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, signupResponse.StatusCode);

        var signupJson = await signupResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.AreEqual(newCustomer, signupJson.GetProperty("username").GetString());
    }

    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        // Arrange - create customer
        var newCustomer = $"customer_{Guid.NewGuid():N}".Substring(0, 20);

        var signupBody = new
        {
            username = newCustomer,
            password = "1234"
        };

        var signupResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/signup", signupBody);
        Assert.AreEqual(HttpStatusCode.OK, signupResponse.StatusCode);

        // Login
        var loginBody = new
        {
            username = newCustomer,
            password = "1234"
        };

        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", loginBody);
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginJson.GetProperty("access_token").GetString();

        Assert.IsFalse(string.IsNullOrWhiteSpace(token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var userResponse = await client.GetAsync($"{GlobalContext.appUrl}/user");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, userResponse.StatusCode);

        var userJson = await userResponse.Content.ReadFromJsonAsync<JsonElement>();
        var products = userJson.GetProperty("products");

        Assert.AreEqual(JsonValueKind.Array, products.ValueKind);
        Assert.AreEqual(0, products.GetArrayLength());
    }
}