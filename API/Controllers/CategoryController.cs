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
    public class CategoryController : ControllerBase {
        readonly IRepository<Category> _repository;
        public CategoryController(IRepository<Category> repository) {
            _repository = repository;
        }

        [HttpGet("GetAllCategories")]
        public IEnumerable<CategoryView> GetAllCategories() {
            return _repository.Get().Select(category => new CategoryView() { Id = category.Id, Name = category.Name });
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] CategoryView categoryView) {
            if (categoryView.Id != 0) {
                return BadRequest("Category Id should NOT be specified");
            }

            var newCategory = new Category() { Id = categoryView.Id, Name = categoryView.Name };
            try {  
                _repository.Insert(newCategory);
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }
            return Ok(newCategory.Id);
        }


    }
}
