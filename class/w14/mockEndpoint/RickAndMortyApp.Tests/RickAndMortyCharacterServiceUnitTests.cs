using Moq;
using Moq.Protected;
using System.Net;

namespace RickAndMortyApp.Tests;

[TestClass]
[TestCategory("Unit")]
public class RickAndMortyCharacterServiceUnitTests
{
    private RickAndMortyCharacterService _service = null!;
    private Mock<HttpMessageHandler> _mockHandler = null!;
    private HttpClient _httpClient = null!;


    private string GetMockCharacterResponse()
    {
        return @"{
            ""info"": {
                ""count"": 826,
                ""pages"": 42,
                ""next"": ""https://rickandmortyapi.com/api/character?page=2"",
                ""prev"": null
            },
            ""results"": [
                {
                    ""id"": 1,
                    ""name"": ""Rick Sanchez"",
                    ""status"": ""Alive"",
                    ""species"": ""Human"",
                    ""type"": """",
                    ""gender"": ""Male"",
                    ""origin"": {
                        ""name"": ""Earth (C-137)"",
                        ""url"": ""https://rickandmortyapi.com/api/location/1""
                    },
                    ""location"": {
                        ""name"": ""Citadel of Ricks"",
                        ""url"": ""https://rickandmortyapi.com/api/location/3""
                    },
                    ""image"": ""https://rickandmortyapi.com/api/character/avatar/1.jpeg"",
                    ""episode"": [
                        ""https://rickandmortyapi.com/api/episode/1""
                    ],
                    ""url"": ""https://rickandmortyapi.com/api/character/1"",
                    ""created"": ""2017-11-04T18:48:46.250Z""
                },
                {
                    ""id"": 2,
                    ""name"": ""Morty Smith"",
                    ""status"": ""Alive"",
                    ""species"": ""Human"",
                    ""type"": """",
                    ""gender"": ""Male"",
                    ""origin"": {
                        ""name"": ""unknown"",
                        ""url"": """"
                    },
                    ""location"": {
                        ""name"": ""Citadel of Ricks"",
                        ""url"": ""https://rickandmortyapi.com/api/location/3""
                    },
                    ""image"": ""https://rickandmortyapi.com/api/character/avatar/2.jpeg"",
                    ""episode"": [
                        ""https://rickandmortyapi.com/api/episode/1""
                    ],
                    ""url"": ""https://rickandmortyapi.com/api/character/2"",
                    ""created"": ""2017-11-04T18:50:21.651Z""
                }
            ]
        }";
    }

    private void SetupMockHandler(string jsonResponse, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);
    }

    [TestMethod]
    public async Task GetCharactersVerifyRickExists()
    {
        // Arrange
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object);
        _service = new RickAndMortyCharacterService(_httpClient);
        SetupMockHandler(GetMockCharacterResponse());

        // Act
        var characters = await _service.GetCharactersAsync();

        // Assert
        Assert.IsTrue(characters.Count > 0);
        var rickSanchez = characters.FirstOrDefault(c => c.Name == "Rick Sanchez");
        Assert.IsNotNull(rickSanchez);
        Assert.AreEqual("Human", rickSanchez.Species);
        Assert.AreEqual(1, rickSanchez.Id);
    }

    [TestMethod]
    public async Task GetCharactersVerifyMortyExists()
    {
        // Arrange
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object);
        _service = new RickAndMortyCharacterService(_httpClient);
        SetupMockHandler(GetMockCharacterResponse());
        SetupMockHandler(GetMockCharacterResponse());

        // Act
        var characters = await _service.GetCharactersAsync();

        // Assert
        Assert.IsTrue(characters.Count >= 2);
        var mortySmith = characters.FirstOrDefault(c => c.Name == "Morty Smith");
        Assert.IsNotNull(mortySmith);
        Assert.AreEqual("Human", mortySmith.Species);
        Assert.AreEqual(2, mortySmith.Id);
    }

}
