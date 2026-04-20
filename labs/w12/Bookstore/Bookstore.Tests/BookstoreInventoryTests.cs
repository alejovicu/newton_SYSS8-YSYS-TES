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
        // --- Arrange ---
        // I need a book
        var isbn = "123";
        var expectedStock = 5;
        var bestBook = new Bookstore.Book(isbn, "The Best of the Books", "Jane Doe", expectedStock);

        // --- Act ---
        // Add the book to the library
        _inventory.AddBook(bestBook);

        // --- Assert ---
        // Library should have book in it's inventory
        var stock = _inventory.CheckStock(isbn);

        Assert.AreEqual(expectedStock, stock);
    }

    [TestMethod]
    public void TestRemoveBook()
    {
        // --- Arrange ---
        // I need a book
        var isbn = "321";
        var initialStock = 5;
        var worstBook = new Bookstore.Book(isbn, "The Worst of the Books", "John Doe", initialStock);

        // --- Act ---
        // Add book
        _inventory.AddBook(worstBook);

        // Remove book
        var remove = _inventory.FindBookByTitle("The Worst of the Books");
        _inventory.RemoveBook(remove.ISBN);

        // --- Assert ---
        Assert.AreEqual(initialStock - 1, remove.Stock);
    }
}