namespace ShoppingCartAppIntegration.Tests;

using System.Text;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

[TestClass]
public class User
{
    // Local Host and URL
    private readonly string appUrl = "http://localhost:8000";
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
        var response = await _httpClient.PostAsync($"{appUrl}/signup", new StringContent(
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
        var responseLogin = await _httpClient.PostAsync($"{appUrl}/login", new StringContent(
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
        var responseSignUp = await _httpClient.PostAsync($"{appUrl}/signup",
            new StringContent(JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
            ));

            // LOGIN, Customer must login.
        var responseLogin = await _httpClient.PostAsync($"{appUrl}/login",
            new StringContent(JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
            ));

        var loginJson = JsonDocument.Parse(await responseLogin.Content.ReadAsStringAsync());

        // Act
            // Get customer's cart.
        var responseUserCart = await _httpClient.GetAsync($"{appUrl}/user");

        var cartJson = JsonDocument.Parse(await responseUserCart.Content.ReadAsStringAsync()).RootElement;
        var products = cartJson.GetProperty("products").EnumerateArray();

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, responseSignUp.StatusCode);
        Assert.AreEqual(username, loginJson.GetProperty("username").GetString());
        Assert.AreEqual(HttpStatusCode.OK, responseUserCart.StatusCode);
        Assert.IsNotNull(products);
    }
}
