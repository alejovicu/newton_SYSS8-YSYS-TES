namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Text.Json;
using System.Net;
using System.Text;
using System.Linq;

[TestClass]
public class User
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly string _appUrl = GlobalContext.appUrl;

    private string randomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    [TestMethod]
    public async Task RegisterNewCustomer()
    {
		// Arrange
        var username = "customer_" +randomString(8);
        var password = "Password123!";
     
        // Act
        var response = await _httpClient.PostAsync($"{_appUrl}/signup", new StringContent(
            JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
        ));


        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonDocument.Parse(responseContent).RootElement;
        Assert.AreEqual(username, jsonResponse.GetProperty("username").GetString());
    
        // Validate if the user can log in with the created credentials
        var responseLogin = await _httpClient.PostAsync($"{_appUrl}/login", new StringContent(
            JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
        ));
        Assert.AreEqual(HttpStatusCode.OK, responseLogin.StatusCode);

    }

    [TestMethod]
    public async Task CustomerListsProductsInCart()
    {
		// Arrange
		var username = "customer_" +randomString(8);
		var password = "Password123!";

		//Act
		await _httpClient.PostAsync($"{_appUrl}/signup", new StringContent(
			JsonSerializer.Serialize(new { username, password }),
        	Encoding.UTF8, "application/json"));
        
        var loginResponse = await _httpClient.PostAsync($"{_appUrl}/login", new StringContent(
		JsonSerializer.Serialize(new { username, password }),
		Encoding.UTF8, "application/json"));
        
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(loginContent).RootElement.GetProperty("access_token").GetString();
        
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        var response = await _httpClient.GetAsync($"{_appUrl}/user");

		//Assert 
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var resContent = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(resContent).RootElement;
        
        Assert.AreEqual(username, json.GetProperty("username").GetString());
        Assert.AreEqual(0, json.GetProperty("products").GetArrayLength());

    }
}



