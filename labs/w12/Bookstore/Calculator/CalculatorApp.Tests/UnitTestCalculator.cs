namespace CalculatorApp.Tests;

[TestClass]
public class UnitTestCalculator
{
    [TestMethod]
    public void TestSum()
    {
        // Arrange
        var calculator = new Calculator();
        int a = 5;
        int b = 10;

        // Act
        int result = calculator.Sum(a, b);

        // Assert
        Assert.AreEqual(15, result);
    }
}
