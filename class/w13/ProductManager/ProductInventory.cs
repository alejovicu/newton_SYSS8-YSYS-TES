using System;
using Npgsql;
using System.Collections.Generic;

namespace ProductManager
{
    // ProductInventory provides access to product data.
    public class ProductInventory
    {
        // Repository interface used to access product data.
        private readonly IProductRepository _repo;

        // Constructor that accepts a repository implementation.
        // This enables dependency injection, making unit tests possible without a database.
        public ProductInventory(IProductRepository repo)
        {
            _repo = repo;
        }

        // Gets all products in a specific category.
        public List<Product> GetProductsByCategory(string category)
        {
            return _repo.GetByCategory(category);
        }
    }
}