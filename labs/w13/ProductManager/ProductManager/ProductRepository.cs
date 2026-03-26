namespace ProductManager;

using Npgsql;
using System.Collections.Generic;

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository()
    {
        _connectionString = "Host=localhost;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres";
    }
}