namespace BookstoreApp.Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void AddBook_NewIsbn_AddsBookAndReturnsTrue()
    {
        var inventory = new BookstoreInventory();
        var book = new Book("123", "Clean Code", "Robert C. Martin", 5);

        var wasAdded = inventory.AddBook(book);

        Assert.IsTrue(wasAdded);
        Assert.AreEqual(5, inventory.CheckStock("123"));
    }

    [TestMethod]
    public void AddBook_ExistingIsbn_RestocksAndReturnsFalse()
    {
        var inventory = new BookstoreInventory();
        inventory.AddBook(new Book("123", "Clean Code", "Robert C. Martin", 2));

        var wasAdded = inventory.AddBook(new Book("123", "Clean Code", "Robert C. Martin", 3));

        Assert.IsFalse(wasAdded);
        Assert.AreEqual(5, inventory.CheckStock("123"));
    }

    [TestMethod]
    public void RemoveBook_ExistingIsbn_DecrementsStockAndReturnsTrue()
    {
        var inventory = new BookstoreInventory();
        inventory.AddBook(new Book("123", "Clean Code", "Robert C. Martin", 2));

        var removed = inventory.RemoveBook("123");

        Assert.IsTrue(removed);
        Assert.AreEqual(1, inventory.CheckStock("123"));
    }

    [TestMethod]
    public void RemoveBook_UnknownIsbn_ReturnsFalse()
    {
        var inventory = new BookstoreInventory();

        var removed = inventory.RemoveBook("missing");

        Assert.IsFalse(removed);
    }

    [TestMethod]
    public void FindBookByTitle_IsCaseInsensitive_ReturnsMatchingBook()
    {
        var inventory = new BookstoreInventory();
        inventory.AddBook(new Book("123", "Clean Code", "Robert C. Martin", 1));

        var found = inventory.FindBookByTitle("cLeAn cOdE");

        Assert.IsNotNull(found);
        Assert.AreEqual("123", found.ISBN);
    }

    [TestMethod]
    public void FindBookByTitle_UnknownTitle_ReturnsNull()
    {
        var inventory = new BookstoreInventory();
        inventory.AddBook(new Book("123", "Clean Code", "Robert C. Martin", 1));

        var found = inventory.FindBookByTitle("Unknown");

        Assert.IsNull(found);
    }

    [TestMethod]
    public void CheckStock_UnknownIsbn_ReturnsZero()
    {
        var inventory = new BookstoreInventory();

        Assert.AreEqual(0, inventory.CheckStock("missing"));
    }
}