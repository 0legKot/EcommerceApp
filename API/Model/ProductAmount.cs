using System.ComponentModel.DataAnnotations.Schema;

namespace API.Model
{
    public class ProductAmount
    {
        public int ProductAmountId { get; set; }
        public Product Product { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Amount { get; set; }
    }
}