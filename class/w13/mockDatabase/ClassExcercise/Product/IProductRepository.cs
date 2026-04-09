using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManager
{
    public interface IProductRepository
    {
        List<Product> GetProductsByCategory(string Category);
    }
}
