namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static GlobalContext;

[TestClass]
public class User
{
    private static readonly HttpClient client = new HttpClient();

    [TestMethod]
    public async Task RegisterNewCustomer()
    {
        // Hint: Use appUrl from GlobalContext to make API calls to the application
        // GlobalContext.appUrl

        // Arrange
        var payload = new
        {
            username = $"testuser_{Guid.NewGuid():N}".Substring(0, 20),
            password = "testpassword"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        
        // Act
        var response = await client.PostAsync($"{appUrl}/signup", content);

        Console.WriteLine($"Payload: {json}");
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response body: {responseBody}");
        Console.WriteLine($"Status code: {(int)response.StatusCode} ({response.StatusCode})");

        // Assert
        //Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Registration failed. Status code: {response.StatusCode}");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Registration failed");
        var tryLogin = await client.PostAsync($"{appUrl}/login", content);
        var loginBody = await tryLogin.Content.ReadAsStringAsync();

        Console.WriteLine($"Login body: {loginBody}");
        Console.WriteLine($"Login status: {(int)tryLogin.StatusCode}");
        Assert.AreEqual(HttpStatusCode.OK, tryLogin.StatusCode, "Login failed after registration");
    }

    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        // Arrange
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
        var response = await client.GetAsync($"{appUrl}/products");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Get products failed");
        Assert.IsNotEmpty(await response.Content.ReadAsStringAsync(), "Product list should not be empty");
    }
}
