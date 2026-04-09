namespace ShoppingCartAppIntegration.Tests;

using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

[TestClass]
public class User
{
    private static readonly HttpClient client = new HttpClient();


    [TestMethod]
    public async Task RegisterNewCustomer()
    {
        // Arrange
        var username = "customer_" + Guid.NewGuid().ToString("N");
        var signupBody = new StringContent(
            JsonSerializer.Serialize(new { username = username, password = "1234" }),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await client.PostAsync($"{GlobalContext.appUrl}/signup", signupBody);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        Assert.AreEqual(200, (int)response.StatusCode);
        Assert.AreEqual(username, json.GetProperty("username").GetString());
    }

    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        // Arrange
        var username = "customer_" + Guid.NewGuid().ToString("N");

        var signupBody = new StringContent(
            JsonSerializer.Serialize(new { username = username, password = "1234" }),
            Encoding.UTF8,
            "application/json"
        );
        await client.PostAsync($"{GlobalContext.appUrl}/signup", signupBody);

        var loginBody = new StringContent(
            JsonSerializer.Serialize(new { username = username, password = "1234" }),
            Encoding.UTF8,
            "application/json"
        );


        // Act
        var loginResponse = await client.PostAsync($"{GlobalContext.appUrl}/login", loginBody);
        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginJson.GetProperty("access_token").GetString();

        var request = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/user");
        request.Headers.Add("Authorization", $"Bearer {token}");
        var cartResponse = await client.SendAsync(request);
        var cartJson = await cartResponse.Content.ReadFromJsonAsync<JsonElement>();


        // Assert
        Assert.AreEqual(200, (int)cartResponse.StatusCode);
        Assert.AreEqual(0, cartJson.GetProperty("products").GetArrayLength());
    }
}
