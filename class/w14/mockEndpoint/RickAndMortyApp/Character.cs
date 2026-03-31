using System.Text.Json.Serialization;

namespace RickAndMortyApp;

public class Character
{
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    public string Species { get; set; } = string.Empty;

    [JsonPropertyName("created")]
    public DateTime CreatedAt { get; set; }
}
