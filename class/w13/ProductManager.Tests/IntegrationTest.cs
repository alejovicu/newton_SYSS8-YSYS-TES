using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductManager;
using System.Linq;

namespace ProductManager.Tests
{
    [TestClass]
    public class ProductIntegrationTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public void GetProductsByCategory_FromDatabase()
        {
            // Arrange
            var repository = new ProductRepository();
            var service = new ProductService(repository);

            // Act
            var techProducts = service.GetProductsByCategory("Tech");
            var foodProducts = service.GetProductsByCategory("Food");

            // Assert
            Assert.IsTrue(techProducts.Any());
            Assert.IsTrue(foodProducts.Any());

            Assert.AreEqual("Tech", techProducts[0].Category);
            Assert.AreEqual("Food", foodProducts[0].Category);
        }
    }
}