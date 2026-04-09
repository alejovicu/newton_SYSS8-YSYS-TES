using Npgsql;
using System.Collections.Generic;
using System.Data;

namespace ProductManager
{
    public class ProductService
    {
        private readonly IDbConnection _connection;

        public ProductService()
        {
            _connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mydb");
        }

        public ProductService(IDbConnection connection)
        {
            _connection = connection;
        }

        public List<Product> GetProductsByCategory(string category)
        {
            var products = new List<Product>();

            _connection.Open();

            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT name, category, price FROM products WHERE category = @category ORDER BY id";

            var parameter = cmd.CreateParameter();
            parameter.ParameterName = "@category";
            parameter.Value = category;
            cmd.Parameters.Add(parameter);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var product = new Product
                {
                    Name = reader.GetString(0),
                    Category = reader.GetString(1),
                    Price = reader.GetString(2)
                };

                if (product.Category == category)
                {
                    products.Add(product);
                }
            }

            _connection.Close();

            return products;
        }
    }
}