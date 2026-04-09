using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bookstore;

namespace Bookstore.Tests;

[TestClass]
public class BookstoreInventoryTests
{
    private BookstoreInventory? _inventory;

    [TestInitialize]
    public void Setup()
    {
        _inventory = new BookstoreInventory();
    }
    
    [TestMethod]
    public void TestAddBook()
    {
        // Arrange 
        var isbn = "123";
        var expectedStock = 10;
        var dotnetBook = new Bookstore.Book(isbn, "C# Deep Dive", "John Doe", expectedStock);
        var library = new Bookstore.BookstoreInventory();

        // Act
        library.AddBook(dotnetBook);


        // Assert
        var stock = library.CheckStock(isbn);
        Assert.AreEqual(expectedStock, stock);

    }

    [TestMethod]
    public void TestRemoveOneBook()
    {
        // Arrange
        var isbn = "123";
        var currentStock = 10;
        var book = new Book(isbn, "C# Deep Dive", "John Doe", currentStock);
        _inventory?.AddBook(book);
        
        // Act
        _inventory?.RemoveBook(isbn);
        
        // Assert 
        var actualStock = _inventory.CheckStock(isbn);
        Assert.AreEqual(9, actualStock);
    }

    [TestMethod]
    public void TestFindBook()
    {
        // Arrange
        var title = "C# Deep Dive";
        _inventory.AddBook(new Book(title, "C# Deep Dive", "John Doe", 10));

        // Act
        var result = _inventory?.FindBookByTitle(title);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(title, result!.Title);
    }

}