using System.Collections.Generic;

namespace ProductManager;

public interface IProductRepository
{
    List<Product> GetProductsByCategory(string category);
	void InsertProduct(Product product);

}