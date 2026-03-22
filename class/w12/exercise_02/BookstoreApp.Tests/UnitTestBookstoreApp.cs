using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookstoreApp;

namespace BookstoreApp.Tests;

[TestClass]
public class BookstoreInventoryTests
{
    private BookstoreInventory _inventory;

    // Runs before each test.
    [TestInitialize]
    public void Setup()
    {
        _inventory = new BookstoreInventory();
    }

    // Adding a new book should return true and set the correct stock.
    [TestMethod]
    public void AddBook_NewBook_AddsBookAndReturnsTrue()
    {
        // Arrange
        var book = new Book("1234", "SmallKnight", "F. Filip Monteza", 5);

        // Act
        var added = _inventory.AddBook(book);

        // Assert
        Assert.IsTrue(added, "AddBook should return true for a brand new ISBN.");
        Assert.AreEqual(5, _inventory.CheckStock("1234"), "Stock should match the amount added.");
    }

    // Removing a book should decrement stock by one.
    [TestMethod]
    public void RemoveBook_ExistingBook_DecrementsStock()
    {
        // Arrange
        var book = new Book("2345", "LittleHero", "A. Author", 2);
        _inventory.AddBook(book);

        // Act
        var removed = _inventory.RemoveBook(book.ISBN);

        // Assert
        Assert.IsTrue(removed, "RemoveBook should return true when the ISBN exists.");
        Assert.AreEqual(1, _inventory.CheckStock(book.ISBN), "Stock should drop by one after a remove.");
    }

    // Finding by title should return the book (case-insensitive).
    [TestMethod]
    public void FindBookByTitle_ExistingBook_ReturnsBook()
    {
        // Arrange
        var book = new Book("3456", "BraveChild", "B. Writer", 3);
        _inventory.AddBook(book);

        // Act
        var found = _inventory.FindBookByTitle("BraveChild");

        // Assert
        Assert.IsNotNull(found, "FindBookByTitle should return a Book when the title exists.");
        Assert.AreEqual("3456", found!.ISBN, "Found book should match the added book's ISBN.");
    }
}