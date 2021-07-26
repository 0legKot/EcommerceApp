using API.Model;
using API.Repositories;
using API.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase {
        readonly IOrderRepository _repository;
        public OrderController(IOrderRepository repository) {
            _repository = repository;
        }

        [HttpGet("GetAllOrders")]
        public IEnumerable<OrderOutputView> GetAllOrders() {
            return _repository.Get().Select(order => new OrderOutputView(order)).ToList();
        }

        [HttpGet("GetById")]
        public OrderOutputView GetOrder(int orderid) {
            return new OrderOutputView(_repository.GetByID(orderid));
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] OrderInputView orderView) {
            if (orderView.Id != 0) {
                return BadRequest("Order Id should NOT be specified");
            }
            try {
                ValidateOrderView(orderView);
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }

            var newOrder = orderView.ToOrder();
            try { 
                _repository.Insert(newOrder);
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }
            return Ok(newOrder.Id);
        }

        [HttpPost("Update")]
        public IActionResult Update([FromBody] OrderInputView orderView) {
            if (orderView.Id == 0) {
                return BadRequest("Order Id should be specified");
            }
            try {
                ValidateOrderView(orderView);
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }

            try { 
                _repository.Update(orderView.ToOrder());
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        [HttpPost("Complete")]
        public IActionResult Complete(int orderid) {
            if (orderid == 0) {
                return BadRequest("Order Id should be specified");
            }
            try {
                _repository.ReleaseOrder(orderid);
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        private void ValidateOrderView(OrderInputView orderView) {
            if (orderView.Products.Any(product => product.Amount <= 0)) {
                throw new ApplicationException("Product Amount should be greater than 0");
            }          
        }
    }
}
