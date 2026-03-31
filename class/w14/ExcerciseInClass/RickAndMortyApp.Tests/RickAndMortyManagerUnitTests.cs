using Moq;
using Moq.Protected;
using System.Net;
using System.Text;

namespace RickAndMortyApp.Tests;

[TestClass]
[TestCategory("Unit")]
public sealed class RickAndMortyManagerUnitTests
{
    private RickAndMortyManager _manager = null!;
    private Mock<HttpMessageHandler> _mockHandler = null!;
    private HttpClient _httpClient = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object);
        _manager = new RickAndMortyManager(_httpClient);
    }

    [TestMethod]
    [DataRow("Rick Sanchez", 1)]
    [DataRow("Morty Smith", 2)]
    [DataRow("Alien Rick", 15)]
    public async Task GetCharacterID_ReturnsCorrectID_UsingMockedHttpClient(string characterName, int expectedId)
    {
        // Arrange
        SetupMockHandler(CreateCharacterResponse(expectedId, characterName));

        // Act
        var characterId = await _manager.GetCharacterID(characterName);

        // Assert
        Assert.AreEqual(expectedId, characterId);
    }

    [TestMethod]
    [DataRow("Pripudlian", new string[] { "S01E01", "S01E11", "S02E08", "S03E04" })]
    public async Task GetEpisodesWhereCharacterIsPresent_ReturnsCorrectEpisodes_UsingMockedHttpClient(string characterName, string[] expectedEpisodes)
    {
        // Arrange
        SetupMockHandler(
            CreateCharacterResponse(435, characterName),
            CreateEpisodeResponse()
        );

        // Act
        var episodes = await _manager.GetEpisodesWhereCharacterIsPresent(characterName);

        // Assert
        CollectionAssert.AreEqual(expectedEpisodes, episodes);
    }

    private void SetupMockHandler(params HttpResponseMessage[] responses)
    {
        var sequence = _mockHandler
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );

        foreach (var response in responses)
        {
            sequence = sequence.ReturnsAsync(response);
        }
    }

    private static HttpResponseMessage CreateCharacterResponse(int id, string name)
    {
        return CreateJsonResponse(
            $$"""
            {
              "info": { "count": 1, "pages": 1, "next": null, "prev": null },
              "results": [
                { "id": {{id}}, "name": "{{name}}" }
              ]
            }
            """
        );
    }

    private static HttpResponseMessage CreateEpisodeResponse()
    {
        return CreateJsonResponse(
            """
            {
              "info": { "count": 3, "pages": 1, "next": null, "prev": null },
              "results": [
                {
                  "id": 1,
                  "name": "Dummy Episode 1",
                  "air_date": "December 2, 2013",
                  "episode": "S01E01",
                  "characters": [
                    "https://rickandmortyapi.com/api/character/435"
                  ],
                  "url": "https://rickandmortyapi.com/api/episode/1",
                  "created": "2017-11-10T12:56:33.798Z"
                },
                {
                  "id": 1,
                  "name": "Dummy Episode 2",
                  "air_date": "December 2, 2013",
                  "episode": "S01E11",
                  "characters": [
                    "https://rickandmortyapi.com/api/character/435"
                  ],
                  "url": "https://rickandmortyapi.com/api/episode/2",
                  "created": "2017-11-10T12:56:33.798Z"
                },
                {
                  "id": 1,
                  "name": "Dummy Episode 3",
                  "air_date": "December 2, 2013",
                  "episode": "S02E08",
                  "characters": [
                    "https://rickandmortyapi.com/api/character/435"
                  ],
                  "url": "https://rickandmortyapi.com/api/episode/3",
                  "created": "2017-11-10T12:56:33.798Z"
                },
                {
                  "id": 1,
                  "name": "Dummy Episode 4",
                  "air_date": "December 2, 2013",
                  "episode": "S03E04",
                  "characters": [
                    "https://rickandmortyapi.com/api/character/435"
                  ],
                  "url": "https://rickandmortyapi.com/api/episode/4",
                  "created": "2017-11-10T12:56:33.798Z"
                }
              ]
            }
            """
        );
    }

    private static HttpResponseMessage CreateJsonResponse(string json)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }
}
