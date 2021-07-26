using API.Model;
using System.Collections.Generic;
using System.Linq;

namespace API.Views {
    public class OrderInputView {
        public int Id { get; set; }
        public List<ProductView> Products { get; set; }
        public Order ToOrder() {
            var order = new Order() { Id = Id, Status = OrderStatus.Open };
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
