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
        Book HarryPotter = new Book("1", "Harry Potter", "J.K. Rowling", 10);
        _inventory.AddBook(HarryPotter);
        Book WesternFront = new Book("2", "All Quiet On The Western Front", "Erich Maria Remarque", 2);
        _inventory.AddBook(WesternFront);

    }

    [TestMethod]
    public void CreateValidBook_ShouldBeTrue()
    {
        // Arrange
        var book = new Book("15353263", "BookTitle", "BookAuthor", 5);

        // Assert
        Assert.IsTrue(!String.IsNullOrWhiteSpace(book.ISBN));
        Assert.IsTrue(!String.IsNullOrWhiteSpace(book.Title));
        Assert.IsTrue(!String.IsNullOrWhiteSpace(book.Author));
        Assert.IsTrue(book.Stock != 0);
    }

    [TestMethod]
    public void AddBook_NewBookShouldCreateNewObject()
    {
        // Arrange
        Book test = new Book("3", "New book", "Author test", 1);

        // Act
        _inventory.AddBook(test);

        // Assert
        var result = _inventory.FindBookByTitle("New book");
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void AddBook_ExistingBookShouldAddStock()
    {
        // Arrange
        Book testInstanceBook = new Book("3", "New book", "Author test", 1);
        _inventory.AddBook(testInstanceBook);
        var initialStock = _inventory.CheckStock(testInstanceBook.ISBN);
        _inventory.AddBook(testInstanceBook);

        // Act
        var newStock = _inventory.CheckStock(testInstanceBook.ISBN);

        // Assert
        Assert.AreEqual(initialStock + 1, newStock);
    }

    [TestMethod]
    public void FindBookByTitle_IgnoresOrdinalCase()
    {
        // Arrange, Act
        var book = _inventory.FindBookByTitle("hArRy PoTtEr");
        var caseBook = _inventory.FindBookByTitle("Harry Potter");

        // Assert
        Assert.AreSame(book, caseBook);
    }

    [TestMethod]
    public void RemoveBook_RemovingUnexistingBook()
    {
        // Arrange, Act, Assert
        Assert.IsFalse(_inventory.RemoveBook("10"));
    }

    [TestMethod]
    public void RemoveBook_RemoveExistingBook()
    {
        // Arrange
        var initialStock = _inventory.CheckStock("1");

        // Assert
        Assert.IsTrue(_inventory.RemoveBook("1"));

        // Act
        var newStock = _inventory.CheckStock("1");
        // Assert
        Assert.AreEqual(newStock, initialStock -1);
    }
}