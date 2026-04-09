namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

[TestClass]
public class Product
{
    private static readonly HttpClient client = new HttpClient();

    /*
    Given: I am an admin
    When: I add a new product to the catalog
    Then: The product should be created with a valid id and the correct name
    */
    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {
        // Arrange
        // Log in as admin
        var loginResponse = await client.PostAsync($"{GlobalContext.appUrl}/login", new StringContent(
            JsonSerializer.Serialize(new { username = "admin", password = "admin" }),
            Encoding.UTF8,
            "application/json"
        ));
        var loginBody = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync()).RootElement;
        var token = loginBody.GetProperty("access_token").GetString();

        // Prepare a product with a random name
        var productName = $"product_{Guid.NewGuid().ToString("N")[..8]}";
        var request = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        request.Headers.Add("Authorization", $"bearer {token}");
        request.Content = new StringContent(
            JsonSerializer.Serialize(new { name = productName, price = 99.99 }),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        // Call product endpoint to add the product
        var productResponse = await client.SendAsync(request);
        var productBody = JsonDocument.Parse(await productResponse.Content.ReadAsStringAsync()).RootElement;

        // Assert
        // Check that login and product creation were successful
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, productResponse.StatusCode);

        // Check that the product was created with a valid id and the correct name
        Assert.IsGreaterThan(productBody.GetProperty("id").GetInt32(), 0);
        Assert.AreEqual(productName, productBody.GetProperty("name").GetString());
    }

    /*
    Given: I am an admin and a product exists in the catalog
    When: I remove that product
    Then: The product should be deleted successfully
    */
    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange
        // Log in as admin
        var loginResponse = await client.PostAsync($"{GlobalContext.appUrl}/login", new StringContent(
            JsonSerializer.Serialize(new { username = "admin", password = "admin" }),
            Encoding.UTF8,
            "application/json"
        ));
        var loginBody = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync()).RootElement;
        var token = loginBody.GetProperty("access_token").GetString();

        // Create a product to delete
        var productName = $"product_{Guid.NewGuid().ToString("N")[..8]}";
        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");
        createRequest.Headers.Add("Authorization", $"bearer {token}");
        createRequest.Content = new StringContent(
            JsonSerializer.Serialize(new { name = productName, price = 99.99 }),
            Encoding.UTF8,
            "application/json"
        );

        var createResponse = await client.SendAsync(createRequest);
        var createBody = JsonDocument.Parse(await createResponse.Content.ReadAsStringAsync()).RootElement;
        var productId = createBody.GetProperty("id").GetInt32();

        // Act
        // Call delete endpoint to remove the product
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"{GlobalContext.appUrl}/product/{productId}");
        deleteRequest.Headers.Add("Authorization", $"bearer {token}");
        var deleteResponse = await client.SendAsync(deleteRequest);

        // Assert
        // Check that all requests were successful
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, createResponse.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode);
    }
}
