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
    public void TestAddNewBookToInventory()
    {
        // Arrange
        var isbn = "123456";
        var expectedStock = 5;
        var potterBook = new Book(isbn, "Harry Potter", "JK Rowling", expectedStock);        

        // Act
        _inventory.AddBook(potterBook);

        // Assert
        var stock = _inventory.CheckStock(isbn);
        Assert.AreEqual(expectedStock, stock);
    }

    [TestMethod]
    public void TestRemoveBook()
    {
        // Arrange
        // First add a book
        var isbn = "123456";        
        var potterBook = new Book(isbn, "Harry Potter", "JK Rowling", 5);
        _inventory.AddBook(potterBook);

        // Then remove the book
        var bookToRemove = _inventory.FindBookByTitle("Harry Potter");
        var expectedStock = 4;

        // Act
        _inventory.RemoveBook(bookToRemove.ISBN);

        // Assert
        Assert.AreEqual(expectedStock, bookToRemove.Stock);
    }

    [TestMethod]
    public void TestAddStockTooExistingBook()
    {
        // Arrange
        // Add new boook
        var isbn = "123456";        
        var potterBook = new Book(isbn, "Harry Potter", "JK Rowling", 5);
        _inventory.AddBook(potterBook);

        // Adding stock to book
        var newStock = new Book(isbn, "Harry Potter", "JK Rowling", 3);
        var newExpectedStock = 8;

        // Act
        _inventory.AddBook(newStock);

        // Assert
        Assert.AreEqual(newExpectedStock, potterBook.Stock);
    }
}