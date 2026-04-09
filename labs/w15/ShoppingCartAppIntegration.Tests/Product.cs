using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ShoppingCartAppIntegration.Tests;

[TestClass]
public class Product
{
  
private static readonly HttpClient client = new HttpClient();

    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {
        var adminToken = await LoginAsAdmin();
        var newProduct = $"product_{Guid.NewGuid():N}";

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{GlobalContext.appUrl}/product"
        );

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        request.Content = JsonContent.Create(new
        {
            name = newProduct
        });

        var response = await client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);

        var returnedName = json.RootElement.GetProperty("name").GetString();

        Assert.AreEqual(newProduct, returnedName);
    }

    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        var adminToken = await LoginAsAdmin();
        var newProduct = $"product_{Guid.NewGuid():N}";

        // 1. Skapa produkt först
        var createRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{GlobalContext.appUrl}/product"
        );

        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        createRequest.Content = JsonContent.Create(new
        {
            name = newProduct
        });

        var createResponse = await client.SendAsync(createRequest);

        Assert.AreEqual(HttpStatusCode.OK, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createJson = JsonDocument.Parse(createContent);

        // Här antar vi att API returnerar id
        var productId = createJson.RootElement.GetProperty("id").GetInt32();

        // 2. Ta bort produkt
        var deleteRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"{GlobalContext.appUrl}/product/{productId}"
        );

        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var deleteResponse = await client.SendAsync(deleteRequest);

        Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode);
    }

    private static async Task<string> LoginAsAdmin()
    {
        var loginBody = new
        {
            username = "admin",
            password = "admin"
        };

        var response = await client.PostAsJsonAsync(
            $"{GlobalContext.appUrl}/login",
            loginBody
        );
 

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var json = JsonDocument.Parse(content);

        var token = json.RootElement.GetProperty("access_token").GetString();

        Assert.IsFalse(string.IsNullOrWhiteSpace(token));

        return token!;
    }
}
