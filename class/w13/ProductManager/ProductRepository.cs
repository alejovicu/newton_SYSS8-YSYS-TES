using Npgsql;
using System.Collections.Generic;
using System.Data;

namespace ProductManager
{
    public class ProductRepository
    {
        private readonly IDbConnection _connection;

        // Constructor for real database connection
        public ProductRepository()
        {
            _connection = new NpgsqlConnection(
                "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=productsdb");
        }

        // Constructor for dependency injection (mocking)
        public ProductRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        // Make method virtual so it can be overridden in tests if needed
        public List<Product> GetProductsByCategory(string category)
        {
            var products = new List<Product>();

            _connection.Open();

            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT name, category, price FROM products";

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product
                {
                    Name = reader.GetString(0),
                    Category = reader.GetString(1),
                    Price = reader.GetString(2)
                });
            }

            _connection.Close();

            return products
                .Where(p => p.Category == category)
                .ToList();
        }
    }
}