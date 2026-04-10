namespace ShoppingCartAppIntegration.Tests;

public static class Utils
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    /// <summary>
    /// Generates a thread-safe random alphanumeric string of the given length.
    /// Uses <see cref="Random.Shared"/> which is safe for concurrent access,
    /// so it plays nicely with MSTest method-level parallelization.
    /// </summary>
    public static string RandomString(int length)
    {
        return new string(Enumerable.Range(0, length)
            .Select(_ => Chars[Random.Shared.Next(Chars.Length)])
            .ToArray());
    }
}
