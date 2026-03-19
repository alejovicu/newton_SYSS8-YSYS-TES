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

    [TestMethod]
    public void Test1()
    {
        //Implement tests
        Assert.IsTrue(true);
    }

}