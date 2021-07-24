using API.Model;
using API.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Search")]
        public IEnumerable<ProductView> Search(int page = 1, int categoryId = 0, SortType sortType = SortType.Rating, string searchText = "")
        {
            return new List<ProductView>() { 
                new ProductView(
                    new Product()
                    { Id = 1, Description = "d", Name = "Name", Price = 1, Rating = 2.1, ProductAmount = new ProductAmount() { Amount = 10 }}  )
            };
        }

        [HttpGet("GetProduct")]
        public ProductView GetProduct(int productId)
        {
            return
                new ProductView(
                    new Product()
                    { Id = 1, Description = "d", Name = "Name", Price = 1, Rating = 2.1, ProductAmount = new ProductAmount() { Amount = 10 } });
            
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody]ProductView productView)
        {
            if (productView.Id != 0)
            {
                return BadRequest("Product Id should NOT be specified");
            }
            return Ok();
        }

        [HttpPost("Update")]
        public IActionResult Update([FromBody] ProductView productView)
        {
            if (productView.Id == 0) 
            {
                return BadRequest("Product Id should be specified");
            }
            return Ok();
        }

        [HttpGet("GetAllCategories")]
        public IEnumerable<Category> GetAllCategories()
        {
            return new List<Category>() {
                new Category() {Id = 1, Name = "MyCat"}
            };
        }

        [HttpPost("Delete")]
        public IActionResult Delete(int productId)
        {
            return Ok();
        }
    }
}
