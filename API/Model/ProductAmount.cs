using System.ComponentModel.DataAnnotations.Schema;

namespace API.Model {
    public class ProductAmount {
        public int ProductAmountId { get; set; }
        public Product Product { get; set; }
        [Column(TypeName = "decimal(38,19)")]
        public decimal Amount { get; set; }
    }
}