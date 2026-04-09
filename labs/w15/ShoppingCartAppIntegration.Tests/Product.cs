namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

[TestClass]
public class Product
{
    private readonly HttpClient client = new HttpClient();

    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {
        var adminLoginBody = new
        {
            username = "admin",
            password = "admin"
        };

        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", adminLoginBody);
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = loginJson.GetProperty("access_token").GetString();

        Assert.IsFalse(string.IsNullOrEmpty(accessToken));

        var productName = $"Phone_{Guid.NewGuid():N}";

        var createProductBody = new
        {
            name = productName,
            price = 5000
        };

        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        createRequest.Content = JsonContent.Create(createProductBody);

        var createResponse = await client.SendAsync(createRequest);
        Assert.AreEqual(HttpStatusCode.OK, createResponse.StatusCode);

        var createJson = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var productId = createJson.GetProperty("id").GetInt32();
        var returnedName = createJson.GetProperty("name").GetString();

        Assert.AreEqual(productName, returnedName);

        var listRequest = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/products");
        listRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var listResponse = await client.SendAsync(listRequest);
        Assert.AreEqual(HttpStatusCode.OK, listResponse.StatusCode);

        var listJson = await listResponse.Content.ReadFromJsonAsync<JsonElement>();

        bool productExists = false;

        foreach (var item in listJson.EnumerateArray())
        {
            if (item.GetProperty("id").GetInt32() == productId)
            {
                productExists = true;
                Assert.AreEqual(productName, item.GetProperty("name").GetString());
                break;
            }
        }

        Assert.IsTrue(productExists, "The created product was not found in /products.");
    }

    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        var adminLoginBody = new
        {
            username = "admin",
            password = "admin"
        };

        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", adminLoginBody);
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = loginJson.GetProperty("access_token").GetString();

        Assert.IsFalse(string.IsNullOrEmpty(accessToken));

        var productName = $"Phone_{Guid.NewGuid():N}";

        var createProductBody = new
        {
            name = productName,
            price = 5000
        };

        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        createRequest.Content = JsonContent.Create(createProductBody);

        var createResponse = await client.SendAsync(createRequest);
        Assert.AreEqual(HttpStatusCode.OK, createResponse.StatusCode);

        var createJson = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var productId = createJson.GetProperty("id").GetInt32();

        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"{GlobalContext.appUrl}/product/{productId}");
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var deleteResponse = await client.SendAsync(deleteRequest);

        Assert.IsTrue(
            deleteResponse.StatusCode == HttpStatusCode.OK ||
            deleteResponse.StatusCode == HttpStatusCode.NoContent,
            $"Unexpected status code: {deleteResponse.StatusCode}"
        );

        if (deleteResponse.StatusCode == HttpStatusCode.OK)
        {
            var deleteJson = await deleteResponse.Content.ReadFromJsonAsync<JsonElement>();
            Assert.AreEqual(productId, deleteJson.GetProperty("id").GetInt32());
            Assert.AreEqual(productName, deleteJson.GetProperty("name").GetString());
        }

        var listRequest = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/products");
        listRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var listResponse = await client.SendAsync(listRequest);
        Assert.AreEqual(HttpStatusCode.OK, listResponse.StatusCode);

        var listJson = await listResponse.Content.ReadFromJsonAsync<JsonElement>();

        bool productStillExists = false;

        foreach (var item in listJson.EnumerateArray())
        {
            if (item.GetProperty("id").GetInt32() == productId)
            {
                productStillExists = true;
                break;
            }
        }

        Assert.IsFalse(productStillExists, "The deleted product is still listed in /products.");
    }
}