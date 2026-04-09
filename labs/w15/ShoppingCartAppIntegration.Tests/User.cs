using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace ShoppingCartAppIntegration.Tests;

[TestClass]
public class User
{
    private static readonly HttpClient client = new HttpClient();


    private static string RandomUsername() => "customer_" + Guid.NewGuid().ToString("N")[..20];

    [TestMethod]
    public async Task RegisterNewCustomer()
    {
        // Arrange
        var username = RandomUsername();
        var password = "Test123!";

        var signupBody = new
        {
            username = username,
            password = password
        };

        // Act
        var response = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/signup", signupBody);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.AreEqual(username, responseJson.GetProperty("username").GetString());
    }


    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        // Arrange
        var username = RandomUsername();
        var password = "Test123!";

        var signupBody = new
        {
            username,
            password
        };

        var signupResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/signup", signupBody);
        Assert.AreEqual(HttpStatusCode.OK, signupResponse.StatusCode);

        var loginBody = new
        {
            username,
            password
        };

        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", loginBody);
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginJson.GetProperty("access_token").GetString();

        var request = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/user");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.IsFalse(string.IsNullOrWhiteSpace(responseBody), "Response can't be empty.");

        using var json = JsonDocument.Parse(responseBody);
        Assert.IsTrue(json.RootElement.TryGetProperty("products", out _), "Response should contain products.");

    }
}