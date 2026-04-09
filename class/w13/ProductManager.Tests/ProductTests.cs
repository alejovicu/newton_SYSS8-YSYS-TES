using Moq;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductManager;

namespace ProductManager.Tests
{
    [TestClass]
    public class ProductTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public void GetProductsByCategory()
        {
            // Arrange
            var service = new ProductService();

            // Act
            var result = service.GetProductsByCategory("Tech");

            // Assert
            Assert.HasCount(3, result);
            Assert.AreEqual("iPhone 17 Pro", result[0].Name);
            Assert.AreEqual("MacBook Pro", result[1].Name);
            Assert.AreEqual("Airfryer", result[2].Name);

            foreach (var product in result)
            {
                Assert.AreEqual("Tech", product.Category);
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void GetProductsByCategoryUnitTest()
        {
            // Arrange
            var expectedCategory = "Tech";

            var mockConnection = new Mock<IDbConnection>();
            var mockCommand = new Mock<IDbCommand>();
            var mockReader = new Mock<IDataReader>();
            var mockParameters = new Mock<IDataParameterCollection>();
            var mockParameter = new Mock<IDbDataParameter>();

            mockConnection.Setup(c => c.Open());
            mockConnection.Setup(c => c.Close());

            mockCommand.Setup(c => c.CreateParameter()).Returns(mockParameter.Object);
            mockCommand.Setup(c => c.Parameters).Returns(mockParameters.Object);

            mockReader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);

            // Row 1
            mockReader.SetupSequence(r => r.GetString(0))
                .Returns("iPhone 17 Pro")
                .Returns("Bread");

            // Row 2
            mockReader.SetupSequence(r => r.GetString(1))
                .Returns("Tech")
                .Returns("Food");

            // Row 3
            mockReader.SetupSequence(r => r.GetString(2))
                .Returns("13000")
                .Returns("30");

            mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

            var service = new ProductService(mockConnection.Object);

            // Act
            var result = service.GetProductsByCategory(expectedCategory);

            // Assert
            Assert.HasCount(1, result);
            Assert.AreEqual("iPhone 17 Pro", result[0].Name);
            Assert.AreEqual("Tech", result[0].Category);
            Assert.AreEqual("13000", result[0].Price);
            Assert.IsFalse(result.Exists(p => p.Name == "Bread"));
        }
    }
}
