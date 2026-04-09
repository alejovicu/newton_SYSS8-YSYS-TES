using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartAppIntegration.Tests;

[TestClass]
public class Product
{
    private readonly string _appUrl = GlobalContext.appUrl;
    private readonly HttpClient _httpClient = new HttpClient();

    private string randomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {
        // Arrange
        
        // Generate new product name
        var productName = "product_" + randomString(6);

        // Login as admin
        var loginResponse = await _httpClient.PostAsync($"{_appUrl}/login", new StringContent(
        JsonSerializer.Serialize(new { username = "admin", password = "admin"}),
        Encoding.UTF8,
        "application/json"
        ));
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        // Extract auth token from login response
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginContent).RootElement;
        var token = loginJson.GetProperty("access_token").GetString();

        // Act

        // Create a product
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_appUrl}/product");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(
            JsonSerializer.Serialize(new { name = productName }),
            Encoding.UTF8,
            "application/json"
        );
        var createProductResponse = await _httpClient.SendAsync(request);

        // Assert

        // Check the response from the "/product" endpoint
        Assert.AreEqual(HttpStatusCode.OK, createProductResponse.StatusCode);

        // Check that the product name is correct
        var createProductContent = await createProductResponse.Content.ReadAsStringAsync();
        var createProductJson = JsonDocument.Parse(createProductContent).RootElement;
        Assert.AreEqual(productName, createProductJson.GetProperty("name").GetString());
    }


    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange
        // Generate a new product name
        var productName = "product_" + randomString(6);

        // Login as admin
        var loginResponse = await _httpClient.PostAsync($"{_appUrl}/login", new StringContent(
        JsonSerializer.Serialize(new { username = "admin", password = "admin"}),
        Encoding.UTF8,
        "application/json"
        ));       

        // Extract auth token from login response
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginContent).RootElement;
        var token = loginJson.GetProperty("access_token").GetString();

        // Create a product to delete
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_appUrl}/product");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(
            JsonSerializer.Serialize(new { name = productName }),
            Encoding.UTF8,
            "application/json"
        );
        var createProductResponse = await _httpClient.SendAsync(request);
        var createBody = JsonDocument.Parse(await createProductResponse.Content.ReadAsStringAsync()).RootElement;
        var productId = createBody.GetProperty("id").GetInt32();

        // Act

        // Delete the newly created product
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"{_appUrl}/product/{productId}");
        deleteRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var deleteResponse = await _httpClient.SendAsync(deleteRequest);

        // Assert

        // Check login success
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        // Check product creation success
        Assert.AreEqual(HttpStatusCode.OK, createProductResponse.StatusCode);

        // Check deletion success
        Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode);
    }
}
