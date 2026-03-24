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
    public void AddNewBookTest()
    {
        // Arrange - Create a book.
        var isbn = "123";
        var book = new Book(isbn , "Pippi Långstrump", "Astrid Lindgren", 5);

        // Act - Add new book to inventory.
        var result = _inventory.AddBook(book);
        var stock = _inventory.CheckStock(isbn);

        // Assert - Check that add succeeded and stock is correct.
        Assert.IsTrue(result);
        Assert.AreEqual(5, stock);
    }

    [TestMethod]
    public void AddExistingBookTest()
    {
        // Arrange - Create a book.
        var isbn = "123";
        var book = new Book(isbn, "Pippi Långstrump", "Astrid Lindgren", 5);

        // Act - Creating a scenario where the book already exists and gets added again.
        _inventory.AddBook(book);
        var result = _inventory.AddBook(book);
        var stock = _inventory.CheckStock(isbn);

        // Assert - Check that second add fails and stock increased.
        Assert.IsFalse(result);
        Assert.AreEqual(10, stock);
    }

    [TestMethod]
    public void RemoveBookTest()
    {
        // Arrange - Create a book and add it to inventory.
        var isbn = "123";
        var book = new Book(isbn, "Pippi Långstrump", "Astrid Lindgren", 5);

        _inventory.AddBook(book);

        // Act - Removing the book.
        var result = _inventory.RemoveBook(isbn);
        var stock = _inventory.CheckStock(isbn);

        // Assert - Check that removal was successful and stock decreased.
        Assert.IsTrue(result);
        Assert.AreEqual(4, stock);
    }

}