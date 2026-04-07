namespace ShoppingCartAppIntegration.Tests;

using System.Net.Http;
using System.Net.Http.Json;

[TestClass]
public class User
{
    private static readonly HttpClient client = new HttpClient();


    [TestMethod]
    public void RegisterNewCustomer()
    {
        // Hint: Use appUrl from GlobalContext to make API calls to the application
        // GlobalContext.appUrl


        //Implement tests
        Assert.IsTrue(false, "Test not implemented yet.");
    }

    [TestMethod]
    public void CustomerListsProductsInCart()
    {
        //Implement tests
        Assert.IsTrue(false, "Test not implemented yet.");
    }
}
