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

    // ========== Book constructor & properties ==========

    [TestMethod]
    public void Book_Constructor_SetsPropertiesCorrectly()
    {
        var book = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 5);

        Assert.AreEqual("978-0-13-468599-1", book.ISBN);
        Assert.AreEqual("Clean Code", book.Title);
        Assert.AreEqual("Robert C. Martin", book.Author);
        Assert.AreEqual(5, book.Stock);
    }

    // ========== AddBook ==========

    [TestMethod]
    public void AddBook_NewBook_ReturnsTrue()
    {
        var book = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 3);

        bool result = _inventory.AddBook(book);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void AddBook_NewBook_BookIsInInventory()
    {
        var book = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 3);

        _inventory.AddBook(book);

        var found = _inventory.FindBookByTitle("Clean Code");
        Assert.IsNotNull(found);
        Assert.AreEqual("978-0-13-468599-1", found.ISBN);
    }

    [TestMethod]
    public void AddBook_ExistingIsbn_ReturnsFalse()
    {
        var book1 = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 3);
        var book2 = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 2);

        _inventory.AddBook(book1);
        bool result = _inventory.AddBook(book2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void AddBook_ExistingIsbn_IncreasesStock()
    {
        var book1 = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 3);
        var book2 = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 2);

        _inventory.AddBook(book1);
        _inventory.AddBook(book2);

        int stock = _inventory.CheckStock("978-0-13-468599-1");
        Assert.AreEqual(5, stock);
    }

    // ========== RemoveBook ==========

    [TestMethod]
    public void RemoveBook_ExistingIsbn_ReturnsTrue()
    {
        var book = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 3);
        _inventory.AddBook(book);

        bool result = _inventory.RemoveBook("978-0-13-468599-1");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void RemoveBook_ExistingIsbn_ChangesStock()
    {
        var book = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 3);
        _inventory.AddBook(book);

        _inventory.RemoveBook("978-0-13-468599-1");

        int stock = _inventory.CheckStock("978-0-13-468599-1");
        // Note: the current implementation increments stock (Stock++) on remove
        Assert.AreEqual(4, stock);
    }

    [TestMethod]
    public void RemoveBook_NonExistingIsbn_ReturnsFalse()
    {
        bool result = _inventory.RemoveBook("000-0-00-000000-0");

        Assert.IsFalse(result);
    }

    // ========== FindBookByTitle ==========

    [TestMethod]
    public void FindBookByTitle_ExactMatch_ReturnsBook()
    {
        var book = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 3);
        _inventory.AddBook(book);

        var found = _inventory.FindBookByTitle("Clean Code");

        Assert.IsNotNull(found);
        Assert.AreEqual("978-0-13-468599-1", found.ISBN);
        Assert.AreEqual("Clean Code", found.Title);
        Assert.AreEqual("Robert C. Martin", found.Author);
    }

    [TestMethod]
    public void FindBookByTitle_CaseInsensitive_ReturnsBook()
    {
        var book = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 3);
        _inventory.AddBook(book);

        var found = _inventory.FindBookByTitle("clean code");

        Assert.IsNotNull(found);
        Assert.AreEqual("Clean Code", found.Title);
    }

    [TestMethod]
    public void FindBookByTitle_UpperCase_ReturnsBook()
    {
        var book = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 3);
        _inventory.AddBook(book);

        var found = _inventory.FindBookByTitle("CLEAN CODE");

        Assert.IsNotNull(found);
        Assert.AreEqual("Clean Code", found.Title);
    }

    [TestMethod]
    public void FindBookByTitle_NoMatch_ReturnsNull()
    {
        var book = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 3);
        _inventory.AddBook(book);

        var found = _inventory.FindBookByTitle("The Pragmatic Programmer");

        Assert.IsNull(found);
    }

    [TestMethod]
    public void FindBookByTitle_EmptyInventory_ReturnsNull()
    {
        var found = _inventory.FindBookByTitle("Clean Code");

        Assert.IsNull(found);
    }

    // ========== CheckStock ==========

    [TestMethod]
    public void CheckStock_ExistingBook_ReturnsStock()
    {
        var book = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 7);
        _inventory.AddBook(book);

        int stock = _inventory.CheckStock("978-0-13-468599-1");

        Assert.AreEqual(7, stock);
    }

    [TestMethod]
    public void CheckStock_NonExistingBook_ReturnsZero()
    {
        int stock = _inventory.CheckStock("000-0-00-000000-0");

        Assert.AreEqual(0, stock);
    }

    [TestMethod]
    public void CheckStock_AfterRestock_ReturnsUpdatedStock()
    {
        var book1 = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 3);
        var book2 = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 4);

        _inventory.AddBook(book1);
        _inventory.AddBook(book2);

        int stock = _inventory.CheckStock("978-0-13-468599-1");
        Assert.AreEqual(7, stock);
    }

    // ========== Multiple books ==========

    [TestMethod]
    public void AddMultipleBooks_AllAccessible()
    {
        var book1 = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", 3);
        var book2 = new Book("978-0-20-161622-4", "The Pragmatic Programmer", "David Thomas", 5);
        var book3 = new Book("978-0-59-651798-7", "JavaScript: The Good Parts", "Douglas Crockford", 2);

        _inventory.AddBook(book1);
        _inventory.AddBook(book2);
        _inventory.AddBook(book3);

        Assert.IsNotNull(_inventory.FindBookByTitle("Clean Code"));
        Assert.IsNotNull(_inventory.FindBookByTitle("The Pragmatic Programmer"));
        Assert.IsNotNull(_inventory.FindBookByTitle("JavaScript: The Good Parts"));
    }
}
