using API.Model;

namespace API.Views {
    public class OrderProductInputView {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public OrderProductInputView() { } //for serialization

        public Product ToProduct() {
            var p = new Product() {
                Id = Id,
                ProductAmount = new ProductAmount() { Amount = Amount }
            };
            p.ProductAmount.Product = p;
            return p;
        }
    }
}