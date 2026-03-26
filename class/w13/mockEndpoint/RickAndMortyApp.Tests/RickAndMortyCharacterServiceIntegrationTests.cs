namespace RickAndMortyApp.Tests;

[TestClass]
[TestCategory("Integration")]
public class RickAndMortyCharacterServiceIntegrationTests
{
    private RickAndMortyCharacterService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _service = new RickAndMortyCharacterService();
    }

    // Here we use  async Task instead of void because
    // we want to await the GetCharactersAsync method and ensure that the test waits for the asynchronous operation to complete before making assertions. This allows us to properly test the asynchronous behavior of the method and ensures that we are checking the results after the data has been retrieved.

    [TestMethod]
    public async Task GetCharactersVerifyRickExists()
    {
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
