using API.Model;

namespace API.Views
{
    public class ProductView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public Category Category { get; set; }
        public decimal Amount { get; set; }
        public ProductView() { } //for serialization
        public ProductView(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Price = product.Price;
            Rating = product.Rating;
            Category = product.Category;
            Amount = product.ProductAmount.Amount;

        }
    }
}