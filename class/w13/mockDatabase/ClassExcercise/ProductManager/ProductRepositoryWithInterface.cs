namespace ProductManager;

using Npgsql;
using System.Collections.Generic;
using System.Data;


public class ProductRepositoryWithInterface : IProductRepository
{
    private readonly IDbConnection _connection;
    // Real DB
    public ProductRepositoryWithInterface()
    {
        _connection = new NpgsqlConnection("Host=localhost;Port=9999;Username=testuser ;Password=mysecretpassword;Database=testdb");
    }

    // Mock
    public ProductRepositoryWithInterface(IDbConnection connection)
    {
        _connection = connection;
    }

    public List<Product> GetProductsByCategory(string category)
    {
        var products = new List<Product>();

        _connection.Open();

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT name, category, price FROM products";

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
                if (product.Category == category)
                {
                products.Add(product);
                }
        });
        }

        _connection.Close();

        return products;
    }

}
