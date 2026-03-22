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
    public void AddBook()
    {
        //Arrange
        var book = new Book("9781800260245", "The First Wall", "Gav Thorpe", 5);

        // Act
        _inventory?.AddBook(book);

        // Assert
        Assert.AreEqual(5, _inventory?.CheckStock("9781800260245"));
    }

    [TestMethod]
    public void RemoveBook()
    {
        //Arrange
        var book = new Book("9781789992908", "The Solar War", "John Fremch", 6);
        _inventory?.AddBook(book);

        // Act
        _inventory?.RemoveBook("9781789992908");

        // Assert
        Assert.AreEqual(5, _inventory?.CheckStock("9781789992908"));
    }

    [TestMethod]
    public void FindBook()
    {
        //Arrange
        var book = new Book("9781789999341", "The lost and the Damned", "Guy Haley", 10);
        _inventory?.AddBook(book);

        // Act
        var result = _inventory?.FindBookByTitle("The lost and the Damned");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("9781789999341", result.ISBN);
    }


}