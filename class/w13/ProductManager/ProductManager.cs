namespace ProductManager;

// ProductManager handles business logic related to products.
public class ProductManager
{
    // Repository interface used to access product data.
    private readonly IProductRepository _repo;

    // Constructor that accepts a repository implementation.
    public ProductManager(IProductRepository repo)
    {
        _repo = repo;
    }

    // Retrieves all products in the specified category.
    public List<Product> GetProductsByCategory(string category)
    {
        return _repo
            .GetByCategory(category)
            .Where(p => p.Category == category)
            .ToList();
    }
}