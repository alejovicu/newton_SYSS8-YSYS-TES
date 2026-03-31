using System.Text.Json.Serialization;

namespace RickAndMortyApp;

public class CharacterResponse
{
    public Info? Info { get; set; }
    public List<Character> Results { get; set; } = new();
}
