namespace ProductManager;
using System.Collections.Generic;

public interface IProductRepository
{
    List<Product> GetProductsByCategory(string category);
}