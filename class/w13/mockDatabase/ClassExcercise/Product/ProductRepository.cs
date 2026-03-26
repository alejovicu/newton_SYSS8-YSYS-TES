using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ProductManager
{
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository()
        {
            _connectionString = "Host=localhost;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres";
        }

        public List<Product> GetProductsByCategory(string Category)
        {
            var products = new List<Product>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand($"SELECT id, name, category FROM Products WHERE Category = '{Category}'", connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Category = reader.GetString(2)
                });
            }

            return products;
        }
    }
}
