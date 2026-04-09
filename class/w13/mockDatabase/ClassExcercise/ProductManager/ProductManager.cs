using System;
namespace ProductManager;

public class ProductManager
{
    private readonly IDatabaseContext _context;

    public ProductManager(IDatabaseContext context)
    {
        _context = context;
    }

    public List<Product> GetProductsByCategory(string category)
    {
        var allProducts = _context.GetAllProducts();
        return allProducts.Where(propa => propa.Category == category).ToList();
    }
}
