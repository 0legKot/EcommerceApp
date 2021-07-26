using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Model {
    public class Product : IEntity {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(38,19)")]
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public Category Category { get; set; }
        public ProductAmount ProductAmount { get; set; }
        public List<OrderProduct> OrderProducts { get; set; }
        public List<Order> Orders { get; set; }
    }
}