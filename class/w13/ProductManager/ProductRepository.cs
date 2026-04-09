using Npgsql;
using System.Collections.Generic;
using System.Data;

namespace ProductManager
{
    public class ProductRepository
    {
        private readonly IDbConnection _connection;

        // Database Connection
        public ProductRepository()
        {
            _connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mydb");
        }

        // MOCKING / Constructor for dependancy injection
        public ProductRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public List<Product> GetProductsByCategory(string category)
        {
            var products = new List<Product>();

            _connection.Open();

            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT name, category, price FROM products WHERE category = @category";

            var parameter = cmd.CreateParameter();
            parameter.ParameterName = "@category";
            parameter.Value = category;
            cmd.Parameters.Add(parameter);

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

            return products;
        }
    }
}
