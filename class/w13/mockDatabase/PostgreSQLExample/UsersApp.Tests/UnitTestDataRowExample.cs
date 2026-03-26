namespace UsersApp.Tests;

public static class Calculator
{
    public static int Add(int a, int b)
    {
       return a + b;
    }

}

[TestClass]
public class UnitTestDataRowExample
{
    [TestMethod]
    [DataRow(1, 2, 3, DisplayName = "Test two possitive numbers case 1")]
    [DataRow(3, 4, 7, DisplayName = "Test two possitive numbers case 2")]
    public void TestAdd(int a, int b, int expected)
    {
        Assert.AreEqual(Calculator.Add(a, b), expected);
    }

}
