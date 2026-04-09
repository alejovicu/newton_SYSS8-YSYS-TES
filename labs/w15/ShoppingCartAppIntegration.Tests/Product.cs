namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Text.Json;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class Product
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _appUrl = GlobalContext.appUrl;

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
        var loginBody = new { username = "Admin", password = "admin" }; 
		await _httpClient.PostAsync($"{_appUrl}/signup", new StringContent(
            JsonSerializer.Serialize(loginBody), Encoding.UTF8, "application/json"));

        var loginResponse = await _httpClient.PostAsync($"{_appUrl}/login", new StringContent(
            JsonSerializer.Serialize(loginBody), Encoding.UTF8, "application/json"));
        
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(loginContent).RootElement.GetProperty("access_token").GetString();

		_httpClient.DefaultRequestHeaders.Authorization = null;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var productName = "product_" + randomString(5);
        
        // Act
        var response = await _httpClient.PostAsync($"{_appUrl}/product", new StringContent(
            JsonSerializer.Serialize(new { name = productName }), Encoding.UTF8, "application/json"));
            
        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }


    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange
        var loginBody = new { username = "Admin", password = "admin" };
        
        await _httpClient.PostAsync($"{_appUrl}/signup", new StringContent(
            JsonSerializer.Serialize(loginBody), Encoding.UTF8, "application/json"));
        
        var loginResponse = await _httpClient.PostAsync($"{_appUrl}/login", new StringContent(
            JsonSerializer.Serialize(loginBody), Encoding.UTF8, "application/json"));
        
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(loginContent).RootElement.GetProperty("access_token").GetString();

        _httpClient.DefaultRequestHeaders.Authorization = null;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var productName = "deleteMe" + randomString(5);
        var createResponse = await _httpClient.PostAsync($"{_appUrl}/product", new StringContent(
            JsonSerializer.Serialize(new { name = productName }), Encoding.UTF8, "application/json"));

		var createContent = await createResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(createContent);
        
        var productId = doc.RootElement.GetProperty("id").GetInt32();
        
        // Act 
		var response = await _httpClient.DeleteAsync($"{_appUrl}/product/{productId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
