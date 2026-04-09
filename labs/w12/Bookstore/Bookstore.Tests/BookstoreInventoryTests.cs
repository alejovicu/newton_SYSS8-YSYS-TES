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

    // ----- addbook tests -----

    [TestMethod]
    public void AddBook_NewBook_ReturnsTrue()
    {
        //arrange
        var book = new Book("101", "Harry Potter", "J.K. Rowling", 4);

        //act
        bool result = _inventory.AddBook(book);

        //assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void AddBook_ExistingBook_ReturnsFalse()
    {
        //arrange
        var book = new Book("101", "Harry Potter", "J.K. Rowling", 4);
        _inventory.AddBook(book);

        //act
        bool result = _inventory.AddBook(new Book("101", "Harry Potter", "J.K. Rowling", 2));

        //assert
        Assert.IsFalse(result);
    }

    // ----- removebook tests -----

    [TestMethod]
    public void RemoveBook_ExistingBook_ReturnsTrue()
    {
        //arrange
        var book = new Book("101", "Harry Potter", "J.K. Rowling", 4);
        _inventory.AddBook(book);

        //act
        bool result = _inventory.RemoveBook("101");

        //assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void RemoveBook_NonExistentBook_ReturnsFalse()
    {
        //arrange
        string isbn = "999";

        //act
        bool result = _inventory.RemoveBook(isbn);

        //assert
        Assert.IsFalse(result);
    }

    // ----- findbytitle tests -----

    [TestMethod]
    public void FindBookByTitle_ExistingTitle_ReturnsBook()
    {
        //arrange
        var book = new Book("101", "Harry Potter", "J.K. Rowling", 4);
        _inventory.AddBook(book);

        //act
        var result = _inventory.FindBookByTitle("harry potter");

        //assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void FindBookByTitle_NonExistentTitle_ReturnsNull()
    {
        //act
        var result = _inventory.FindBookByTitle("unknown book");

        //assert
        Assert.IsNull(result);
    }

    // ----- checkstock tests -----

    [TestMethod]
    public void CheckStock_ExistingBook_ReturnsStock()
    {
        //arrange
        var book = new Book("101", "Harry Potter", "J.K. Rowling", 4);
        _inventory.AddBook(book);

        //act
        int stock = _inventory.CheckStock("101");

        //assert
        Assert.AreEqual(4, stock);
    }

    [TestMethod]
    public void CheckStock_NonExistentBook_ReturnsZero()
    {
        //act
        int stock = _inventory.CheckStock("999");

        //assert
        Assert.AreEqual(0, stock);
    }

}
