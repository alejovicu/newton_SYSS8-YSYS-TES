using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bookstore;

namespace Bookstore.Tests;

[TestClass]
public class UnitTest
{

    private BookstoreInventory? _inventory;

    // To avoid creating inventory in every test
    [TestInitialize]
    public void Setup()
    {
        _inventory = new BookstoreInventory();
    }

    [TestMethod]
    public void AddBook()
    {
        // Arrange - create the book
        var book = new Book("978-0261103573", "The Hobbit", "J.R.R. Tolkien", 10);

        // Act - Run method in the source code
        if (_inventory != null)
        {
            _inventory.AddBook(book);

            // Assert - check if it works
            Assert.AreEqual(10, _inventory.CheckStock("978-0261103573"));
        }
    }

    [TestMethod]
        public void RemoveBook()
        {
            // Arrange - Create and add book first
            var book = new Book("999", "Star Wars", "George Lucas", 5);
            _inventory?.AddBook(book);
            
            // Act - Remove created book. Find the book ISBN
            _inventory?.RemoveBook("999");

            // Assert - Check if the code works
            Assert.AreEqual(4, _inventory?.CheckStock("999"));
        }
    }

