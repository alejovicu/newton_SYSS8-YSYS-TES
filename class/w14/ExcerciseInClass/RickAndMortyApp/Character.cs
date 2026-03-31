namespace RickAndMortyApp;

public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string AirDate { get; set; } = null!;
    public string EpisodeCode { get; set; } = null!;
    public List<string> Characters { get; set; } = new();
    public string Url { get; set; } = null!;
    public DateTime Created { get; set; }
}