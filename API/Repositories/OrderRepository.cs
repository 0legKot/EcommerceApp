using API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repositories {
    public class OrderRepository : BaseRepository<Order>, IOrderRepository {
        public OrderRepository(StoreDbContext context) : base(context) { }
        public override IEnumerable<Order> Get(Expression<Func<Order, bool>> filter = null, Func<Order, object> orderBy = null, int skip = 0, int take = 0) {
            var query = CreateQuery(filter, skip, take)
                .Include(order => order.OrderProducts)
                .Include(order => order.Products).ThenInclude(product => product.Category);
            if (orderBy != null) {
                return query.OrderBy(orderBy).ToList();
            }
            return query.ToList();
        }
        public override Order GetByID(int id) {
            return context.Orders.Include(order => order.OrderProducts)
                .Include(order => order.Products).ThenInclude(product => product.Category)
                .FirstOrDefault(product => product.Id == id);
        }
        public override void Update(Order updatedEntity) {
            if (context.Orders.Find(updatedEntity.Id) == null) { throw new ApplicationException("Order not found"); }
            SetProducts(updatedEntity);
            context.SaveChanges();
        }
        public override void Insert(Order entity) {
            SetProducts(entity);
            base.Insert(entity);
        }
        public void ReleaseOrder(int orderId) {
            Order order = context.Orders.Include(order => order.OrderProducts).FirstOrDefault(order => order.Id == orderId) ?? throw new ApplicationException("Order not found");
            HashSet<int> productIds = order.OrderProducts.Select(product => product.ProductId).ToHashSet();
            List<ProductAmount> productAmounts = GetProductAmounts(productIds);
            List<Product> products = GetProducts(productIds);

            ValidateOrder(order, productAmounts, order, products);
            if (order.OrderProducts.Count == 0) {
                throw new ApplicationException("Order without products cannot be released");
            }

            foreach (var productAmount in productAmounts) {
                productAmount.Amount -= order.OrderProducts.First(prod => prod.ProductId == productAmount.Product.Id).Amount;
            }
            order.Status = OrderStatus.Completed;
            context.SaveChanges();
        }

        private List<Product> GetProducts(HashSet<int> productIds) {
            return context.Products.Where(prod => productIds.Contains(prod.Id)).ToList();
        }

        private void SetProducts(Order updatedEntity) {
            HashSet<int> newProductIds = updatedEntity.OrderProducts.Select(product => product.ProductId).ToHashSet();
            List<ProductAmount> productAmounts = GetProductAmounts(newProductIds);
            List<Product> products = GetProducts(newProductIds);
            ValidateOrder(updatedEntity, productAmounts, context.Orders.Find(updatedEntity.Id), products);

            List<OrderProduct> existingProducts = context.OrderProducts.Where(ordProduct => ordProduct.OrderId == updatedEntity.Id).ToList();
            var productsToRemove = existingProducts.Where(prod => !newProductIds.Contains(prod.ProductId));
            context.OrderProducts.RemoveRange(productsToRemove);
            existingProducts.RemoveAll(prod => !newProductIds.Contains(prod.ProductId));

            foreach (var ordProduct in existingProducts) {
                ordProduct.Amount = updatedEntity.OrderProducts.First(prod => prod.ProductId == ordProduct.ProductId).Amount;
            }

            HashSet<int> existingProductIds = existingProducts.Select(prod => prod.ProductId).ToHashSet();
            var newProducts = updatedEntity.OrderProducts.Where(prod => !existingProductIds.Contains(prod.ProductId)).ToList();
            foreach (var newProduct in newProducts) {
                newProduct.Product = products.First(prod => prod.Id == newProduct.Product.Id);
                newProduct.ProductId = newProduct.Product.Id;
                newProduct.OrderId = updatedEntity.Id;
            }
            context.OrderProducts.AddRange(newProducts);
        }

        private List<ProductAmount> GetProductAmounts(HashSet<int> newProductIds) {
            return context.ProductAmounts
                            .Include(prodAmount => prodAmount.Product).AsNoTracking()
                            .Where(prodAmount => newProductIds.Contains(prodAmount.Product.Id))
                            .ToList();
        }

        private void ValidateOrder(Order updatedEntity, List<ProductAmount> productAmounts, Order oldEntity, List<Product> products) {
            if (oldEntity != null && oldEntity.Status == OrderStatus.Completed) {
                throw new ApplicationException($"No actions allowed on orders with status {OrderStatus.Completed}");
            }

            CheckAllProductsExist(updatedEntity, products);
            CheckEnoughProductsPresent(updatedEntity, productAmounts);
        }

        private void CheckAllProductsExist(Order updatedEntity, List<Product> products) {
            HashSet<int> newProductIds = updatedEntity.OrderProducts.Select(product => product.ProductId).ToHashSet();
            HashSet<int> foundProductIds = products.Select(prod => prod.Id).ToHashSet();
            if (!foundProductIds.SetEquals(newProductIds)) {
                var notFoundIds = string.Join(',', newProductIds.Except(products.Select(product => product.Id)));
                throw new ApplicationException($"Not all products found. Not found ids: {notFoundIds}");
            }
        }

        private static void CheckEnoughProductsPresent(Order updatedEntity, List<ProductAmount> productAmounts) {
            List<ProductAmount> runOutProducts = productAmounts.Where(
                            productAmount => productAmount.Amount <
                                             updatedEntity.OrderProducts.First(ordProduct => ordProduct.ProductId == productAmount.Product.Id).Amount
                            ).ToList();
            if (runOutProducts.Count > 0) {
                var notFoundIds = string.Join(',', runOutProducts.Select(productAmount => productAmount.Product.Id));
                throw new ApplicationException($"Some products are out of stock. Products' out of stock ids: {notFoundIds}");
            }
        }
    }
}
