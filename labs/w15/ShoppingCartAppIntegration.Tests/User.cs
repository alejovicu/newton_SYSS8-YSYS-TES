namespace ShoppingCartAppIntegration.Tests;

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
    public void RegisterNewCustomer()
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
        var productName = "product_" + randomString(8);

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
        var userId = loginJson.RootElement.GetProperty("id").GetInt32();

            // Create Product, Product must exist.
        var responseProduct = await _httpClient.PostAsync($"{appUrl}/product",
            new StringContent(JsonSerializer.Serialize(new { productName }),
            Encoding.UTF8,
            "application/json"
            ));

        var productJson = JsonDocument.Parse(await responseProduct.Content.ReadAsStringAsync());
        var productId = productJson.RootElement.GetProperty("id").GetInt32();

        // Act
            // Add Product to cart.
        var responseAddCart = await _httpClient.PostAsync(
            $"{appUrl}/user/product/{productId}");

            // Get customer's cart.
        var responseUserCart = await _httpClient.GetAsync($"{appUrl}/products");

        var cartJson = JsonDocument.Parse(await responseUserCart.Content.ReadAsStringAsync());
        var products = cartJson.RootElement.GetProperty("products");

        // Assert
        Assert.AreEqual(HttpStatusCode.Ok, responseProduct.StatusCode);
        Assert.AreEqual(HttpStatusCode.Ok, responseAddCart.StatusCode);
        Assert.IsTrue(products.Count > 0);
    }
}
