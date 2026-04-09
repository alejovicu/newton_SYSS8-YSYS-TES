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
    public void AddBookToInventory()
    {
        var isbn = "1234";
        var expectedStock = 10;
        
        var book = new Bookstore.Book("1234", "Harry Potter", "J.K Rowling", 10);

        var _inventory = new Bookstore.BookstoreInventory();
        
        _inventory.AddBook(book);

        var stock = _inventory.CheckStock(isbn);
        Assert.AreEqual(expectedStock, stock);
    }

    [TestMethod]
    public void RemoveBookFromInventory()
    {
        var isbn = "1234";
        var _inventory = new Bookstore.BookstoreInventory();
        var book = new Bookstore.Book("1234", "Harry Potter", "J.K Rowling", 10);
        
        _inventory.AddBook(book);
        _inventory.RemoveBook(isbn);
        
        var stock = _inventory.CheckStock(isbn);
        Assert.AreEqual(9, stock);
    }

    [TestMethod]
    public void FindBookInInventoryByTitle()
    {
        var title = "Harry Potter";
        
        var book = new Bookstore.Book("1234", "Harry Potter", "J.K Rowling", 10);
        var _inventory = new Bookstore.BookstoreInventory();
        
        _inventory.AddBook(book);
        var result = _inventory.FindBookByTitle(title);
        
        Assert.IsNotNull(result);
        Assert.AreEqual("Harry Potter", result.Title);
    }
    
    [TestMethod]
    public void CheckBookStockInInventory()
    {
        var isbn = "1234";
        var expectedStock = 10;
        var book = new Bookstore.Book("1234", "Harry Potter", "J.K Rowling", expectedStock);
        var _inventory = new Bookstore.BookstoreInventory();
        
        _inventory.AddBook(book);
        var actualStock = _inventory.CheckStock(isbn);
        
        Assert.AreEqual(10,actualStock);
    }
}