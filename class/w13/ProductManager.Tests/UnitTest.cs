using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProductManager;
using System.Collections.Generic;
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
            var expectedCategory = "Food";

            var mockConnection = new Mock<IDbConnection>();
            var mockCommand = new Mock<IDbCommand>();
            var mockReader = new Mock<IDataReader>();
            var mockParameter = new Mock<IDbDataParameter>();
            var mockParameters = new Mock<IDataParameterCollection>();

            // Connection
            mockConnection.Setup(c => c.Open());
            mockConnection.Setup(c => c.Close());
            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

            // Command
            mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
            mockCommand.Setup(c => c.CreateParameter()).Returns(mockParameter.Object);
            mockCommand.Setup(c => c.Parameters).Returns(mockParameters.Object);

            // Reader (simulate DB rows)
            mockReader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);

            // name
            mockReader.SetupSequence(r => r.GetString(0))
                .Returns("Pizza")
                .Returns("iPhone 17 Pro");

            // category
            mockReader.SetupSequence(r => r.GetString(1))
                .Returns("Food")
                .Returns("Tech");

            // price
            mockReader.SetupSequence(r => r.GetString(2))
                .Returns("120")
                .Returns("13000");

            var repository = new ProductRepository(mockConnection.Object);
            var service = new ProductService(repository);

            // Act
            var result = service.GetProductsByCategory(expectedCategory);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Food", result[0].Category);
            Assert.AreEqual("Pizza", result[0].Name);
        }
    }
}