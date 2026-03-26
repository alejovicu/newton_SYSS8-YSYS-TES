using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;

namespace ProductManager
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnection _connection;

        // Real DB
        public ProductRepository()
        {
            _connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres");
        }

        // Setup repo with mock when value is used
        public ProductRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public List<Product> GetProductsByCategory(string Category)
        {
            var products = new List<Product>();

            _connection.Open();

            using var cmd = _connection.CreateCommand();
            cmd.CommandText = $"SELECT id, name, category FROM Products";
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var product = new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Category = reader.GetString(2)
                };

                if (product.Category == Category)
                {
                    products.Add(product);
                }
            }

            _connection.Close();

            return products;
            
        }
    }
}
