
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bookstore;

namespace Bookstore.Tests;

[TestClass]
public class BookTests
{
    
    [TestMethod]
    public void TestBookConstructorSetsPropertiesCorrectly()
    {
        // Arrange
        string isbn = "123";
        string title = "Clean Code";
        string author = "Robert Martin";
        int stock = 5;

        // Act
        var book = new Book(isbn, title, author, stock);

        // Assert
        Assert.AreEqual("123", book.ISBN);
        Assert.AreEqual("Clean Code", book.Title);
        Assert.AreEqual("Robert Martin", book.Author);
        Assert.AreEqual(5, book.Stock);
    }
    
}