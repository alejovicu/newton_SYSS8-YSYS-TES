namespace ShoppingCartAppIntegration.Tests;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

[TestClass]
public class User
{
    private readonly HttpClient client = new HttpClient();

    [TestMethod]
    public async Task RegisterNewCustomer()
    {
        var newCustomer = $"customer_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

        var requestBody = new
        {
            username = newCustomer,
            password = "password123"
        };

        var response = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/signup", requestBody);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.IsNotNull(json);

        var username = json.RootElement.GetProperty("username").GetString();

        Assert.AreEqual(newCustomer, username);
    }



    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        //  Skapar en unik användare för testet 
        var username = $"customer_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        var password = "password123";
 
        // Registrera en ny användare (signup)
        var signupResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/signup", new
        {
            username,
            password
        });
        
        // Verifierar att signup lyckades (http 200 OK)
        Assert.AreEqual(HttpStatusCode.OK, signupResponse.StatusCode);

        // Logga in med den nya användaren
        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", new
        {
            username,
            password
        });
        
        // Verifera att inloggning lyckades
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        // Läs login - response som innehåller token
        var loginBody = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginBody);

        // Kontrollerar att Access_token finns i svaret.
        Assert.IsTrue(
            loginJson.RootElement.TryGetProperty("access_token", out var tokenElement),
            $"access_token saknas i login response. Body: {loginBody}");

        // Hämtar token Stringen.
        var token = tokenElement.GetString();
        
        // Säkerställer att token inte är null eller tom
        Assert.IsFalse(string.IsNullOrWhiteSpace(token), "access_token är null eller tom.");

        // Skapa en GET Request till /user med Authorization-header
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/user");
        
        // Lägger till Bearer token för autentisering
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Skickar request
        var response = await client.SendAsync(request);
        
        // Veriferar att vi får OK svar användare autentiserad.
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        // Läs svaret från /user endpoint
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        // Kontrollerar att products finns i svaret / lista 
        Assert.IsTrue(json.RootElement.TryGetProperty("products", out _), "Products saknas i /user response.");
    }
}
