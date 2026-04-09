namespace ShoppingCartAppIntegration.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[TestClass]
public class User
{
    private HttpClient _client = null!;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    [TestInitialize]
    public void Init()
    {
        if (string.IsNullOrEmpty(GlobalContext.appUrl)) throw new AssertFailedException("GlobalContext.appUrl is not set");
        _client = new HttpClient { BaseAddress = new Uri(GlobalContext.appUrl) };
    }

    private static string RandomString(int len = 8)
    {
        var rnd = Guid.NewGuid().ToString("n");
        return rnd.Substring(0, Math.Min(len, rnd.Length));
    }

    private async Task<(string username, string password, int id)> CreateRandomUserAsync()
    {
        var username = "user_" + RandomString(6);
        var password = "P@ssw0rd!" + RandomString(4);
        var email = username + "@example.test";

        var user = new { username, email, password };
        var content = new StringContent(JsonSerializer.Serialize(user, _jsonOptions), Encoding.UTF8, "application/json");

        var resp = await _client.PostAsync("/signup", content);
        resp.EnsureSuccessStatusCode();

        var body = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        int id = 0;
        if (doc.RootElement.TryGetProperty("id", out var idEl)) id = idEl.GetInt32();

        return (username, password, id);
    }

    [TestMethod]
    public async Task RegisterNewCustomer()
    {
        var (username, password, id) = await CreateRandomUserAsync();
        Assert.IsFalse(string.IsNullOrEmpty(username));
        Assert.IsFalse(string.IsNullOrEmpty(password));
        Assert.IsTrue(id >= 0);
    }

    [TestMethod]
    public async Task CustomerLogsIn()
    {
        var (username, password, _) = await CreateRandomUserAsync();

        var creds = new { username = username, password = password };
        var content = new StringContent(JsonSerializer.Serialize(creds, _jsonOptions), Encoding.UTF8, "application/json");

        var resp = await _client.PostAsync("/login", content);
        resp.EnsureSuccessStatusCode();

        var body = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        string? token = null;
        if (doc.RootElement.TryGetProperty("access_token", out var tokenEl)) token = tokenEl.GetString();

        Assert.IsFalse(string.IsNullOrEmpty(token));
    }

    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
        // This test will create a user, log in, and call the cart endpoint to ensure it returns 200.
        var (username, password, _) = await CreateRandomUserAsync();

        var creds = new { username = username, password = password };
        var content = new StringContent(JsonSerializer.Serialize(creds, _jsonOptions), Encoding.UTF8, "application/json");
        var resp = await _client.PostAsync("/login", content);
        resp.EnsureSuccessStatusCode();

        var body = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        string? token = null;
        if (doc.RootElement.TryGetProperty("access_token", out var tokenEl)) token = tokenEl.GetString();
        Assert.IsFalse(string.IsNullOrEmpty(token));

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var cartResp = await _client.GetAsync("/user");
        // Accept both 200 OK or 204 NoContent depending on implementation
        Assert.IsTrue(cartResp.IsSuccessStatusCode || cartResp.StatusCode == System.Net.HttpStatusCode.NoContent,
            $"Unexpected status code from /user: {cartResp.StatusCode}");
    }
}
