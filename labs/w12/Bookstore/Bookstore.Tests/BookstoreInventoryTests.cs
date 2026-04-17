using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bookstore;

namespace Bookstore.Tests;

[TestClass]
public class BookstoreInventoryTests
{
    private BookstoreInventory _inventory;

    [TestInitialize]
    public void Setup()
    {
        _inventory = new BookstoreInventory();
    }

    [TestMethod]
    public void TestAddBook()
    {
        // Arrange
        var book = new Book("123", "ABC", "Author A", 3); // A book must exist to be added.

        // Act
        _inventory.AddBook(book); // A book needs a place to be put.

        var stock = _inventory.CheckStock("123"); // The "Bookshelf" should not be empty.

        // Assert
        Assert.AreEqual(3, stock); // Assert how many copies you have.
    }

    [TestMethod]
    public void TestFindBookByTitle()
    {
        // Arrange
        var book = new Book("12345", "Wild West", "Cowboy Cowly", 5); // A book must exist to be found.
        _inventory.AddBook(book); // A book needs to be put somewhere to be found later.

        // Act
        var bookSought = _inventory.FindBookByTitle("Wild West");

        // Assert
        Assert.AreEqual(book.Title, bookSought.Title); // Found the right book?

    }

    [TestMethod]
    public void TestCheckStockExist()
    {
        // Arrange
        var book = new Book("777", "Bible", "Apostles", 30000);
        _inventory.AddBook(book);

        // Act
        var stock = _inventory.CheckStock("777");

        // Assert
        Assert.AreEqual(30000, stock);
    }

    [TestMethod]
    public void TestCheckStockDoesNotExist()
    {
        // Arrange
        // A book does not need to exist to check if they exist.

        // Act
        var stock = _inventory.CheckStock("666");

        // Assert
        Assert.AreEqual(0, stock);
    }

    [TestMethod]
    public void TestRemoveBook()
    {
        // Arrange
        var book = new Book("3469", "Horror Show", "Bloody Author", 1); // A book must exist to be removed.
        _inventory.AddBook(book);

        // Act
        _inventory.RemoveBook("3469");

        var stock = _inventory.CheckStock("3469");

        // Assert
        Assert.AreEqual(0, stock);
    }

}