namespace ProductManager;

public class ProductManagerService
{
    private readonly IProductRepository _repository;

    public ProductManagerService(IProductRepository repository)
    {
        _repository = repository;
    }

    public List<Product> GetProductsByCategory(string category)
    {
        var products = _repository.GetAllProducts();

        return products
            .Where(p => p.Category == category)
            .ToList();
    }
}