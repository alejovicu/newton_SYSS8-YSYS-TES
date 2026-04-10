namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

[TestClass]
public class Product
{
    // Shared HttpClient — thread-safe for request sending and safe to reuse
    // across parallel test runs.
    private static readonly HttpClient client = new HttpClient();

    // Admin credentials the class repo documents for this lab.
    private const string AdminUsername = "admin";
    private const string AdminPassword = "admin";

    /// <summary>
    /// Logs in as the admin user and returns the bearer access token.
    /// Shared helper for both admin-only product tests.
    /// </summary>
    private static async Task<string> LoginAsAdminAsync()
    {
        var response = await client.PostAsync(
            $"{GlobalContext.appUrl}/login",
            JsonContent.Create(new { username = AdminUsername, password = AdminPassword })
        );

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
            "Precondition failed: could not log in as admin.");

        var body = await response.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(body)
            .RootElement.GetProperty("access_token").GetString();

        Assert.IsFalse(string.IsNullOrWhiteSpace(token),
            "Admin login response should contain an access_token.");

        return token!;
    }

    /*
    Given I am an admin user
    When I add a product to the catalog
    Then The product is available to be used in the app
    */
    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {
        // Arrange — log in as admin and prepare a unique product name
        var adminToken = await LoginAsAdminAsync();
        var productName = "product_" + Utils.RandomString(20);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product")
        {
            Content = JsonContent.Create(new { name = productName })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Act — create the product
        var response = await client.SendAsync(request);

        // Assert — 200 OK and the returned name should match what we sent
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
            "POST /product should return 200 OK for an admin with a valid payload.");

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body).RootElement;

        Assert.AreEqual(
            productName,
            json.GetProperty("name").GetString(),
            "The created product should echo back the name we sent."
        );
    }

    /*
    Given I am an admin user
    When I remove a product from the catalog
    Then The product should not be listed in the app to be used
    */
    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange — log in as admin
        var adminToken = await LoginAsAdminAsync();

        // Create a product that we will then delete
        var productName = "product_" + Utils.RandomString(20);

        using var createRequest = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product")
        {
            Content = JsonContent.Create(new { name = productName })
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var createResponse = await client.SendAsync(createRequest);
        Assert.AreEqual(HttpStatusCode.OK, createResponse.StatusCode,
            "Precondition failed: could not create the product that should be deleted.");

        var createBody = await createResponse.Content.ReadAsStringAsync();
        var createJson = JsonDocument.Parse(createBody).RootElement;

        Assert.IsTrue(createJson.TryGetProperty("id", out var idProperty),
            "Create-product response should contain an 'id' field.");

        // The id can be a number or a string depending on the backend implementation.
        // Read it as raw text so we don't care which one it is.
        var productId = idProperty.ValueKind switch
        {
            JsonValueKind.String => idProperty.GetString(),
            JsonValueKind.Number => idProperty.GetRawText(),
            _ => idProperty.GetRawText()
        };

        Assert.IsFalse(string.IsNullOrWhiteSpace(productId),
            "Create-product response should include a non-empty id.");

        // Act — delete the product using its id
        using var deleteRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"{GlobalContext.appUrl}/product/{productId}"
        );
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var deleteResponse = await client.SendAsync(deleteRequest);

        // Assert — 200 OK
        Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode,
            "DELETE /product/{id} should return 200 OK for an admin.");
    }
}
