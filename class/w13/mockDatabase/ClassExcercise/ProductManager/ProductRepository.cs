using Npgsql;
using System.Data;

namespace ProductManager;

public class ProductRepository : IProductRepository
{
        private readonly IDbConnection _connection;

        // Constructor for real database connection
        public ProductRepository()
        {
            _connection = new NpgsqlConnection("Host=127.0.0.1;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres");
        }

        // Constructor for dependency injection (mocking)
        public ProductRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public List<Product> GetProductsByCategory(string category)
        {
            var products = new List<Product>();
            if (_connection.State != ConnectionState.Open) _connection.Open();
            

            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT id, name, category, price FROM products";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Category = reader.GetString(2),
                    Price = reader.GetString(3)
                });
            }

            _connection.Close();

            return products
                .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();;
        }
}