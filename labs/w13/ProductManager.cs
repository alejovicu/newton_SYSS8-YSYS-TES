namespace LibraryProject.MSTests;

using Npgsql;
using System.Data;

public class ProductManager
{
    private readonly IDbConnection _connection;

    public ProductManager() 
    {
        _connection = new NpgsqlConnection("Host=localhost;Port=5432;Database=bibliotek;Username=postgres;Password=pass\r\n");
    }

    public ProductManager(IDbConnection connection) 
    {
        _connection = connection;
    }

    public List<Product> GetProductsByCategory(string category)
    {
        var all = new List<Product>();

        _connection.Open();
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT name, category, price FROM products"; // ingen WHERE
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            all.Add(new Product
            {
                Name = reader.GetString(0),
                Category = reader.GetString(1),
                Price = reader.GetString(2)
            });
        }
        _connection.Close();

        return all.Where(p => p.Category == category).ToList(); // filtret i C#
    }
}

