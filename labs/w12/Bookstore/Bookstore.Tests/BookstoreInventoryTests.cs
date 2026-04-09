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
    public void AddBook_NewBook_ShouldReturnTrue()
    {
        // Arrange
        var book = new Book("123", "Test Book", "Author", 5);

        var result = _inventory.AddBook(book);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void RemoveBook_ExistingBook_ShouldReturnTrueAndDecreaseStock()
    {
        // Arrange
        var book = new Book("123", "Test Book", "Author", 5);
        _inventory.AddBook(book);
        
        // Act
        var result = _inventory.RemoveBook("123");
        var stock = _inventory.CheckStock("123");
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(4, stock);
    }

    [TestMethod]
    public void RemoveBook_NonExistingBook_ShouldReturnFalse()
    {
        // Act
        var result = _inventory.RemoveBook("999");
        
        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void FindBookByTitle_ExistingBook_ShouldReturnBook()
    {
        // Arrange 
        var book = new Book("123", "Test Book", "Author", 5);
        _inventory.AddBook(book);
        
        // Act
        var result = _inventory.FindBookByTitle("Test Book");
            
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("123", result.ISBN);
    }

    [TestMethod]
    public void FindBookByTitle_NonExistingBook_ShouldReturnNull()
    {
        // Act
        var result = _inventory.FindBookByTitle("Unknown");
        
        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void CheckStock_ExistingBook_ShouldReturnCorrectStock()
    {
        // Arrange
        var book = new Book("123", "Test Book", "Author", 5);
        _inventory.AddBook(book);
        
        // Act
        var stock = _inventory.CheckStock("123");
        
        // Assert
        Assert.AreEqual(5, stock);
    }

    [TestMethod]
    public void CheckStock_NonExistingBook_ShouldReturnZero()
    {
        // Act
        var stock = _inventory.CheckStock("999");
        
        // Assert
        Assert.AreEqual(0, stock);
    }
    
}