namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

[TestClass]
public class User
{
    // Local Host and URL
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
        // Arrange
        // Create username and password for the new customer
        var username = "customer_" + randomString(8);
        var password = "Password123!";

        // Act
        // Call signup endpoint
        var response = await _httpClient.PostAsync($"{_appUrl}/signup", new StringContent(
            JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
        ));


        // Assert
        // Check that the response is successful
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        // Check that the response contains the expected data (username)
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonDocument.Parse(responseContent).RootElement;
        Assert.AreEqual(username, jsonResponse.GetProperty("username").GetString());

        // Validate if the user can log in with the created credentials
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
        var username = "customer_" + randomString(8);
        var password = "Password123!";

            // SIGN UP, Customer must exist.
        var responseSignUp = await _httpClient.PostAsync($"{_appUrl}/signup",
            new StringContent(JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
            ));

            // LOGIN, Customer must login.
        var responseLogin = await _httpClient.PostAsync($"{_appUrl}/login",
            new StringContent(JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
            ));

        var loginJson = JsonDocument.Parse(await responseLogin.Content.ReadAsStringAsync());
        var userId = loginJson.RootElement.GetProperty("id").GetInt32();

        // Act
            // Get customer's cart.
        var responseUserCart = await _httpClient.GetAsync($"{_appUrl}/user");

        var cartJson = JsonDocument.Parse(await responseUserCart.Content.ReadAsStringAsync());
        var products = cartJson.RootElement.GetProperty("products");

        // Assert
        Assert.IsTrue(responseUserCart != null);
    }
}
