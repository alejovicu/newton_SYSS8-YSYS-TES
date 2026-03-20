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
    public void TestAddBook()
    {
        // arrange 
        var testbook = new Book("001", "Percy Jackson", "Rick Riordan", 12);
        
        // act
        _inventory.AddBook(testbook);
        
        // assert
        var stock = _inventory.CheckStock("001");
        Assert.AreEqual(12, stock);
    }

    [TestMethod]
    public void TestRemoveBook()
    {
        // arrange 
        var onepiece = new Book("002", "One Piece", "Eiichiro Oda", 7); // add book to remove
        _inventory.AddBook(onepiece);
        
        var booktoremove = _inventory.FindBookByTitle("One Piece");
        Assert.IsNotNull(booktoremove);
        var newStock = 6;
        
        // act
        _inventory.RemoveBook(booktoremove.ISBN); //remove book
       
        // assert
        Assert.AreEqual(newStock, booktoremove.Stock); //check that book is removed
    }

    [TestMethod]
    public void TestCheckStockNotExists()
    {
        // arrange 
        // no book created
        
        // act
        
        // assert
        var stock = _inventory.CheckStock("000");
        Assert.AreEqual(0, stock);
    }

}