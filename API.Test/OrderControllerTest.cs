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
        public void CreatedProductsAreStored() {
            IActionResult result = controller.Create(new OrderInputView() {
                Id = 0, 
                Products = new List<ProductView>() {
                    new ProductView() {Id = 1, Amount = 1.1m, Category = new CategoryView() },
                    new ProductView() {Id = 2, Amount = 1.2m, Category = new CategoryView() }
                }
            });
            Assert.IsTrue(result is OkObjectResult);
            List<Order> allOrders = repository.Get().ToList();
            Assert.AreEqual(allOrders.Count, 1);
            Assert.IsNotNull(allOrders[0]);
            Assert.AreEqual(1, allOrders[0].Id);
            Assert.AreEqual(1, allOrders[0].OrderProducts[0].ProductId);
            Assert.AreEqual(1.1m, allOrders[0].OrderProducts[0].Product.ProductAmount.Amount);
            Assert.AreEqual(2, allOrders[0].OrderProducts[1].ProductId);
            Assert.AreEqual(1.2m, allOrders[0].OrderProducts[1].Product.ProductAmount.Amount);
        }


    }
}