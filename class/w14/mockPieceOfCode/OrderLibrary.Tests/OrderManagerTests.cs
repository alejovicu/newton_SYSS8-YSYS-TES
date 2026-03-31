using Moq;
using OrderLibrary;

namespace OrderLibrary.Tests;

[TestClass]
public class OrderManagerTests
{


    [TestMethod]
    public void OrderManager_CalculateTotal_realImplementation_noDiscount()
    {
        // Arrange
        var subtotal = 100m;
        var expectedTotal= 100m;
        var calculator = new OrderManager();

        // Act
        var total = calculator.CalculateTotal(subtotal, "REGULAR");

        // Assert
        Assert.AreEqual(expectedTotal, total);
    }


    [TestMethod]
    public void OrderManager_CalculateTotal_realImplementation_studentDiscount()
    {
        // Arrange
        var subtotal = 100m;
        var expectedTotal= 90m;

        var calculator = new OrderManager();

        // Act
        var total = calculator.CalculateTotal(subtotal, "STUDENT");

        // Assert
        Assert.AreEqual( expectedTotal, total);
    }


    // Just an example to show how to use a mock 
    // to test the OrderManager with a mocked ILoyaltyDiscountService.
    [TestMethod]
    public void OrderManager_CalculateTotal_mocked()
    {
        // Arrange
        var subtotal = 100m;
        var expectedTotal= 80m;
    
       // For any call, It will return 0.20m as the discount rate, which simulates a VIP customer.
        var discountServiceMock = new Mock<ILoyaltyDiscountService>();
        discountServiceMock
            .Setup(service => service.GetDiscountRate("VIP"))
            .Returns(0.20m);

        var calculator = new OrderManager(discountServiceMock.Object);

        // Act
        var total = calculator.CalculateTotal(subtotal, "VIP");

        // Assert
        Assert.AreEqual(expectedTotal, total);
        discountServiceMock.Verify(service => service.GetDiscountRate("VIP"), Times.Once);
    }
}
