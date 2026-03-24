namespace CalculatorApp.Tests;

[TestClass]
public class UnitTestCalculatorDivision
{
    [TestMethod]
    public void TestDivisionTwoPositiveNumbers()
    {
        // Arrange
        var calculator = new Calculator();
        int a = 10;
        int b = 5;

        // Act
        int result = calculator.Division(a, b);

        // Assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void TestDivisionOnePositiveOneNegativeNumber()
    {
        // Arrange
        var calculator = new Calculator();
        double a = 1;
        double b = -2;

        // Act
        double result = calculator.Division(a, b);

        // Assert
        Assert.AreEqual(-0.5, result);
    }

    [TestMethod]
    public void TestDivisionByZero()
    {
        // Arrange
        var calculator = new Calculator();
        int a = 10;
        int b = 0;

        // Act & Assert
        Assert.ThrowsException<DivideByZeroException>(() => calculator.Division(a, b));
    }
}
