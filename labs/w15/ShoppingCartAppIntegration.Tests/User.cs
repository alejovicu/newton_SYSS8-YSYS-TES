namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

[TestClass]
public class User
{
    private static readonly HttpClient client = new HttpClient();

    private static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }

    [TestMethod]
    public async Task RegisterNewCustomer()
    {
        // Hint: Use appUrl from GlobalContext to make API calls to the application
        // GlobalContext.appUrl
        var username = "customer_" + GenerateRandomString(8);
        var password = "1234";

        var response = await client.PostAsJsonAsync(
            $"{GlobalContext.appUrl}/signup",
            new { username, password }
        );

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.AreEqual(username, body.GetProperty("username").GetString());
    }

    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        var username = "customer_" + GenerateRandomString(8);
        var password = "1234";

        await client.PostAsJsonAsync(
            $"{GlobalContext.appUrl}/signup",
            new { username, password }
        );

        var loginResponse = await client.PostAsJsonAsync(
            $"{GlobalContext.appUrl}/login",
            new { username, password }
        );
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginBody.GetProperty("access_token").GetString();

        var request = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/user");
        request.Headers.Add("Authorization", $"bearer {token}");
        var userResponse = await client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.OK, userResponse.StatusCode);

        var userBody = await userResponse.Content.ReadFromJsonAsync<JsonElement>();
        var products = userBody.GetProperty("products").EnumerateArray().ToList();
        Assert.AreEqual(0, products.Count);
    }
}
