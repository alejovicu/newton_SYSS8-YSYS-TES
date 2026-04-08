namespace ShoppingCartAppIntegration.Tests;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

[TestClass]
public class Product
{
    private static readonly HttpClient client = new HttpClient();
    
    private async Task<string> GetAdminToken()
    {
        // Arrange
        var body = new StringContent(
            JsonSerializer.Serialize(new { username = "admin", password = "admin" }),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await client.PostAsync($"{GlobalContext.appUrl}/login", body);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        // Assert
        Assert.AreEqual(200, (int)response.StatusCode);

        return json.GetProperty("access_token").GetString()!;
    }
    
    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {
        // Arrange
        var token = await GetAdminToken();
        var productName = "Test Product " + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        request.Headers.Add("Authorization", $"Bearer {token}");
        request.Content = new StringContent(
            JsonSerializer.Serialize(new { name = productName }),
            Encoding.UTF8, "application/json"
        );
        var response = await client.SendAsync(request);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        Assert.AreEqual(200, (int)response.StatusCode);
        Assert.IsTrue(json.TryGetProperty("id", out _));
        Assert.AreEqual(productName, json.GetProperty("name").GetString());
    }


    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange
        var token = await GetAdminToken();
        var productName = "Test Product " + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Act - Add product first
        var addRequest = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        addRequest.Headers.Add("Authorization", $"Bearer {token}");
        addRequest.Content = new StringContent(
            JsonSerializer.Serialize(new { name = productName }),
            Encoding.UTF8, "application/json"
        );
        var addResponse = await client.SendAsync(addRequest);
        var addJson = await addResponse.Content.ReadFromJsonAsync<JsonElement>();
        var productId = addJson.GetProperty("id").GetInt32();

        // Act - Delete product
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"{GlobalContext.appUrl}/product/{productId}");
        deleteRequest.Headers.Add("Authorization", $"Bearer {token}");
        var deleteResponse = await client.SendAsync(deleteRequest);

        // Assert
        Assert.AreEqual(200, (int)deleteResponse.StatusCode);
    }
}
