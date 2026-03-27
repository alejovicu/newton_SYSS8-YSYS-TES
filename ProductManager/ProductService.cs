namespace ProductManager;

public class ProductService
{
    private readonly ProductRepository _repo;

    public ProductService(ProductRepository repo)
    {
        _repo = repo;
    }

    public List<Product> GetProductsByCategory(string category)
    {
        return _repo.GetProductsByCategory(category);
    }
}