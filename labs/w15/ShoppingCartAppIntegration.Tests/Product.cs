using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShoppingCartAppIntegration.Tests;

[TestClass]
public class Product
{
    private readonly HttpClient client = new HttpClient();
    
    [TestMethod]
    public async Task AdminAddsProductToTheCatalog()
    {

        // Logga in som admin
        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", new
        {
            username = "admin",
            password = "admin"
        });

        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginBody = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginBody);

        Assert.IsTrue(
            loginJson.RootElement.TryGetProperty("access_token", out var tokenElement),
            $"access_token saknas i login response. Body: {loginBody}");

        var token = tokenElement.GetString();
        Assert.IsFalse(string.IsNullOrWhiteSpace(token), "Token är null eller tom.");

        // Skapa ett unikt produktnamn
        var productName = $"product_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

        // Skicka POST /product
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");

        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        request.Content = JsonContent.Create(new
        {
            name = productName
        });

        var response = await client.SendAsync(request);

        // Verifiera statuskod
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        // Läs svaret
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        // Verifiera att produkten har name
        Assert.IsTrue(json.RootElement.TryGetProperty("name", out var nameElement),
            "Product saknar name i response.");

        Assert.AreEqual(productName, nameElement.GetString(),
            "Produktnamnet matchar inte det som skickades.");

        // Verifiera att produkten har id
        Assert.IsTrue(json.RootElement.TryGetProperty("id", out var idElement),
            "Product saknar id i response.");
       
        Assert.IsTrue(idElement.GetInt32() > 0, "Product id är ogiltigt.");
        
    }
    
    [TestMethod]
    public async Task AdminRemovesProductFromTheCatalog()
    {
        // Arrange
        // Förbered testet genom att först logga in som admin
        // och skapa en produkt som sedan ska tas bort.

        // Logga in som admin via /login
        var loginResponse = await client.PostAsJsonAsync($"{GlobalContext.appUrl}/login", new
        {
            username = "admin",
            password = "admin"
        });

        // Kontrollera att inloggningen lyckades
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        // Läs svaret från login och parsa JSON
        var loginBody = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginBody);

        // Kontrollera att access_token finns i login-svaret
        Assert.IsTrue(
            loginJson.RootElement.TryGetProperty("access_token", out var tokenElement),
            $"access_token saknas i login response. Body: {loginBody}");

        // Hämta token som ska användas för autentiserade requests
        var token = tokenElement.GetString();

        // Säkerställ att token inte är null, tom eller bara whitespace
        Assert.IsFalse(string.IsNullOrWhiteSpace(token), "Token är null eller tom.");

        // Skapa ett unikt produktnamn så att testet inte krockar med andra produkter
        var productName = $"product_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

        // Skapa en POST-request till /product för att lägga till produkten
        using var createRequest = new HttpRequestMessage(HttpMethod.Post, $"{GlobalContext.appUrl}/product");

        // Lägg till Bearer token i Authorization-headern så admin blir autentiserad
        createRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Skicka med produktens namn i request body
        createRequest.Content = JsonContent.Create(new
        {
            name = productName
        });

        // Skicka requesten som skapar produkten
        var createResponse = await client.SendAsync(createRequest);

        // Kontrollera att produkten skapades korrekt
        Assert.AreEqual(HttpStatusCode.OK, createResponse.StatusCode);

        // Läs svaret från produktskapandet
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var createJson = JsonDocument.Parse(createBody);

        // Kontrollera att den skapade produkten har ett id i svaret
        Assert.IsTrue(createJson.RootElement.TryGetProperty("id", out var idElement),
            "Product saknar id i response.");

        // Hämta produktens id, detta behövs senare för att kunna radera exakt rätt produkt
        var productId = idElement.GetInt32();

        // Säkerställ att id är större än 0, alltså ett giltigt id
        Assert.IsTrue(productId > 0, "Product id är ogiltigt.");

        // Kontrollera att produkten också har name i svaret
        Assert.IsTrue(createJson.RootElement.TryGetProperty("name", out var nameElement),
            "Product saknar name i response.");

        // Kontrollera att namnet i svaret är samma som vi skickade in
        Assert.AreEqual(productName, nameElement.GetString(),
            "Produktnamnet matchar inte det som skickades.");

        // Act
        // Nu utför vi själva handlingen som testet handlar om
        // att ta bort den produkt som nyss skapades.

        // Skapa en DELETE-request till /product/{id}
        using var deleteRequest =
            new HttpRequestMessage(HttpMethod.Delete, $"{GlobalContext.appUrl}/product/{productId}");

        // Lägg till Bearer token även här så admin får rätt behörighet
        deleteRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Skicka delete-requesten
        var deleteResponse = await client.SendAsync(deleteRequest);

        // Assert
        // Här verifierar vi att borttagningen verkligen lyckades.

        // Kontrollera att delete gav godkänd statuskod
        // API:t verkar acceptera både 200 OK och 204 NoContent som lyckat svar
        Assert.IsTrue(
            deleteResponse.StatusCode == HttpStatusCode.OK ||
            deleteResponse.StatusCode == HttpStatusCode.NoContent,
            $"Fel statuskod vid delete: {(int)deleteResponse.StatusCode}");

        // Hämta hela produktlistan efter borttagningen
        using var getProductsRequest = new HttpRequestMessage(HttpMethod.Get, $"{GlobalContext.appUrl}/products");

        // Lägg till token även här
        getProductsRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Skicka requesten för att hämta alla produkter
        var getProductsResponse = await client.SendAsync(getProductsRequest);

        // Kontrollera att hämtningen av produktlistan lyckades
        Assert.AreEqual(HttpStatusCode.OK, getProductsResponse.StatusCode);

        // Läs svaret från /products
        var productsBody = await getProductsResponse.Content.ReadAsStringAsync();
        var productsJson = JsonDocument.Parse(productsBody);

        // Kontrollera att /products faktiskt returnerar en JSON-array
        Assert.AreEqual(JsonValueKind.Array, productsJson.RootElement.ValueKind,
            "Products response är inte en array.");

        // Gå igenom produktlistan och kontrollera om den borttagna produktens id fortfarande finns kvar
        var productStillExists = productsJson.RootElement.EnumerateArray()
            .Any(p =>
                p.TryGetProperty("id", out var currentId) &&
                currentId.GetInt32() == productId);

        // Verifiera att produkten INTE längre finns kvar i katalogen
        Assert.IsFalse(productStillExists, "Produkten finns fortfarande kvar efter delete.");
    }


   
}
