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
    public void AddBook_NewBook_AddsSuccessfully()
    {
        // Arrange
        var isbn = "1234567890";
        var expectedStock = 10;
        var book = new Book(isbn, "Test Book", "Test Author", expectedStock);

        // Act
        _inventory.AddBook(book);

        // Assert
        var stock = _inventory.CheckStock(isbn);
        Assert.AreEqual(expectedStock, stock);
    }
    [TestMethod]
    public void RemoveBook_WhenBookExists_DecreasesStockByOne()
    {
        // Arrange
        var isbn = "999";
        var initialStock = 5;

        _inventory.AddBook(new Book(isbn, "Clean Code", "Robert Martin", 5));

        // Act
        bool wasRemoved = _inventory.RemoveBook(isbn);
        var remainingStock = _inventory.CheckStock(isbn);
        // Assert
        Assert.IsTrue(wasRemoved);
        Assert.AreEqual(initialStock - 1, remainingStock);
    }

    [TestMethod]
    public void RemoveBook_NonExistentBook_ReturnsFalse()
    {
        bool result = _inventory.RemoveBook("NOT-IN-LIST");

        Assert.IsFalse(result);
    }
    [TestMethod]
    public void FindBookByTitle_CaseInsensitive_ReturnsBook()
    {
        _inventory.AddBook(new Book("111", "C# Guide", "Author", 1));

        var found = _inventory.FindBookByTitle("c# guide");

        Assert.IsNotNull(found);
        Assert.AreEqual("111", found.ISBN);
    }


}