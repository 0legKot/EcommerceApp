using API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Views {
    public class OrderOutputView {
        public int Id { get; set; }
        public List<ProductView> Products { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderOutputView(Order order) {
            Id = order?.Id ?? 0;
            Status = order?.Status ?? 0;
            Products = order?.OrderProducts?
                .Select(product => new ProductView(product.Product) {
                    Amount = product.Amount
            })?.ToList() ?? new List<ProductView>();
            TotalPrice = Products.Sum(product => product.Price * product.Amount);
        }
    }
}
