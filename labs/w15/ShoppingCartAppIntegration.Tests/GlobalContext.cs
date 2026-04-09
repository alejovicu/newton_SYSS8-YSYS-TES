namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Net.Http.Json;

[TestClass]
public class GlobalContext
{
    public static string appUrl { get; private set; } = string.Empty;

    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        appUrl = context.Properties["appUrl"]?.ToString() ?? string.Empty;

        if (string.IsNullOrEmpty(appUrl))
        {
            throw new AssertFailedException("appUrl not found in .runsettings file.");
        }

        using var client = new HttpClient();

        var adminBody = new
        {
            username = "admin",
            password = "admin"
        };

        var response = client.PostAsJsonAsync($"{appUrl}/signup", adminBody).GetAwaiter().GetResult();

        // OK om admin skapades nu, eller om den redan finns
        if (response.StatusCode != System.Net.HttpStatusCode.OK &&
            response.StatusCode != System.Net.HttpStatusCode.BadRequest)
        {
            throw new AssertFailedException($"Failed to ensure admin user exists. Status: {response.StatusCode}");
        }
    }
}