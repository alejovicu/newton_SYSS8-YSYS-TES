namespace CalculatorApp.Tests;

[TestClass]
public class UnitTestCalculatorSum
{
    [TestMethod]
    public void TestSumTwoPositiveNumbers()
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

    [TestMethod]
    public void TestSumOnePositiveOneNegativeNumber()
    {
        // Arrange
        var calculator = new Calculator();
        int a = 5;
        int b = -10;

        // Act
        int result = calculator.Sum(a, b);

        // Assert
        Assert.AreEqual(-5, result);
    }

    [TestMethod]
    public void TestSumTwoNegativeNumbers()
    {
        // Arrange
        var calculator = new Calculator();
        int a = -5;
        int b = -10;

        // Act
        int result = calculator.Sum(a, b);

        // Assert
        Assert.AreEqual(-15, result);
    }
}