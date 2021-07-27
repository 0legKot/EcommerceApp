using API.Model;

namespace API.Views {
    public class ProductView {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public CategoryView Category { get; set; }
        public decimal Amount { get; set; }
        public ProductView() { } //for serialization
        public ProductView(Product product) {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Price = product.Price;
            Rating = product.Rating;
            Category = new CategoryView() {
                Id = product.Category?.Id ?? 0,
                Name = product.Category?.Name ?? ""
            };
            Amount = product.ProductAmount?.Amount ?? 0;
        }
        public Product ToProduct() {
            var p = new Product() {
                Id = Id,
                Category = new Category() { Id = Category?.Id ?? 0, Name = Category?.Name ?? "" },
                Name = Name,
                Description = Description,
                Price = Price,
                Rating = Rating,
                ProductAmount = new ProductAmount() { Amount = Amount }
            };
            p.ProductAmount.Product = p;
            return p;
        }
    }
}