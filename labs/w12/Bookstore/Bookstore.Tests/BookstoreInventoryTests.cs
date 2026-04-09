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
    public void AddBookTest()
    {
        // Arrange
        var book = new Book("1234", "Test Title", "Test Author", 10);

        // Act
        var result = _inventory.AddBook(book);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(10, _inventory.CheckStock("1234"));
    }

    [TestMethod]
    public void AddBookThatAlreadyExistsTest()
    {
        // Arrange
        var book1 = new Book("1234", "Test Title", "Test Author", 10);
        var book2 = new Book("1234", "Test Title", "Test Author", 2);

        _inventory.AddBook(book1);

        // Act
        var result = _inventory.AddBook(book2);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(12, _inventory.CheckStock("1234"));
    }

    [TestMethod]
    public void DeleteBookTestWhenBookExists()
    {
        // Arrange
        var book = new Book("1234", "Test Title", "Test Author", 10);
        _inventory.AddBook(book);

        // Act
        var result = _inventory.RemoveBook("1234");

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(9, _inventory.CheckStock("1234"));
    }

    [TestMethod]
    public void DeleteBookUntilStockZeroTest()
    {
        // Arrange
        var book = new Book("1234", "Test Title", "Test Author", 0);
        _inventory.AddBook(book);

        // Act
        var result = _inventory.RemoveBook("1234");

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(0, _inventory.CheckStock("1234"));
    }

    [TestMethod]
    public void DeleteBookTestWhenBookDoesNotExist()
    {
        // Act
        var result = _inventory.RemoveBook("5678");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void FindBookByTitleMatchTest()
    {
        // Arrange
        var book = new Book("1234", "Test Title", "Test Author", 10);
        _inventory.AddBook(book);

        // Act
        var result = _inventory.FindBookByTitle("Test Title");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("1234", result.ISBN);
    }

    [TestMethod]
    public void FindBookWhenNotFoundTest()
    {
        // Act
        var result = _inventory.FindBookByTitle("Test Title NON EXISTING");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindBookByTitleCaseInsensitiveTest()
    {
        // Arrange
        var book = new Book("1234", "Test Title", "Test Author", 10);
        _inventory.AddBook(book);

        // Act
        var result = _inventory.FindBookByTitle("test title");

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void CheckStockTestWhenBookDoesNotExist()
    {
        // Act
        var stock = _inventory.CheckStock("5678");

        // Assert
        Assert.AreEqual(0, stock);
    }

    [TestMethod]
    public void CheckStockTestWhenBookExists()
    {
        // Arrange
        var book = new Book("1234", "Test Title", "Test Author", 10);
        _inventory.AddBook(book);

        // Act
        var result = _inventory.CheckStock("1234");

        // Assert
        Assert.AreEqual(10, result);
    }
}