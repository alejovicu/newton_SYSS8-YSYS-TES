using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProductManager;
using System.Data;

namespace ProductManager.Tests
{
    [TestClass]
    public class ProductUnitTests
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        public void GetProductsByCategory_UnitTest_WithMock()
        {
            // Arrange
            var mockConnection = new Mock<IDbConnection>();
            var mockCommand = new Mock<IDbCommand>();
            var mockReader = new Mock<IDataReader>();

            mockConnection.Setup(c => c.Open());
            mockConnection.Setup(c => c.Close());
            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

            mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);

            mockReader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);

            mockReader.SetupSequence(r => r.GetString(0))
                .Returns("Pizza")
                .Returns("iPhone 17 Pro");

            mockReader.SetupSequence(r => r.GetString(1))
                .Returns("Food")
                .Returns("Tech");

            mockReader.SetupSequence(r => r.GetString(2))
                .Returns("120")
                .Returns("13000");

            var repository = new ProductRepository(mockConnection.Object);
            var service = new ProductService(repository);

            // Act
            var result = service.GetProductsByCategory("Food");

            // Assert
            Assert.HasCount(1, result);
            Assert.AreEqual("Food", result[0].Category);
            Assert.AreEqual("Pizza", result[0].Name);
            Assert.AreEqual("120", result[0].Price);
        }
    }
}