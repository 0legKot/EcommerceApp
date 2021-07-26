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
        readonly OrderRepository _repository;
        public OrderController(OrderRepository repository) {
            _repository = repository;
        }

        [HttpGet("GetAllOrders")]
        public IEnumerable<OrderView> GetAllOrders() {
            var result = _repository.Get();
            return result.Select(order => new OrderView(order)).ToList();
        }

        [HttpGet("GetById")]
        public OrderView GetOrder(int orderid) {
            return new OrderView(_repository.GetByID(orderid));
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] OrderView orderView) {
            if (orderView.Id != 0) {
                return BadRequest("Order Id should NOT be specified");
            }
            try { 
                _repository.Insert(orderView.ToOrder());
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        [HttpPost("Update")]
        public IActionResult Update([FromBody] OrderView orderView) {
            if (orderView.Id == 0) {
                return BadRequest("Order Id should be specified");
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
    }
}
