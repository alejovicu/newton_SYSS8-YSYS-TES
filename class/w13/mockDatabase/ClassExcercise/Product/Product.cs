using System.ComponentModel.DataAnnotations;

namespace ProductManager
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [Required] public string Category { get; set; } 
        public int Price { get; set; }
    }
}
