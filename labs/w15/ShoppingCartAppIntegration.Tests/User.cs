namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

[TestClass]
public class User
{
    // Shared HttpClient is recommended; HttpClient is thread-safe for request sending
    // and avoids socket exhaustion across parallel tests.
    private static readonly HttpClient client = new HttpClient();

    /*
    Given I am a new potential customer
    When I sign up in the app
    Then I should be able to log in as an application customer
    */
    [TestMethod]
    public async Task RegisterNewCustomer()
    {
        // Hint: Use appUrl from GlobalContext to make API calls to the application
        // GlobalContext.appUrl

        // Arrange — build a unique customer name so this test is isolated
        // from the other parallel tests running against the same backend.
        var username = "customer_" + Utils.RandomString(20);
        var password = "1234";
        var body = JsonContent.Create(new { username, password });

        // Act — sign the new customer up
        var signupResponse = await client.PostAsync(
            $"{GlobalContext.appUrl}/signup",
            JsonContent.Create(new { username, password })
        );

        // Assert — status should be 200 OK and the returned username
        // should match the one we just sent in.
        Assert.AreEqual(HttpStatusCode.OK, signupResponse.StatusCode,
            "Signup should return 200 OK for a new username.");

        var signupContent = await signupResponse.Content.ReadAsStringAsync();
        var signupJson = JsonDocument.Parse(signupContent).RootElement;
        Assert.AreEqual(
            username,
            signupJson.GetProperty("username").GetString(),
            "Signup response should echo the username we registered."
        );

        // Act — verify the same credentials can now log in
        var loginResponse = await client.PostAsync(
            $"{GlobalContext.appUrl}/login",
            JsonContent.Create(new { username, password })
        );

        // Assert — login should succeed
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode,
            "Login should return 200 OK for the newly created customer.");
    }

    /*
    Given I am a customer
    When I log in into the application
    Then I should see all the products on my cart (empty for a brand new user)
    */
    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        // Arrange — create a brand new customer so the test is self-contained
        // and the cart is guaranteed to be empty.
        var username = "customer_" + Utils.RandomString(20);
        var password = "1234";

        var signupResponse = await client.PostAsync(
            $"{GlobalContext.appUrl}/signup",
            JsonContent.Create(new { username, password })
        );
        Assert.AreEqual(HttpStatusCode.OK, signupResponse.StatusCode,
            "Precondition failed: could not sign up a new customer.");

        // Log in to obtain an access token
        var loginResponse = await client.PostAsync(
            $"{GlobalContext.appUrl}/login",
            JsonContent.Create(new { username, password })
        );
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode,
            "Precondition failed: could not log in as the new customer.");

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var accessToken = JsonDocument.Parse(loginContent)
            .RootElement.GetProperty("access_token").GetString();

        Assert.IsFalse(string.IsNullOrWhiteSpace(accessToken),
            "Login response should contain an access_token.");

        // Act — call /user with the bearer token
        using var cartRequest = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/user");
        cartRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var cartResponse = await client.SendAsync(cartRequest);

        // Assert — 200 OK and the cart should be empty for a brand new user
        Assert.AreEqual(HttpStatusCode.OK, cartResponse.StatusCode,
            "GET /user should return 200 OK with a valid bearer token.");

        var cartContent = await cartResponse.Content.ReadAsStringAsync();
        var cartJson = JsonDocument.Parse(cartContent).RootElement;

        Assert.IsTrue(cartJson.TryGetProperty("products", out var products),
            "Response should contain a 'products' array.");
        Assert.AreEqual(JsonValueKind.Array, products.ValueKind,
            "'products' should be an array.");
        Assert.AreEqual(0, products.GetArrayLength(),
            "A brand new customer should have 0 products in the cart.");
    }
}
