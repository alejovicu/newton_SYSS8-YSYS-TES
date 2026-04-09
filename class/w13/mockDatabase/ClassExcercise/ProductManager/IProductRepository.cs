namespace ProductManager;

public interface IProductRepository
{
    List<Product> GetProductsByCategory(string category);
}
