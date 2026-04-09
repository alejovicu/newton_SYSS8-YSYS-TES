namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;

[TestClass]
public class User
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

        // Create a user
        var username = "customer_" + randomString(8);
        var password = "Password123!";

        // Call signup endpoint
        var response = await _httpClient.PostAsync($"{_appUrl}/signup", new StringContent(
            JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
        ));

        // Login as user
        var loginResponse = await _httpClient.PostAsync($"{_appUrl}/login", new StringContent(
        JsonSerializer.Serialize(new { username = username, password = password }),
        Encoding.UTF8,
        "application/json"
        ));

        // Extract access token from login response
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginContent).RootElement;
        var token = loginJson.GetProperty("access_token").GetString();

        // Act

        // GET /user with token attached to this request
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_appUrl}/user");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var userResponse = await _httpClient.SendAsync(request);


        //Assert

        // Check that login was successful
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        // Check the response from the /user endpoint 
        Assert.AreEqual(HttpStatusCode.OK, userResponse.StatusCode);

        var userContent = await userResponse.Content.ReadAsStringAsync();
        var userJson = JsonDocument.Parse(userContent).RootElement;
        
        // Check that the product array is correct
        Assert.AreEqual(JsonValueKind.Array, userJson.GetProperty("products").ValueKind);
        Assert.AreEqual(0, userJson.GetProperty("products").GetArrayLength());

    }
}
