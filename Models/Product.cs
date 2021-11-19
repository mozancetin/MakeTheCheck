using System.Drawing;

namespace MakeTheCheck.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public int ProductImage { get; set; }
    }
}
