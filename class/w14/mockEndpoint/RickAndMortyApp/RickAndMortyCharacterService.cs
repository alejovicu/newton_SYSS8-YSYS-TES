using System.Text.Json;

namespace RickAndMortyApp;

public class RickAndMortyCharacterService
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "https://rickandmortyapi.com/api";

    public RickAndMortyCharacterService()
    {
        _httpClient = new HttpClient();
    }

    public RickAndMortyCharacterService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Character>> GetCharactersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/character");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var characterResponse = JsonSerializer.Deserialize<CharacterResponse>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return characterResponse?.Results ?? new List<Character>();
        }
        catch
        {
            return new List<Character>();
        }
    }
}
