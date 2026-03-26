using Npgsql;
using System.Data;
using Moq;

namespace ProductManager.Tests
{

    [TestClass]
    public sealed class UnitTestProductRepo
    {

        [TestMethod]
        [TestCategory("Integration")]
        public void TestGetProductByCategory_ShouldBeListOfProduct()
        {
            // Arrange
            var productRepository = new ProductRepository();
            var expectedCategoryTerm = "Technology";

            // Act
            var result = productRepository.GetProductsByCategory(expectedCategoryTerm);

            // Assert
            Assert.IsInstanceOfType(result, typeof(List<Product>));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void TestGetProductByCategory_ValidateExpected()
        {
            // Arrange
            var expectedProductCount = 1;
            var expectedCategoryTerm = "Technology";

            var mockConnection = new Mock<IDbConnection>();
            var mockCommand = new Mock<IDbCommand>();
            var mockReader = new Mock<IDataReader>();

            // Setup mock Read to increment and read value
            var readCallCount = 0;
            mockReader.Setup(r => r.Read()).Returns(() => readCallCount++ < 2);

            // Mock return values for ID, Name and Category
            mockReader.Setup(r => r.GetInt32(0))
                .Returns(() => readCallCount == 1 ? 1 : 2);
            mockReader.Setup(r => r.GetString(1))
                .Returns(() => readCallCount == 1 ? "iPhone 17 Pro" : "Banana");
            mockReader.Setup(r => r.GetString(2))
                .Returns(() => readCallCount == 1 ? "Technology" : "Food");

            // Setup ExecuteReader() to return our mock reader
            mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
            // Setup CreateCommand() to return our mock command
            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

            var productRepository = new ProductRepository(mockConnection.Object);

            // Act
            var result = productRepository.GetProductsByCategory(expectedCategoryTerm);

            // Assert
            Assert.HasCount(expectedProductCount, result);
            Assert.AreEqual(expectedCategoryTerm, result[0].Category);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [TestCategory("Slow")]
        public void TestGetProductByCategory_EnsureAllReturnTypesMatchAndValidateName()
        {
            // Arrange
            var expectedCategoryTerm = "Technology";

            var mockConnection = new Mock<IDbConnection>();
            var mockCommand = new Mock<IDbCommand>();
            var mockReader = new Mock<IDataReader>();

            // Setup mock Read to increment and read value
            var readCallCount = 0;
            mockReader.Setup(r => r.Read()).Returns(() => readCallCount++ == 0);

            // Mock return values for ID, Name and Category
            mockReader.Setup(r => r.GetInt32(0)).Returns(1);
            mockReader.Setup(r => r.GetString(1)).Returns("iPhone 17 Pro");
            mockReader.Setup(r => r.GetString(2)).Returns("Technology");

            // Setup ExecuteReader() to return our mock reader
            mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
            // Setup CreateCommand() to return our mock command
            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

            var productRepository = new ProductRepository(mockConnection.Object);

            // Act
            var result = productRepository.GetProductsByCategory(expectedCategoryTerm);

            // Assert
            foreach (var product in result)
            {
                Assert.AreEqual(expectedCategoryTerm, product.Category);
                Assert.IsNotEmpty(product.Name);
            }

        }

    }
}
