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
        

 var newCustomer = $"customer_{Guid.NewGuid():N}";

        var signupBody = new
        {
            username = newCustomer,
            password = "1234"
        };

        var response = await client.PostAsJsonAsync(
            $"{GlobalContext.appUrl}/signup",
            signupBody
        );

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);

        var returnedUsername = json.RootElement.GetProperty("username").GetString();

        Assert.AreEqual(newCustomer, returnedUsername);
    }

    [TestMethod]
   public async Task CustomerListsProductsInCart()
    {
        //Implement tests
        //Assert.IsTrue(false, "Test not implemented yet.");
var newCustomer = $"customer_{Guid.NewGuid():N}";
        var password = "1234";

        // 1. Skapa kund
        var signupBody = new
        {
            username = newCustomer,
            password = password
        };

        var signupResponse = await client.PostAsJsonAsync(
            $"{GlobalContext.appUrl}/signup",
            signupBody
        );

        Assert.AreEqual(HttpStatusCode.OK, signupResponse.StatusCode);

        // 2. Logga in kunden
        var loginBody = new
        {
            username = newCustomer,
            password = password
        };

        var loginResponse = await client.PostAsJsonAsync(
            $"{GlobalContext.appUrl}/login",
            loginBody
        );

        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginContent);
        var accessToken = loginJson.RootElement.GetProperty("access_token").GetString();

        Assert.IsFalse(string.IsNullOrWhiteSpace(accessToken));

        // 3. Hämta user/cart info
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{GlobalContext.appUrl}/user"
        );

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var userResponse = await client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.OK, userResponse.StatusCode);

        var userContent = await userResponse.Content.ReadAsStringAsync();
        var userJson = JsonDocument.Parse(userContent);

        var products = userJson.RootElement.GetProperty("products");

        Assert.AreEqual(0, products.GetArrayLength());
    }
    
}
