using System.Text.Json.Serialization;

namespace RickAndMortyApp;

public class Info
{
    public int Count { get; set; }
    public int Pages { get; set; }
    public string? Next { get; set; }
    public string? Prev { get; set; }
}
