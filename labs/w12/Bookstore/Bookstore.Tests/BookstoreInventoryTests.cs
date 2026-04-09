using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bookstore;

namespace Bookstore.Tests;

[TestClass]
public class BookstoreInventoryTests
{    
    [TestMethod]
    public void TestAddNewBook()
    {
        //Arrange
        var isbn = "1234567890";
        var expectedStock = 10;
        var dotnetBook = new Bookstore.Book(isbn, "Test Book", "Test Author", expectedStock);
        var library = new Bookstore.BookstoreInventory();

        //Act
        library.AddBook(dotnetBook);

        //Assert
        var stock = library.CheckStock(isbn);
        Assert.AreEqual(expectedStock, stock);
    }

    [TestMethod]
    public void TestFindBookByTitle()
    {
        // Arrange
        var title = "Test Book";
        var dotnetBook = new Bookstore.Book("1234567890", title, "Test Author", 10);
        var library = new Bookstore.BookstoreInventory();
        library.AddBook(dotnetBook);

        // Act
        var foundBook = library.FindBookByTitle(title);

        // Assert
        Assert.IsNotNull(foundBook);
        Assert.AreEqual(title, foundBook.Title);
    }

    [TestMethod]
    public void TestRemoveOneBookFromStock()
    {
        // Arrange
        var isbn = "1234567890";
        var initialStock = 10;
        var dotnetBook = new Bookstore.Book(isbn, "Test Book", "Test Author", initialStock);
        var library = new Bookstore.BookstoreInventory();
        library.AddBook(dotnetBook);

        // Act
        var removed = library.RemoveBook(isbn);

        // Assert
        Assert.IsTrue(removed);
        var stock = library.CheckStock(isbn);
        Assert.AreEqual(initialStock - 1, stock);
    }

    [TestMethod]
    public void TestFindBookByTitle_IfNotExistReturnNull()
    {
        // Arrange
        var library = new Bookstore.BookstoreInventory();

        // Act
        var result = library.FindBookByTitle("Book does not exist");

        // Assert
        Assert.IsNull(result);
    }

}