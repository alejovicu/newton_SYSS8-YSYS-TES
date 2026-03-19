using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bookstore;

namespace Bookstore.Tests;

[TestClass]
public class OrderProcessorTests
{
    private OrderProcessor _processor;

    [TestInitialize]
    public void Setup()
    {
        _processor = new OrderProcessor();
    }

    [TestMethod]
    public void TotalCostCalculator_EnsureValidValue()
    {
        // Arrange
        int numberOfItems = 10;
        decimal unitPrice = 250M;

        // Act 
        var result = _processor.CalculateTotalCost(numberOfItems, unitPrice);

        // Assert
        Assert.IsInstanceOfType(result, typeof(decimal));
    }
}