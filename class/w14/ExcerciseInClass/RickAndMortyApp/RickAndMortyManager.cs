namespace RickAndMortyApp;

using System.Text.Json;

public class RickAndMortyManager
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "https://rickandmortyapi.com/api";

    public RickAndMortyManager()
    {
        _httpClient = new HttpClient();
    }

    public RickAndMortyManager(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
    }

    public async Task<Int16> GetCharacterID(string characterName)
    {
        try
        {
            var encodedCharacterName = Uri.EscapeDataString(characterName);
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/character/?name={encodedCharacterName}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var characterResponse = JsonSerializer.Deserialize<CharacterResponse>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            var matchingCharacter = characterResponse?.Results.FirstOrDefault(character =>
                string.Equals(character.Name, characterName, StringComparison.OrdinalIgnoreCase)
            );

            return Convert.ToInt16(matchingCharacter?.Id ?? 0);
        }
        catch
        {
            return 0;
        }
    }

    public async Task<List<string>> GetEpisodesWhereCharacterIsPresent(string characterName)
    {
        var characterId = await GetCharacterID(characterName);
        if (characterId == 0)
        {
            throw new InvalidOperationException("Character not found.");
        }

        try
        {
            var episodeCodes = new List<string>();
            var characterUrl = $"{ApiBaseUrl}/character/{characterId}";
            var nextUrl = $"{ApiBaseUrl}/episode";

            while (!string.IsNullOrWhiteSpace(nextUrl))
            {
                var response = await _httpClient.GetAsync(nextUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var episodeResponse = JsonSerializer.Deserialize<EpisodeResponse>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (episodeResponse is null)
                {
                    break;
                }

                episodeCodes.AddRange(
                    episodeResponse.Results
                        .Where(episode => episode.Characters.Any(url =>
                            string.Equals(url, characterUrl, StringComparison.OrdinalIgnoreCase)
                        ))
                        .Select(episode => episode.EpisodeCode)
                );

                nextUrl = episodeResponse.Info?.Next ?? string.Empty;
            }

            return episodeCodes;
        }
        catch
        {
            return new List<string>();
        }
    }
}
