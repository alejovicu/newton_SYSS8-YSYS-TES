namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Net.Http.Json;

[TestClass]
public class User
{
    private static readonly HttpClient client = new HttpClient();


    [TestMethod]
    public async Task RegisterNewCustomer()
    {
       // Arrange
       var newCustomer = "customer_" + Guid.NewGuid().ToString("N")[..20];

       var requestBody = new
       {
           username = newCustomer,
           password = "1234"
       };
       
       // Act
       var response = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/signup", requestBody);
       
       // Assert
       Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

       var json = await response.Content.ReadFromJsonAsync<SignupResponse>();
       Assert.IsNotNull(json);
       Assert.AreEqual(newCustomer, json.username);
    }

    private class SignupResponse
    {
        public string? username { get; set; }
    }

    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        // Arrange
        var newCustomer = "customer_" + Guid.NewGuid().ToString("N")[..20];

        var signupBody = new
        {
            username = newCustomer,
            password = "1234"
        };

        await client.PostAsJsonAsync($"{GlobalContext.appUrl}/signup", signupBody);

        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", signupBody);
        Assert.AreEqual(System.Net.HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.IsNotNull(loginJson);
        Assert.IsFalse(string.IsNullOrEmpty(loginJson.access_token));

        var request = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/user");
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginJson.access_token);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

        var userJson = await response.Content.ReadFromJsonAsync<UserResponse>();
        Assert.IsNotNull(userJson);
        Assert.IsNotNull(userJson.products);
        Assert.AreEqual(0, userJson.products.Count);
    }

    private class LoginResponse
    {
        public string? access_token { get; set; }
    }

    private class UserResponse
    {
        public List<object>? products { get; set; }
    }
}
