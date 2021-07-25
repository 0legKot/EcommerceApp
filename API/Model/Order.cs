using System.Collections.Generic;

namespace API.Model
{
    public class Order : IEntity
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderProduct> OrderProducts { get; set; }
    }
}