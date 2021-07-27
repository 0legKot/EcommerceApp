using NUnit.Framework;
using API.Model;
using API.Repositories;
using API.Controllers;
using API.Views;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace API.Test {
    public class OrderControllerTest {
        IOrderRepository repository;
        OrderController controller;
        [SetUp]
        public void Setup() {
            repository = new FakeOrderRepository();
            controller = new OrderController(repository);
        }

        [Test]
        public void StroredOrdersCanBeRetrieved() {
            repository.Insert(new Order() { Id = 1, Status = OrderStatus.Completed, OrderProducts = new List<OrderProduct>() { } });
            repository.Insert(new Order() { Id = 2, Status = OrderStatus.Open, OrderProducts = new List<OrderProduct>() { } });
            List<OrderOutputView> allOrders = controller.GetAllOrders().ToList();
            Assert.AreEqual(2, allOrders.Count);
            Assert.IsTrue(allOrders.Any(order => order.Id == 1 && order.Status == OrderStatus.Completed));
            Assert.IsTrue(allOrders.Any(order => order.Id == 2 && order.Status == OrderStatus.Open));
        }

        [Test]
        public void OrderCanBeRetrievedById() {
            repository.Insert(new Order() { Id = 1, Status = OrderStatus.Completed, OrderProducts = new List<OrderProduct>() { } });
            OrderOutputView order = controller.GetOrder(1);
            Assert.IsNotNull(order);
            Assert.AreEqual(1, order.Id);
            Assert.AreEqual(OrderStatus.Completed, order.Status);
        }

        [Test]
        public void CreatedOrdersAreStored() {
            IActionResult result = controller.Create(new OrderInputView() {
                Id = 0, 
                Products = new List<OrderProductInputView>() {
                    new OrderProductInputView() {Id = 1, Amount = 1.1m },
                    new OrderProductInputView() {Id = 2, Amount = 1.2m }
                }
            });
            Assert.IsTrue(result is OkObjectResult);
            List<Order> allOrders = repository.Get().ToList();
            Assert.AreEqual(1, allOrders.Count);
            Assert.IsNotNull(allOrders[0]);
            Assert.AreEqual(1, allOrders[0].Id);
            Assert.AreEqual(1, allOrders[0].OrderProducts[0].ProductId);
            Assert.AreEqual(1.1m, allOrders[0].OrderProducts[0].Product.ProductAmount.Amount);
            Assert.AreEqual(2, allOrders[0].OrderProducts[1].ProductId);
            Assert.AreEqual(1.2m, allOrders[0].OrderProducts[1].Product.ProductAmount.Amount);
        }

        [Test]
        public void NewOrderCannotHaveNonZeroId() {
            IActionResult result = controller.Create(new OrderInputView() {
                Id = 1,
                Products = new List<OrderProductInputView>() {
                    new OrderProductInputView() {Id = 1, Amount = 1.1m, },
                    new OrderProductInputView() {Id = 2, Amount = 1.2m }
                }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Order Id should NOT be specified", (result as BadRequestObjectResult)?.Value);
        }

        [Test]
        public void NewOrderCannotHaveNonPositiveAmount() {
            IActionResult result = controller.Create(new OrderInputView() {
                Id = 0,
                Products = new List<OrderProductInputView>() {
                    new OrderProductInputView() {Id = 1, Amount = 0, },
                    new OrderProductInputView() {Id = 2, Amount = 1.2m, }
                }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Amount should be greater than 0", (result as BadRequestObjectResult)?.Value);

            result = controller.Create(new OrderInputView() {
                Id = 0,
                Products = new List<OrderProductInputView>() {
                    new OrderProductInputView() {Id = 1, Amount = -1m },
                }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Amount should be greater than 0", (result as BadRequestObjectResult)?.Value);
        }

        [Test]
        public void OrderCanBeUpdated() {
            repository.Insert(new Order() { Id = 1, Status = OrderStatus.Open, OrderProducts = new List<OrderProduct>() { } });
            IActionResult result = controller.Update(new OrderInputView() { Id = 1, Products = new List<OrderProductInputView>() { new OrderProductInputView() { Id = 1, Amount = 1.1m } } });
            Assert.IsTrue(result is OkResult);
            Assert.AreEqual(1, repository.Get().ToList().Count);
        }

        [Test]
        public void OrderToUpdateCannotHaveZeroId() {
            IActionResult result = controller.Update(new OrderInputView() { Id = 0, Products = new List<OrderProductInputView>() { new OrderProductInputView() { Id = 1, Amount = 1.1m } } });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Order Id should be specified", (result as BadRequestObjectResult)?.Value);
        }

        [Test]
        public void OrderToUpdateCannotHaveProductWithZeroAmount() {
            repository.Insert(new Order() { Id = 1, Status = OrderStatus.Open, OrderProducts = new List<OrderProduct>() { } });
            IActionResult result = controller.Update(new OrderInputView() { Id = 1, Products = new List<OrderProductInputView>() { new OrderProductInputView() { Id = 1, Amount = 0 } } });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Amount should be greater than 0", (result as BadRequestObjectResult)?.Value);
        }

        [Test]
        public void OrderCanBeReleased() {
            repository.Insert(new Order() { Id = 1, Status = OrderStatus.Open, OrderProducts = new List<OrderProduct>() { } });
            IActionResult result = controller.Complete(1);
            Assert.IsTrue(result is OkResult);
            Assert.AreEqual(OrderStatus.Completed, repository.GetByID(1).Status);
        }

        [Test]
        public void OrderIdForReleaseCannotBeZero() {
            IActionResult result = controller.Complete(0);
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Order Id should be specified", (result as BadRequestObjectResult)?.Value);
        }
    }
}