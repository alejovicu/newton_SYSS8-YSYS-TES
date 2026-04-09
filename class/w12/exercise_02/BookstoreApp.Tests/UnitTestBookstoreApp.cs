namespace BookstoreApp.Tests;

[TestClass]
public class UnitTestBookstoreApp
{
    [TestMethod]
    public void TestAddBook()
    {
        // Arrange 
        var isbn = "123";
        var expectedStock = 10;
        var dotnetBook = new BookstoreApp.Book(isbn, "C# Deep Dive", "John Doe", expectedStock);
        var library = new BookstoreApp.BookstoreInventory();

        // Act
        library.AddBook(dotnetBook);


        // Assert
        var stock = library.CheckStock(isbn);
        Assert.AreEqual(expectedStock, stock);

    }
}