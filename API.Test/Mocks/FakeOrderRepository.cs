using API.Model;
using API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace API.Test {
    class FakeOrderRepository : FakeRepository<Order>, IOrderRepository {
        public void ReleaseOrder(int orderId) {
            Order orderToRelease = entities.First(entity => entity.Id == orderId);
            orderToRelease.Status = OrderStatus.Completed;
        }
    }
}
