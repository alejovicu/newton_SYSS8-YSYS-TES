
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
    public void Test1()
    {
        //Implement tests
        Assert.IsTrue(true);
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


}