using Dapper;
using Npgsql;

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
        // Hämta ALLA produkter från databasen och filtrera sedan själv
        var allProducts = _context.GetAllProducts();
        return allProducts.Where(p => p.Category == category).ToList();
    }
}



