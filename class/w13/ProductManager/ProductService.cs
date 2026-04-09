using System.Collections.Generic;
using System.Linq;

namespace ProductManager
{
    public class ProductService
    {
        private readonly ProductRepository? _repository;
        private readonly List<Product>? _products;

        // Integration test (DB)
        public ProductService(ProductRepository repository)
        {
            _repository = repository;
        }

        // Unit test (mock data)
        public ProductService(List<Product> products)
        {
            _products = products;
        }

        public List<Product> GetProductsByCategory(string category)
        {
            if (_repository != null)
                return _repository.GetProductsByCategory(category);

            return _products!
                .Where(p => p.Category == category)
                .ToList();
        }
    }
}