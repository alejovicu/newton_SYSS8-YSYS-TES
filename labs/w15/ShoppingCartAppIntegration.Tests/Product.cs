using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace ShoppingCartAppIntegration.Tests;

[TestClass]
public class Product
{
    private static readonly HttpClient client = new HttpClient();

    private static string RandomProductName() => "product_" + Guid.NewGuid().ToString("N")[..8];

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
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginJson.GetProperty("access_token").GetString();

        var request = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var productName = RandomProductName();

        request.Content = JsonContent.Create(new
        {
            name = productName
        });

        // Act
        var productResponse = await client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, productResponse.StatusCode);

        var productJson = await productResponse.Content.ReadFromJsonAsync<JsonElement>();

        Assert.IsTrue(productJson.GetProperty("id").GetInt32() > 0);
        Assert.AreEqual(productName, productJson.GetProperty("name").GetString());
    }


    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange 
        var loginBody = new
        {
            username = "admin",
            password = "admin"
        };

        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", loginBody);
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginJson.GetProperty("access_token").GetString();

        var productName = RandomProductName();

        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        createRequest.Content = JsonContent.Create(new
        {
            name = productName
        });

        var createResponse = await client.SendAsync(createRequest);
        Assert.AreEqual(HttpStatusCode.OK, createResponse.StatusCode);

        var createJson = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var productId = createJson.GetProperty("id").GetInt32();

        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"{GlobalContext.appUrl}/product/{productId}");
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act 
        var deleteResponse = await client.SendAsync(deleteRequest);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode);

    }
}