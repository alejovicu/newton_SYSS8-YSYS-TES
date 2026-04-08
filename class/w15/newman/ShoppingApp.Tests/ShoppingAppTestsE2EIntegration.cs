namespace ShoppingApp.Tests;

using System.Net.Http;
using System.Text.Json;
using System.Net;
using System.Text;


[TestClass]
public sealed class ShoppingAppTestsE2EIntegration
{

    private readonly string _appUrl = "http://localhost:8000";
    private readonly HttpClient _httpClient = new HttpClient();


    /*
    Given I am a new potential customer​
    When I sign in in the app​
    Then I should be able to log in as an application customer
    */
    [TestMethod]
    public async Task CreateNewCustomer()
    {
        // Arrange

        // Create username and password for the new customer
        var username = "customer_" + Utils.RandomString(8);
        var password = "Password123!";
        
        // APP url

        // Act
        // Call signup endpoint
        var response = await _httpClient.PostAsync($"{_appUrl}/signup", new StringContent(
            JsonSerializer.Serialize(new { username, password }),
            Encoding.UTF8,
            "application/json"
        ));


        // Assert
        // Check that the response is successful
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        // Check that the response contains the expected data (username)
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
}
