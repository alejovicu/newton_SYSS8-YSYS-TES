using Npgsql;
using System;
using System.Data;

namespace ProductManager
{
    public class ProductManager
    {
        private readonly IDbConnection _connection;

        // Real DB
        public ProductManager()
        {
            _connection = new NpgsqlConnection("Host=localhost;Port=9999;Username=testuser;Password=mysecretpassword;Database=testdb");
        }

        // Mock DB
        public ProductManager(IDbConnection connection)
        {
            _connection = connection;
        }

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();

            _connection.Open();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT id, name, category, price FROM product";
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Category = reader.GetString(2),
                    Price = reader.GetInt32(3)
                });
            }

            _connection.Close();

            return products;
        }

        public List<Product> GetProductsByCategory(string category)
        {
            var products = GetAllProducts();            
            var productsByCat = new List<Product>();
            
            foreach (var cat in products)
            {
                if (cat.Category == category)
                {
                    productsByCat.Add(cat);
                }
            }

            return productsByCat;
        }

        public void AddProduct(Product product)
        {
            _connection.Open();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "INSERT INTO product (name, category, price) VALUES ('" + product.Name + "', '" + product.Category + "', " + product.Price + ")";
            cmd.CommandText = "INSERT INTO product (name, category, price) VALUES ('" + product.Name + "', '" + product.Category + "', " + product.Price + ")";
            using var reader = cmd.ExecuteReader();

            _connection.Close();
        }
    }
}