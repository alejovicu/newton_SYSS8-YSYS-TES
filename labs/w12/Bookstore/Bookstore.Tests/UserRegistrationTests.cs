using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bookstore;

namespace Bookstore.Tests;

[TestClass]
public class UserRegistrationTests
{
    private UserRegistration _register;

    [TestInitialize]
    public void Setup()
    {
        _register = new UserRegistration();
    }

    [TestMethod]
    public void RegisterNewUser_ShouldReturnTrue()
    {
        // Arrange
        string username = "Gustav";

        // Act 
        var result = _register.RegisterUser(username);

        // Assert
        Assert.IsTrue(result);
    }
}