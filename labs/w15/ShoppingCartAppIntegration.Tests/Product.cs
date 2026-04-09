namespace ShoppingCartAppIntegration.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[TestClass]
public class Product
{
    private HttpClient _client = null!;
    private string? _adminToken;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    [TestInitialize]
    public void Init()
    {
        if (string.IsNullOrEmpty(GlobalContext.appUrl)) throw new AssertFailedException("GlobalContext.appUrl is not set");
        _client = new HttpClient { BaseAddress = new Uri(GlobalContext.appUrl) };
        _adminToken = Environment.GetEnvironmentVariable("ADMIN_TOKEN");
    }

    private async Task<string?> GetAdminTokenAsync()
    {
        if (!string.IsNullOrEmpty(_adminToken)) return _adminToken;

        var user = Environment.GetEnvironmentVariable("API_ADMIN_USER") ?? "admin";
        var pass = Environment.GetEnvironmentVariable("API_ADMIN_PASS") ?? "admin";
        var creds = new { username = user, password = pass };

        var content = new StringContent(JsonSerializer.Serialize(creds, _jsonOptions), Encoding.UTF8, "application/json");
        var resp = await _client.PostAsync("/login", content);
        if (!resp.IsSuccessStatusCode) return null;

        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        if (doc.RootElement.TryGetProperty("access_token", out var tokenEl)) return tokenEl.GetString();
        return null;
    }

    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {
        var token = await GetAdminTokenAsync();
        if (!string.IsNullOrEmpty(token))
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var product = new { name = "TestProduct-" + Guid.NewGuid(), price = 9.99m };
        var content = new StringContent(JsonSerializer.Serialize(product, _jsonOptions), Encoding.UTF8, "application/json");

        var resp = await _client.PostAsync("/product", content);
        resp.EnsureSuccessStatusCode();

        var responseBody = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseBody);
        int id = doc.RootElement.GetProperty("id").GetInt32();

        Assert.IsTrue(id > 0);
    }


    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        var token = await GetAdminTokenAsync();
        if (!string.IsNullOrEmpty(token))
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // create product to remove
        var product = new { name = "DeleteProduct-" + Guid.NewGuid(), price = 1.23m };
        var createResp = await _client.PostAsync("/product", new StringContent(JsonSerializer.Serialize(product, _jsonOptions), Encoding.UTF8, "application/json"));
        createResp.EnsureSuccessStatusCode();
        var body = await createResp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        int id = doc.RootElement.GetProperty("id").GetInt32();

        // delete
        var deleteResp = await _client.DeleteAsync($"/product/{id}");
        deleteResp.EnsureSuccessStatusCode();

        // Product deletion successful - no need to verify since GET /product/{id} endpoint doesn't exist
    }
}
