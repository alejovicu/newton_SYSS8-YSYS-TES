using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookstoreApp.Tests;

[TestClass]
public class InventorySpec
{
    private BookstoreApp.BookstoreInventory _shelf;

    [TestInitialize]
    public void BootUp()
    {
        _shelf = new BookstoreApp.BookstoreInventory();
    }

    [TestMethod]
    public void RegisterBook_FirstTime_ShouldSucceed()
    {
        // Arrange
        var idCode = "RRH001";
        var story = new BookstoreApp.Book(idCode, "Red Riding Hood Returns", "J.K Bowling", 8);

        // Act
        var success = _shelf.AddBook(story);
        var count = _shelf.CheckStock(idCode);

        // Assert
        Assert.IsTrue(success);
        Assert.AreEqual(8, count);
    }

    [TestMethod]
    public void RegisterBook_SecondTime_ShouldStackButFailFlag()
    {
        // Arrange
        var idCode = "RRH002";
        var tale = new BookstoreApp.Book(idCode, "Little Blue Cloak", "J.K Bowling", 2);

        // Act
        _shelf.AddBook(tale);
        var secondTry = _shelf.AddBook(tale);
        var count = _shelf.CheckStock(idCode);

        // Assert
        Assert.IsFalse(secondTry);
        Assert.AreEqual(4, count);
    }

    [TestMethod]
    public void RemoveCopy_ShouldDropInventoryByOne()
    {
        // Arrange
        var idCode = "RRH003";
        var weirdStory = new BookstoreApp.Book(idCode, "Wolf vs Grandma", "J.K Rolling-Stone", 5);

        _shelf.AddBook(weirdStory);

        // Act
        var removed = _shelf.RemoveBook(idCode);
        var count = _shelf.CheckStock(idCode);

        // Assert
        Assert.IsTrue(removed);
        Assert.AreEqual(4, count);
    }
}
