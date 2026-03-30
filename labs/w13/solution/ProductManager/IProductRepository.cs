namespace ProductManager;

public interface IProductRepository
{
    List<Product> GetAllProducts();
    List<Product> GetProductsByCategory(string category);
}
