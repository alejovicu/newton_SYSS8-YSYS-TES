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
            var expectedProductCount = 2;
            var expectedCategoryTerm = "Technology";

            var mockConnection = new Mock<IDbConnection>();
            var mockCommand = new Mock<IDbCommand>();
            var mockReader = new Mock<IDataReader>();

            var rows = new List<(int Id, string Name, string Category)>
            {
                (1, "iPhone 17 Pro", "Technology"),
                (2, "Table", "Furniture"),
                (3, "MacBook Pro", "Technology"),
                (4, "Chair", "Furniture"),
            };
            int rowIndex = -1;

            // Setup mock Read to increment and read value
            mockReader.Setup(r => r.Read()).Returns(() =>
            {
                rowIndex++;
                return rowIndex < rows.Count;
            });


            // Mock return values for ID, Name and Category
            mockReader.Setup(r => r.GetInt32(0))
                .Returns(() => rows[rowIndex].Id);
            mockReader.Setup(r => r.GetString(1))
                .Returns(() => rows[rowIndex].Name);
            mockReader.Setup(r => r.GetString(2))
                .Returns(() => rows[rowIndex].Category);

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

            var rows = new List<(int Id, string Name, string Category)>
            {
                (1, "iPhone 17 Pro", "Technology"),
                (2, "Table", "Furniture"),
                (3, "MacBook Pro", "Technology"),
                (4, "Chair", "Furniture"),
            };
            int rowIndex = -1;

            // Setup mock Read to increment and read value
            mockReader.Setup(r => r.Read()).Returns(() =>
            {
                rowIndex++;
                return rowIndex < rows.Count;
            });


            // Mock return values for ID, Name and Category
            mockReader.Setup(r => r.GetInt32(0))
                .Returns(() => rows[rowIndex].Id);
            mockReader.Setup(r => r.GetString(1))
                .Returns(() => rows[rowIndex].Name);
            mockReader.Setup(r => r.GetString(2))
                .Returns(() => rows[rowIndex].Category);

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
