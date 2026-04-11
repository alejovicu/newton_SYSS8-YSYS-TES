
using System.Net.Http.Json;
namespace ShoppingCartAppIntegration.Tests;

[TestClass]
public class Product
{

    [TestMethod]
    public void AdminAddsProductToTheCatalog()
    {
        // Register the first user (admin)
        var username = $"admin_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        var password = "adminpass";
        var signupUrl = $"{GlobalContext.appUrl}/signup";
        var loginUrl = $"{GlobalContext.appUrl}/login";
        var productUrl = $"{GlobalContext.appUrl}/product";

        var client = new HttpClient();

        // Register admin
        var signupResponse = client.PostAsJsonAsync(signupUrl, new { username, password }).Result;
        Assert.IsTrue(signupResponse.IsSuccessStatusCode, $"Signup failed: {signupResponse.StatusCode}");

        // Login as admin
        var loginResponse = client.PostAsJsonAsync(loginUrl, new { username, password }).Result;
        Assert.IsTrue(loginResponse.IsSuccessStatusCode, $"Login failed: {loginResponse.StatusCode}");
        var loginContent = loginResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>().Result;
        string accessToken = loginContent.TryGetProperty("access_token", out var tokenProp) ? tokenProp.GetString() : null;
        Assert.IsFalse(string.IsNullOrEmpty(accessToken), "Access token not found in login response");

        // Add product
        var productName = $"Product_{Guid.NewGuid().ToString("N").Substring(0, 6)}";
        var request = new HttpRequestMessage(HttpMethod.Post, productUrl);
        request.Content = System.Net.Http.Json.JsonContent.Create(new { name = productName });
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var addResponse = client.SendAsync(request).Result;
        Assert.IsTrue(addResponse.IsSuccessStatusCode, $"Add product failed: {addResponse.StatusCode}");

        // Check response for product name
        var addContent = addResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>().Result;
        var returnedName = addContent.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;
        Assert.AreEqual(productName, returnedName, "Product name in response does not match");
    }


    [TestMethod]
    public void AdminRemovesProductFromTheCatalog()
    {
        // Register the first user (admin)
        var username = $"admin_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        var password = "adminpass";
        var signupUrl = $"{GlobalContext.appUrl}/signup";
        var loginUrl = $"{GlobalContext.appUrl}/login";
        var productUrl = $"{GlobalContext.appUrl}/product";

        var client = new HttpClient();

        // Register admin
        var signupResponse = client.PostAsJsonAsync(signupUrl, new { username, password }).Result;
        Assert.IsTrue(signupResponse.IsSuccessStatusCode, $"Signup failed: {signupResponse.StatusCode}");

        // Login as admin
        var loginResponse = client.PostAsJsonAsync(loginUrl, new { username, password }).Result;
        Assert.IsTrue(loginResponse.IsSuccessStatusCode, $"Login failed: {loginResponse.StatusCode}");
        var loginContent = loginResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>().Result;
        string accessToken = loginContent.TryGetProperty("access_token", out var tokenProp) ? tokenProp.GetString() : null;
        Assert.IsFalse(string.IsNullOrEmpty(accessToken), "Access token not found in login response");

        // Add product to be deleted
        var productName = $"Product_{Guid.NewGuid().ToString("N").Substring(0, 6)}";
        var addRequest = new HttpRequestMessage(HttpMethod.Post, productUrl);
        addRequest.Content = System.Net.Http.Json.JsonContent.Create(new { name = productName });
        addRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var addResponse = client.SendAsync(addRequest).Result;
        Assert.IsTrue(addResponse.IsSuccessStatusCode, $"Add product failed: {addResponse.StatusCode}");
        var addContent = addResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>().Result;
        var productId = addContent.TryGetProperty("id", out var idProp) ? idProp.GetInt32() : 0;
        Assert.IsTrue(productId > 0, "Product ID not found in add response");

        // Remove product
        var deleteUrl = $"{GlobalContext.appUrl}/product/{productId}";
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, deleteUrl);
        deleteRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var deleteResponse = client.SendAsync(deleteRequest).Result;
        Assert.IsTrue(deleteResponse.IsSuccessStatusCode, $"Delete product failed: {deleteResponse.StatusCode}");
    }
}
