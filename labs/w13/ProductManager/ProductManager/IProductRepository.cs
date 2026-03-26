namespace ProductManager;
using System.Collections.Generic;

public interface IProductRepository
{
    List<Product> GetAllProducts();
    List<Product> GetAllProductsByCategory(string category);
    void InsertProduct(Product product);
}

