namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Text.Json;
using System.Net;
using System.Text;

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

    /*
    Given I am an admin user​
    When I add a product to the catalog​
    Then The product is available to be used in the app
    */
    [TestMethod]
    public async Task TestAdminAddsProductToTheCatalog()
    {
        // Arrange (Given I am an admin user​)

        // Login as admin
        var loginResponse = await _httpClient.PostAsync($"{_appUrl}/login", new StringContent(
            JsonSerializer.Serialize(new { username = "admin", password = "admin" }),
            Encoding.UTF8,
            "application/json"
        ));

        // Extract access token from login response
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginContent).RootElement;
        var adminToken = loginJson.GetProperty("access_token").GetString();

        // Create product
        var product = "product_" + randomString(8);

        // Act (When I add a product to the catalog​)

        // Build request with auth token and product name
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_appUrl}/product");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
        request.Content = new StringContent(
            JsonSerializer.Serialize(new { name = product }),
            Encoding.UTF8,
            "application/json"
            );

        var response = await _httpClient.SendAsync(request);

        // Assert (Then The product is available to be used in the app)

        // Check that the response is successful
        Assert.IsTrue(response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.Created);

        // Check that the response contains the expected data (product)
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonDocument.Parse(responseContent).RootElement;
        Assert.AreEqual(product, jsonResponse.GetProperty("name").GetString());
    }

    /*
    Given I am an admin user​
    When I delete a product from the catalog
    Then The product is deleted
    */
    [TestMethod]
    public async Task TestAdminRemovesProductFromTheCatalog()
    {
        // Arrange (Given I am an admin user​)

        // Create name for product to be deleted
        var productToDelete = "product_" + randomString(8);

        // Login as admin
        var loginResponse = await _httpClient.PostAsync($"{_appUrl}/login", new StringContent(
            JsonSerializer.Serialize(new { username = "admin", password = "admin" }),
            Encoding.UTF8,
            "application/json"
        ));

        // Extract access token from login response
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginContent).RootElement;
        var adminToken = loginJson.GetProperty("access_token").GetString();

        // Act (When I delete a product from the catalog)

        // Create product to delete
        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"{_appUrl}/product");
        createRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
        createRequest.Content = new StringContent(
            JsonSerializer.Serialize(new { name = productToDelete }),
            Encoding.UTF8,
            "application/json"
            );
        var createResponse = await _httpClient.SendAsync(createRequest);

        // Extract product id from createResponse
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createJson = JsonDocument.Parse(createContent).RootElement;
        var productId = createJson.GetProperty("id").GetInt32();

        // Delete product using its id
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"{_appUrl}/product/{productId}");
        deleteRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
        var response = await _httpClient.SendAsync(deleteRequest);

        // Assert (Then The product is deleted)

        // Check that the response is successful
        Assert.IsTrue(response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Created);

        // Check that deleted product name matches
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonDocument.Parse(responseContent).RootElement;
        Assert.AreEqual(productToDelete, jsonResponse.GetProperty("name").GetString());
    }
}
