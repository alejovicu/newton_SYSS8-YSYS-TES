using System.Text.Json.Serialization;

namespace RickAndMortyApp;

public class Episode
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    [JsonPropertyName("air_date")]
    public string AirDate { get; set; } = null!;

    [JsonPropertyName("episode")]
    public string EpisodeCode { get; set; } = null!;

    public List<string> Characters { get; set; } = new();
    public string Url { get; set; } = null!;
    public DateTime Created { get; set; }
}