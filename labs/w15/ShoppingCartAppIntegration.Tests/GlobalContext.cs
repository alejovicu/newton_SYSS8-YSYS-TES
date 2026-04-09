namespace ShoppingCartAppIntegration.Tests;

[TestClass]
public class GlobalContext
{
    public static string appUrl { get; private set; } = string.Empty;

    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        // Read from .runsettings and store globally once
        appUrl = context.Properties["appUrl"]?.ToString() ?? string.Empty;

        if (string.IsNullOrEmpty(appUrl))
        {
            throw new AssertFailedException("appUrl not found in .runsettings file.");
        }
    }
}