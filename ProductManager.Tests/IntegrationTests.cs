using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductManager;
using System.Linq;

namespace ProductManager.Tests;

[TestClass]
public class IntegrationTests
{
	[TestMethod]
	[TestCategory("Integration")]
	public void GetProductsByCategory_ShouldReturnTechProducts_FromDatabase()
	{
		// Arrange
		var repo = new ProductRepository();
		var service = new ProductService(repo);

		// Act
		var result = service.GetProductsByCategory("Tech");

		// Assert
		Assert.IsTrue(result.Count > 0);
		Assert.IsTrue(result.All(p => p.Category == "Tech"));
	}
}