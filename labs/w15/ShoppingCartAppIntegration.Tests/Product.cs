namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

[TestClass]
public class Product
{
    private static readonly HttpClient client = new HttpClient();

    private async Task<string> LoginAsAdmin()
    {
        var body = new { username = "ph", password = "123" };
        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{GlobalContext.appUrl}/login", content);
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement.GetProperty("access_token").GetString();
    }

    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {
        // Arrange
        var token = await LoginAsAdmin();
        var productName = "product_" + Guid.NewGuid().ToString("N")[..8];
        var request = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        request.Headers.Add("Authorization", $"bearer {token}");
        request.Content = new StringContent(JsonSerializer.Serialize(new { name = productName, price = 9.99 }), Encoding.UTF8, "application/json");

        // Act
        var response = await client.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        var data = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(productName, data.GetProperty("name").GetString());
    }

    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange
        var token = await LoginAsAdmin();
        var productName = "product_" + Guid.NewGuid().ToString("N")[..8];
        var addRequest = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        addRequest.Headers.Add("Authorization", $"bearer {token}");
        addRequest.Content = new StringContent(JsonSerializer.Serialize(new { name = productName, price = 5.00 }), Encoding.UTF8, "application/json");
        var addResponse = await client.SendAsync(addRequest);
        var productId = JsonDocument.Parse(await addResponse.Content.ReadAsStringAsync()).RootElement.GetProperty("id").GetInt32();

        // Act
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"{GlobalContext.appUrl}/product/{productId}");
        deleteRequest.Headers.Add("Authorization", $"bearer {token}");
        var deleteResponse = await client.SendAsync(deleteRequest);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, addResponse.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode);
    }
}