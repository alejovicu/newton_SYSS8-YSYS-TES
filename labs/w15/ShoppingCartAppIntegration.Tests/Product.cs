namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

[TestClass]
public class Product
{
    private static readonly HttpClient client = new HttpClient();

    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {
        // Arrange
        var loginBody = new
        {
            username = "admin",
            password = "admin"
        };

        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", loginBody);

        // Assert login
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginJson.GetProperty("access_token").GetString();

        Assert.IsFalse(string.IsNullOrWhiteSpace(token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var productName = $"Apple_{Guid.NewGuid():N}"[..12];

        var createBody = new
        {
            name = productName
        };

        // Act
        var createResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/product", createBody);

        // Assert
        Assert.IsTrue(
            createResponse.StatusCode == HttpStatusCode.OK ||
            createResponse.StatusCode == HttpStatusCode.Created);

        var productJson = await createResponse.Content.ReadFromJsonAsync<JsonElement>();

        Assert.AreEqual(productName, productJson.GetProperty("name").GetString());
        Assert.IsTrue(productJson.GetProperty("id").GetInt32() > 0);
    }

    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange - login as admin
        var loginBody = new
        {
            username = "admin",
            password = "admin"
        };

        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", loginBody);
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginJson.GetProperty("access_token").GetString();

        Assert.IsFalse(string.IsNullOrWhiteSpace(token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Create a product first so we have something to delete
        var productName = $"Apple_{Guid.NewGuid():N}"[..12];

        var createBody = new
        {
            name = productName
        };

        var createResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/product", createBody);
        Assert.IsTrue(
            createResponse.StatusCode == HttpStatusCode.OK ||
            createResponse.StatusCode == HttpStatusCode.Created);

        var createdProduct = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var productId = createdProduct.GetProperty("id").GetInt32();

        // Act
        var deleteResponse = await client.DeleteAsync($"{GlobalContext.appUrl}/product/{productId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deletedProduct = await deleteResponse.Content.ReadFromJsonAsync<JsonElement>();

        Assert.AreEqual(productId, deletedProduct.GetProperty("id").GetInt32());
        Assert.AreEqual(productName, deletedProduct.GetProperty("name").GetString());
    }
}