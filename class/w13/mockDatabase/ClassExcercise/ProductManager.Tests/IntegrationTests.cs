using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductManager;

namespace ProductManager.Tests;
[TestClass]
public class IntegrationTests
{
	[TestMethod]
	[TestCategory("Integration")]
	public void GetProductByCategory_RealData()
	{
		// Arrange 
		var repo = new ProductRepository();
		string existingCategory = "Tech";
		
		// Act
		var result = repo.GetProductsByCategory(existingCategory);
		
		// Assert 
		Assert.IsNotNull(result);
		if (result.Count > 0)
		{
			Assert.AreEqual(existingCategory, result[0].Category);
		}
	}
}

