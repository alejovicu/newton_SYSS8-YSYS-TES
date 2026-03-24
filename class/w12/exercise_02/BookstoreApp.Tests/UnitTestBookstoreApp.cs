namespace BookstoreApp.Tests;

[TestClass]
public class UnitTestBookstoreApp
{
    [TestMethod]
    public void TestMethod1()
    {
        //Arrange
        var isbn = "123";
        var exceptedStock = 5;
        var dotnetBook = new BookstoreApp.Book(isbn, "C# Deep Dive", "Jon Skeet", exceptedStock);
        //Shelf --> Library or Book Inventory 
        var library = new BookstoreApp.BookstoreInventory();

        //Act 
        //Add the book to the library 
        library.AddBook(dotnetBook);

        //Assert 
        var stock = library.CheckStock(isbn);

        Assert.AreEqual(exceptedStock, stock);
    }
}