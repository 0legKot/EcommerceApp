using API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Views {
    public class OrderView {
        public int Id { get; set; }
        public List<ProductView> Products { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderView() { } //for serialization
        public OrderView(Order order) {
            Id = order.Id;
            Status = order.Status;
            Products = order.OrderProducts
                .Select(product => new ProductView(product.Product) {
                    Amount = product.Amount
            }).ToList();
            TotalPrice = Products.Sum(product => product.Price * product.Amount);
        }
        public Order ToOrder() {
            var order = new Order() { Id = Id, Status = Status };
            order.OrderProducts = Products.Select(productView => new OrderProduct() {
                Product = productView.ToProduct(),
                Amount = productView.Amount,
                Order = order,
                OrderId = Id,
                ProductId = productView.Id
            }).ToList();
            return order;
        }
    }
}
