namespace ProductManager;

public interface IDatabaseContext
{
    IEnumerable<Product> GetAllProducts();
}
