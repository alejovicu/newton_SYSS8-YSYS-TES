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

    // ----- Tests for CheckStock  -----
    
    [TestMethod]
    public void CheckStock_NonExistentBook_ReturnsZero()
    {
        //Arrange
        var isbn = "999";

        //Act
        var stock = _inventory.CheckStock(isbn);

        //Assert
        Assert.AreEqual(0, stock);
    }

    [TestMethod] 
    public void CheckStock_ExistingBook_ReturnsStock()
    {
        //Arrange
        var book = new Book("123", "The Animal Farm", "George Orwell", 3);
        _inventory.AddBook(book);

        //Act
        var stock = _inventory.CheckStock("123");

        //Assert
        Assert.AreEqual(3, stock);
    }

    // ----- Tests for AddBook  -----
    [TestMethod]
    public void AddBook_NewBook_ReturnsTrueAndCorrectStock()
    {
        //Arrange
        var book = new Book("123", "The Animal Farm", "George Orwell", 3);

        //Act
        bool result = _inventory.AddBook(book);

        //Assert
        Assert.IsTrue(result);
        Assert.AreEqual(3, _inventory.CheckStock("123"));
    }

    [TestMethod]
    public void AddBook_ExistingBook_ReturnsFalseAndAddsStock()
    {
        //Arrange
        var book = new Book("123", "The Animal Farm", "George Orwell", 3);
        _inventory.AddBook(book);

        //Act
        bool result = _inventory.AddBook(new Book("123", "The Animal Farm", "George Orwell", 2));

        //Assert
        Assert.IsFalse(result);
        Assert.AreEqual(5, _inventory.CheckStock("123"));
    }

    // ----- Tests for RemoveBook  -----
    [TestMethod]
    public void RemoveBook_ExistingBook_ReturnsTrueAndCorrectStock()
    {
        //Arrange
        var book = new Book("123", "The Animal Farm", "George Orwell", 3);
        _inventory.AddBook(book);

        //Act
        bool result = _inventory.RemoveBook("123");

        //Assert
        Assert.IsTrue(result);
        Assert.AreEqual(2, _inventory.CheckStock("123"));
    }

    [TestMethod]
    public void RemoveBook_NonExistentBook_ReturnsFalse()
    {
        //Arrange
        string isbn = "999";

        //Act
        bool result = _inventory.RemoveBook(isbn);

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void RemoveBook_BookWithZeroStock_ReturnsFalseAndStockStaysZero()
    {
        //Arrange
        var book = new Book("123", "The Animal Farm", "George Orwell", 1);
        _inventory.AddBook(book);
        _inventory.RemoveBook("123");

        //Act
        bool result = _inventory.RemoveBook("123");

        //Assert
        Assert.IsFalse(result);
        Assert.AreEqual(0, _inventory.CheckStock("123"));
    }

    // ----- Tests for FindBookByTitle  -----
    [TestMethod]
    public void FindBookByTitle_ExistingTitle_ReturnsBook()
    {
        //Arrange
        var book = new Book("123", "The Animal Farm", "George Orwell", 3);
        _inventory.AddBook(book);

        //Act
        var result = _inventory.FindBookByTitle("The Animal Farm");

        //Assert
        Assert.AreEqual("123", result.ISBN);
        Assert.AreEqual("George Orwell", result.Author);
    }

    [TestMethod]
    public void FindBookByTitle_NonExistentTitle_ReturnsNull()
    {
        //Act
        var result = _inventory.FindBookByTitle("Nonexistent Book");

        //Assert
        Assert.IsNull(result);
    }

}