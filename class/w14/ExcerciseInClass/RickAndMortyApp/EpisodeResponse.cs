using System.Text.Json.Serialization;

namespace RickAndMortyApp;

public class EpisodeResponse
{
    public Info? Info { get; set; }
    public List<Episode> Results { get; set; } = new();
}
