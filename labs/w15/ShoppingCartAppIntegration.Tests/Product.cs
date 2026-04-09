namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

[TestClass]
public class Product
{
    private static readonly HttpClient client = new HttpClient();

    private static string GenerateRandomProductName() =>
        "product_" + Guid.NewGuid().ToString("N")[..10];

    private async Task<string> LoginAsAdmin()
    {
        var loginResponse = await client.PostAsJsonAsync(
            $"{GlobalContext.appUrl}/login",
            new { username = "admin", password = "admin" }
        );
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        return loginBody.GetProperty("access_token").GetString()!;
    }

    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {
        var token = await LoginAsAdmin();
        var productName = GenerateRandomProductName();

        // Create product
        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        createRequest.Headers.Add("Authorization", $"bearer {token}");
        createRequest.Content = JsonContent.Create(new { name = productName });
        var createResponse = await client.SendAsync(createRequest);

        Assert.AreEqual(HttpStatusCode.OK, createResponse.StatusCode);
        var createBody = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.AreEqual(productName, createBody.GetProperty("name").GetString());
        var productId = createBody.GetProperty("id").GetInt32();

        // List products
        var listRequest = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/products");
        listRequest.Headers.Add("Authorization", $"bearer {token}");
        var listResponse = await client.SendAsync(listRequest);

        Assert.AreEqual(HttpStatusCode.OK, listResponse.StatusCode);
        var products = await listResponse.Content.ReadFromJsonAsync<JsonElement>();
        var found = products.EnumerateArray().Any(p => p.GetProperty("id").GetInt32() == productId);
        Assert.IsTrue(found, "Product should be available in the catalog");
    }

    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        var token = await LoginAsAdmin();
        var productName = GenerateRandomProductName();

        // Add product to delete
        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        createRequest.Headers.Add("Authorization", $"bearer {token}");
        createRequest.Content = JsonContent.Create(new { name = productName });
        var createResponse = await client.SendAsync(createRequest);
        var createBody = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var productId = createBody.GetProperty("id").GetInt32();

        // Delete product
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"{GlobalContext.appUrl}/product/{productId}");
        deleteRequest.Headers.Add("Authorization", $"bearer {token}");
        var deleteResponse = await client.SendAsync(deleteRequest);

        Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode);
        var deleteBody = await deleteResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.AreEqual(productId, deleteBody.GetProperty("id").GetInt32());
    }
}
