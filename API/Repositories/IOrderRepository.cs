using API.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace API.Repositories {
    public interface IOrderRepository : IRepository<Order> {
        void ReleaseOrder(int orderId);
    }
}