
namespace Bookstore.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bookstore;


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
    public void AddBookNewBookReturnsTrue()
    {
        // Arrange
        var book = new Book("123", "Clean Code", "Robert Martin", 5);

        // Act
        bool result = _inventory.AddBook(book);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(5, _inventory.CheckStock("123"));
    }

 [TestMethod]
    public void AddBookExistingBookIncreasesStock()
    {
        // Arrange
        var book1 = new Book("123", "Clean Code", "Robert Martin", 5);
        var book2 = new Book("123", "Clean Code", "Robert Martin", 3);

        _inventory.AddBook(book1);

        // Act
        bool result = _inventory.AddBook(book2);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(8, _inventory.CheckStock("123"));
    }

[TestMethod]
    public void RemoveBookBookExistsReturnsTrue()
    {
        // Arrange
        var book = new Book("111", "C# Basics", "Alice", 3);
        _inventory.AddBook(book);

        // Act
        bool result = _inventory.RemoveBook("111");

        // Assert
        Assert.IsTrue(result);
    }
[TestMethod]
public void TestRemoveBookDecreasesStockByOne()
{
    // Arrange
    var inventory = new BookstoreInventory();
    var book = new Book("222", "Algorithms", "Bob", 4);
    inventory.AddBook(book);

    // Act
    inventory.RemoveBook("222");
    int stock = inventory.CheckStock("222");

    // Assert
    Assert.AreEqual(3, stock);
}
//22

 [TestMethod]
    public void RemoveBook_BookDoesNotExist_ReturnsFalse()
    {
        // Act
        bool result = _inventory.RemoveBook("999");

        // Assert
        Assert.IsFalse(result);
    }
[TestMethod]
    public void FindBookByTitle_ExactTitle_ReturnsBook()
    {
        // Arrange
        var book = new Book("222", "Harry Potter", "Rowling", 10);
        _inventory.AddBook(book);

        // Act
        var result = _inventory.FindBookByTitle("Harry Potter");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("222", result.ISBN);
    }

    [TestMethod]
    public void FindBookByTitle_CaseInsensitive_ReturnsBook()
    {
        // Arrange
        var book = new Book("222", "Harry Potter", "Rowling", 10);
        _inventory.AddBook(book);

        // Act
        var result = _inventory.FindBookByTitle("harry potter");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Harry Potter", result.Title);
    }

    [TestMethod]
    public void FindBookByTitle_BookDoesNotExist_ReturnsNull()
    {
        // Act
        var result = _inventory.FindBookByTitle("Unknown Book");

        // Assert
        Assert.IsNull(result);
    }

 [TestMethod]
    public void CheckStock_ExistingBook_ReturnsCorrectStock()
    {
        // Arrange
        var book = new Book("333", "Algorithms", "Bob", 7);
        _inventory.AddBook(book);

        // Act
        int stock = _inventory.CheckStock("333");

        // Assert
        Assert.AreEqual(7, stock);
    }

    [TestMethod]
    public void CheckStock_BookDoesNotExist_ReturnsZero()
    {
        // Act
        int stock = _inventory.CheckStock("000");

        // Assert
        Assert.AreEqual(0, stock);
    }
}