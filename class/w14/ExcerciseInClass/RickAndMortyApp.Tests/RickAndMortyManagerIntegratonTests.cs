using System.Data;

namespace RickAndMortyApp.Tests;

[TestClass]
[TestCategory("Integration")]
public sealed class RickAndMortyManagerIntegratonTests
{
    [TestMethod]
    [DataRow("Rick Sanchez", 1)]
    [DataRow("Morty Smith", 2)]
    [DataRow("Alien Rick", 15)]
    public void TestGetCharacterID_ReturnsCorrectID(string characterName, int expectedId)
    {
        // Arrange
        var manager = new RickAndMortyManager();

        // Act
        var characterId = manager.GetCharacterID(characterName).Result;

        // Assert
        Assert.AreEqual(expectedId, characterId);
    }


    [TestMethod]
    [DataRow("Pripudlian", new string[] { "S01E01", "S01E11", "S02E08", "S03E04" })]
    public void TestGetEpisodesWhereCharacterIsPresent_ReturnsCorrectEpisodes(string characterName, string[] expectedEpisodes)
    {
        // Arrange
        var manager = new RickAndMortyManager();

        // Act
        var episodes = manager.GetEpisodesWhereCharacterIsPresent(characterName).Result;

        // Assert
        CollectionAssert.AreEqual(expectedEpisodes, episodes);
    }

}
