namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Text.Json;
using System.Net;
using System.Text;

[TestClass]
public sealed class ShoppingAppTests
{

    private readonly string _appUrl = GlobalContext.appUrl;
    private readonly HttpClient _httpClient = new HttpClient();

    private string randomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /*
    Given I am a new potential customer
    When I sign in in the app
    Then I should be able to log in as an application customer
    */
    [TestMethod]
    public async Task TestCreateNewCustomer()
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

    /*
    Given I am a customer​
    When I log in into the application​
    Then I should see all the products on my cart
    */
    [TestMethod]
    public async Task TestCustomerListProductsInCart()
    {
        // Arrange (Given I am a customer​)

        // Create username and password for the customer
        var username = "customer_" + randomString(8);
        var password = "Password123!";

        // Sign up customer so there is a valid user to login
        var signupResponse = await _httpClient.PostAsync($"{_appUrl}/signup", new StringContent(
            JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
        ));

        // Act (When I log in into the application​)

        // Login as customer to get an access token
        var loginResponse = await _httpClient.PostAsync($"{_appUrl}/login", new StringContent(
            JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
        ));

        // Extract access token from login response
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginContent).RootElement;
        var accessToken = loginJson.GetProperty("access_token").GetString();

        // Call user endpoint with access token in Authorization header
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_appUrl}/user");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
        var responseListProducts = await _httpClient.SendAsync(request);

        // Assert (Then I should see all the products on my cart)

        // Check if response is successful
        Assert.AreEqual(HttpStatusCode.OK, signupResponse.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, responseListProducts.StatusCode);

        // Check that product list in the cart is empty for new customer
        var responseContent = await responseListProducts.Content.ReadAsStringAsync();
        var jsonResponse = JsonDocument.Parse(responseContent).RootElement;
        Assert.AreEqual(0, jsonResponse.GetProperty("products").GetArrayLength());
    }
}