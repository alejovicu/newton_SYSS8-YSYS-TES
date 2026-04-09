using System;
namespace ProductManager;

// Interface for repositary, for mocking in unit test
public interface IProductRepository
{
    List<Product> GetByCategory(string category);
}
