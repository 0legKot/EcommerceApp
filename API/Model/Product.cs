using System.ComponentModel.DataAnnotations.Schema;

namespace API.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public Category Category { get; set; }
        public ProductAmount ProductAmount { get; set; }
    }
}