namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

[TestClass]
public class Product
{
    private static readonly HttpClient client = new HttpClient();

    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {
        // Arrange
        var token = await RegisterAndLogin();

        var productName = "product_" + Guid.NewGuid().ToString("N")[..10];

        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        createRequest.Content = JsonContent.Create(new { name = productName });

        // Act
        var createResponse = await client.SendAsync(createRequest);

        // Assert create
        Assert.AreEqual(System.Net.HttpStatusCode.OK, createResponse.StatusCode);

        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.IsNotNull(createdProduct);
        Assert.AreEqual(productName, createdProduct.name);

        var listRequest = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/products");
        listRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var listResponse = await client.SendAsync(listRequest);
        Assert.AreEqual(System.Net.HttpStatusCode.OK, listResponse.StatusCode);

        var products = await listResponse.Content.ReadFromJsonAsync<List<ProductResponse>>();
        Assert.IsNotNull(products);
        Assert.IsTrue(products.Any(p => p.name == productName));
    }

    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange
        var token = await RegisterAndLogin();

        var productName = "product_" + Guid.NewGuid().ToString("N")[..10];

        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        createRequest.Content = JsonContent.Create(new { name = productName });

        var createResponse = await client.SendAsync(createRequest);
        Assert.AreEqual(System.Net.HttpStatusCode.OK, createResponse.StatusCode);

        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.IsNotNull(createdProduct);

        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"{GlobalContext.appUrl}/product/{createdProduct.id}");
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var deleteResponse = await client.SendAsync(deleteRequest);

        // Assert delete
        Assert.AreEqual(System.Net.HttpStatusCode.OK, deleteResponse.StatusCode);

        var listRequest = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/products");
        listRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var listResponse = await client.SendAsync(listRequest);
        Assert.AreEqual(System.Net.HttpStatusCode.OK, listResponse.StatusCode);

        var products = await listResponse.Content.ReadFromJsonAsync<List<ProductResponse>>();
        Assert.IsNotNull(products);
        Assert.IsFalse(products.Any(p => p.id == createdProduct.id));
    }

    private static async Task<string> RegisterAndLogin()
    {
        var credentials = new
        {
            username = "admin",
            password = "admin"
        };

        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", credentials);
        Assert.AreEqual(System.Net.HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.IsNotNull(loginJson);
        Assert.IsFalse(string.IsNullOrEmpty(loginJson.access_token));

        return loginJson.access_token!;
    }

    private class LoginResponse
    {
        public string? access_token { get; set; }
    }

    private class ProductResponse
    {
        public int id { get; set; }
        public string? name { get; set; }
    }
}