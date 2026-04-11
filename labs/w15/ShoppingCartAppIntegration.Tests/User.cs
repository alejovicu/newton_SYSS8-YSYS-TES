namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Net.Http.Json;

[TestClass]
public class User
{
    private static readonly HttpClient client = new HttpClient();



    [TestMethod]
    public void RegisterNewCustomer()
    {
        // Generate a random username
        var username = $"customer_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        var password = "1234";
        var signupUrl = $"{GlobalContext.appUrl}/signup";
        var loginUrl = $"{GlobalContext.appUrl}/login";

        // Register new customer

        var signupResponse = client.PostAsJsonAsync(signupUrl, new { username, password }).Result;
        Assert.IsTrue(signupResponse.IsSuccessStatusCode, $"Signup failed: {signupResponse.StatusCode}");

        // Check response username (handle as JsonElement)
        var signupContent = signupResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>().Result;
        var returnedUsername = signupContent.TryGetProperty("username", out var unameProp) ? unameProp.GetString() : null;
        Assert.AreEqual(username, returnedUsername, "Username in response does not match");

        // Login as new customer
        var loginResponse = client.PostAsJsonAsync(loginUrl, new { username, password }).Result;
        Assert.IsTrue(loginResponse.IsSuccessStatusCode, $"Login failed: {loginResponse.StatusCode}");
    }

    [TestMethod]
    public void CustomerListsProductsInCart()
    {
        // Generate a random username
        var username = $"customer_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        var password = "1234";
        var signupUrl = $"{GlobalContext.appUrl}/signup";
        var loginUrl = $"{GlobalContext.appUrl}/login";

        // Register new customer
        var signupResponse = client.PostAsJsonAsync(signupUrl, new { username, password }).Result;
        Assert.IsTrue(signupResponse.IsSuccessStatusCode, $"Signup failed: {signupResponse.StatusCode}");

        // Login as new customer
        var loginResponse = client.PostAsJsonAsync(loginUrl, new { username, password }).Result;
        Assert.IsTrue(loginResponse.IsSuccessStatusCode, $"Login failed: {loginResponse.StatusCode}");
        var loginContent = loginResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>().Result;
        string accessToken = loginContent.TryGetProperty("access_token", out var tokenProp) ? tokenProp.GetString() : null;
        Assert.IsFalse(string.IsNullOrEmpty(accessToken), "Access token not found in login response");

        // List products in cart
        var userUrl = $"{GlobalContext.appUrl}/user";
        var request = new HttpRequestMessage(HttpMethod.Get, userUrl);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var userResponse = client.SendAsync(request).Result;
        Assert.IsTrue(userResponse.IsSuccessStatusCode, $"User cart request failed: {userResponse.StatusCode}");
        var userContent = userResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>().Result;
        // Check that products is an array and is empty (as in Postman test)
        var products = userContent.TryGetProperty("products", out var productsProp) ? productsProp : default;
        Assert.IsTrue(products.ValueKind == System.Text.Json.JsonValueKind.Array, "Products property is not an array");
        Assert.AreEqual(0, products.GetArrayLength(), "Products array is not empty for new user");
    }
}
