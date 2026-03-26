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
            var productRepository = new ProductRepository();
            var expectedProductCount = 1;
            var expectedCategoryTerm = "Technology";

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
            var productRepository = new ProductRepository();
            var expectedCategoryTerm = "Technology";

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
