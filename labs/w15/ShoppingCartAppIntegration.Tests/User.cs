namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

[TestClass]
public class User
{
    private readonly HttpClient client = new HttpClient();

    [TestMethod]
    public async Task RegisterNewCustomer()
    {
        var username = $"customer_{Guid.NewGuid():N}";
        var password = "1234";

        var signupBody = new
        {
            username,
            password
        };

        var signupResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/signup", signupBody);

        Assert.AreEqual(HttpStatusCode.OK, signupResponse.StatusCode);

        var signupJson = await signupResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.AreEqual(username, signupJson.GetProperty("username").GetString());

        var loginBody = new
        {
            username,
            password
        };

        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", loginBody);

        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
    }

    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        var username = $"customer_{Guid.NewGuid():N}";
        var password = "1234";

        var signupBody = new
        {
            username,
            password
        };

        var signupResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/signup", signupBody);
        Assert.AreEqual(HttpStatusCode.OK, signupResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", signupBody);
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = loginJson.GetProperty("access_token").GetString();

        Assert.IsFalse(string.IsNullOrEmpty(accessToken));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var productsResponse = await client.GetAsync($"{GlobalContext.appUrl}/products");
        Assert.AreEqual(HttpStatusCode.OK, productsResponse.StatusCode);

        var userResponse = await client.GetAsync($"{GlobalContext.appUrl}/user");
        Assert.AreEqual(HttpStatusCode.OK, userResponse.StatusCode);

        var userJson = await userResponse.Content.ReadFromJsonAsync<JsonElement>();
        var productsInCart = userJson.GetProperty("products");

        Assert.AreEqual(0, productsInCart.GetArrayLength());
    }
}