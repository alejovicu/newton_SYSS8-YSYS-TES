using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bookstore;

namespace Bookstore.Tests;

[TestClass]
public class BookstoreInventoryTests
{
    private BookstoreInventory _inventory = null!;

    [TestInitialize]
    public void Setup()
    {
        _inventory = new BookstoreInventory();
    }

    [TestMethod]
    public void Unknown_ISBN_ReturnsZero()
    {
        // Arrange ???


        var stock = _inventory.CheckStock("999");
        Assert.AreEqual(0, stock);
    }

    [TestMethod]
    public void AddBook_NewISND_ReturnsTrue()
    {
        var book = new Book("123", "Anarchy for the masses", "Eugene DeAlore", 5);
        var result = _inventory.AddBook(book);

        Assert.IsTrue(result);
        Assert.AreEqual(5, _inventory.CheckStock("123"));
    }
    [TestMethod]
    public void CheckStock_ExistingIsbn_ReturnsStock()
    {
        _inventory.AddBook(new Book("123", "Anarchy for the masses", "Eugene DeAlore", 5));

        var stock = _inventory.CheckStock("123");

        Assert.AreEqual(5, stock);
    }

    [TestMethod]
    public void AddBook_ExistingISBN_ReturnsFalseAndRestocks()
    {
        _inventory.AddBook(new Book("123", "Anarchy for the masses", "Eugene DeAlore", 5));
        var result = _inventory.AddBook(new Book("123", "Anarchy for the masses", "Eugene DeAlore", 3));

        Assert.IsFalse(result);
        Assert.AreEqual(8, _inventory.CheckStock("123"));
    }

    [TestMethod]
    public void FindBookByTitle_ExactTitle_ReturnsBook()
    {
        _inventory.AddBook(new Book("123", "Anarchy for the masses", "Eugene DeAlore", 5));

        var found = _inventory.FindBookByTitle("Anarchy for the masses");

        Assert.IsNotNull(found);
        Assert.AreEqual("123", found.ISBN);
    }

    [TestMethod]
    public void FindBookByTitle_DifferentCase_ReturnsBook()
    {
        _inventory.AddBook(new Book("123", "Anarchy for the masses", "Eugene DeAlore", 5));
        var found = _inventory.FindBookByTitle("ANARCHY FOR THE MASSES");

        Assert.IsNotNull(found);
        Assert.AreEqual("Anarchy for the masses", found.Title);
    }

    [TestMethod]
    public void FindBookByTitle_UnknownTitle_ReturnsNull()
    {
        var found = _inventory.FindBookByTitle("Not Existing");

        Assert.IsNull(found);
    }

    [TestMethod]
    public void RemoveBook_ExistingIsbn_DecreasesStock()
    {
        _inventory.AddBook(new Book("123", "Anarchy for the masses", "Eugene DeAlore", 5));

        var result = _inventory.RemoveBook("123");

        Assert.IsTrue(result);
        Assert.AreEqual(4, _inventory.CheckStock("123"));
    }

    [TestMethod]
    public void RemoveBook_UnknownIsbn_ReturnsFalse()
    {
        var result = _inventory.RemoveBook("999");

        Assert.IsFalse(result);
    }
}