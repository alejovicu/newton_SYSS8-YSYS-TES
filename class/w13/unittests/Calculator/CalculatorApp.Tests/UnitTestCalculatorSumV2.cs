namespace CalculatorApp.Tests;

[TestClass]
public class UnitTestCalculatorSumV2
{
    [TestMethod]
    [DataRow(5, 10, 15)]
    [DataRow(5, -10, -5)]
    public void TestSum(int a, int b, int expected)
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        int result = calculator.Sum(a, b);

        // Assert
        Assert.AreEqual(expected, result);
    }

}
