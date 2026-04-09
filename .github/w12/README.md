# Create First Unit Test project

Let's create an application to do the basic atirhmetic operations + - * /

```bash
# Create Project
mkdir Calculator
cd Calculator
dotnet new sln -n Calculator


# Create a Library
dotnet new classlib -n CalculatorApp
dotnet sln add CalculatorApp

# Create the Tests for the library
dotnet new mstest -n CalculatorApp.Tests
dotnet sln add CalculatorApp.Tests

# Link App with Tests
dotnet add CalculatorApp.Tests reference CalculatorApp
```


Let's implement the Operations and unit tests, then Run the unit tests.

1. Create the file `class/w12/Calculator/CalculatorApp/Calculator.cs`
    ```C#
    namespace CalculatorApp;

    public class Calculator
    {
        public int Sum( int a, int b)
        {
            return a + b;
        }
    }
    ```

1. Create the file `class/w12/Calculator/CalculatorApp.Tests/UnitTestCalculator.cs`
    ```C#
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
    ```


Run the tests
```shell
dotnet test
```

Now let's Implement all Operations and validate with tests if the application works properly.