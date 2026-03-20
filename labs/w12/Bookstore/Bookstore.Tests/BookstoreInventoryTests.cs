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
    public void AddBook_NewBook_ReturnsTrueAndAddsBook()
    {
        // Arrange
        var inventory = new  BookstoreInventory();
        var book = new Book("1234", "SmallKnight", "F. Filip Monteza", 5);
        // Act
        var result = inventory.AddBook(book);
        
        // Assert
        Assert.IsTrue(true);
        Assert.AreEqual(5, inventory.CheckStock("1234"));
    }

    [TestMethod]
    public void RemoveBook_ExistingBook_ReturnsTrue()
    {
        // Arrange
        var inventory = new  BookstoreInventory();
        var book = new Book("1234", "SmallKnight", "F. Filip Monteza", 5);
        inventory.AddBook(book);
        
        // Act
        var removeBook = inventory.RemoveBook(book.ISBN);
        
        // Assert
        Assert.IsTrue(removeBook);
        Assert.AreEqual(0, inventory.CheckStock(book.ISBN));
    }

    [TestMethod]
    public void findBook_ExistingBook_ReturnsTrue()
    {
        // Arrange
        // Check if there is a book
        var bookstoreInventory =  new  BookstoreInventory();
        var book =  new Book("1234", "SmallKnight", "F. Filip Monteza", 5);
        bookstoreInventory.AddBook(book);
        
        // Act
        var result = bookstoreInventory.FindBookByTitle("SmallKnight");
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("1234", result.ISBN);
        
    }
    
    [TestMethod]
    public void CheckStock_ExistingBook_ReturnsTrue()
    {
        // Arrange
        var inventory = new  BookstoreInventory(); 
        var book = new Book("1234", "SmallKnight", "F. Filip Monteza", 5);
        inventory.AddBook(book);
        
        // Act
        var result = inventory.CheckStock("1234");
        
        // Assert
        Assert.AreEqual(5, result);
    }

}