using API.Model;
using API.Repositories;
using API.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase {
        readonly IRepository<Product> _repository;
        public ProductController(IRepository<Product> repository) {
            _repository = repository;
        }

        [HttpGet("Search")]
        public IEnumerable<ProductView> Search(int skip = 0, int take = 0, int categoryId = 0, SortType sortType = SortType.Rating, string searchText = "") {
            Expression<Func<Product, bool>> filter = null;

            if (!string.IsNullOrWhiteSpace(searchText)) {
                filter = (Product product) => product.Name.ToUpper().Contains(searchText.ToUpper()) || product.Description.ToUpper().Contains(searchText.ToUpper());
            }
            if (categoryId != 0) {
                filter = (Product product) => product.Category.Id == categoryId;
            }
            if (categoryId != 0 && !string.IsNullOrWhiteSpace(searchText)) {
                filter = (Product product) =>
                (product.Name.ToUpper().Contains(searchText.ToUpper()) ||
                product.Description.ToUpper().Contains(searchText.ToUpper()))
                && product.Category.Id == categoryId;
            }

            Func<Product, object> orderBy = sortType switch {
                SortType.Rating => (Product product) => -1 * product.Rating, //inverted sorting
                SortType.Price => (Product product) => product.Price,
                _ => throw new NotImplementedException("Unknown sort type"),
            };

            return _repository.Get(filter, orderBy, skip, take).Select(product => new ProductView(product)).ToList();
        }

        [HttpGet("GetById")]
        public ProductView GetProduct(int productId) {
            return new ProductView(_repository.GetByID(productId));
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] ProductView productView) {
            if (productView.Id != 0) {
                return BadRequest("Product Id should NOT be specified");
            }
            try {
                ValidateProductView(productView);
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }

            var newProduct = productView.ToProduct();
            try { 
                _repository.Insert(newProduct);
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }
            return Ok(newProduct.Id);
        }

        [HttpPost("Update")]
        public IActionResult Update([FromBody] ProductView productView) {
            if (productView.Id == 0) {
                return BadRequest("Product Id should be specified");
            }
            try {
                ValidateProductView(productView);
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }

            try {
                _repository.Update(productView.ToProduct());
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }
            return Ok();
        }
        private void ValidateProductView(ProductView productView) {
            if (productView.Price <= 0) {
                throw new ApplicationException("Product Price should be greater than 0");
            }
            if (productView.Amount <= 0) {
                throw new ApplicationException("Product Amount should be greater than 0");
            }
            if (productView.Rating <= 0) {
                throw new ApplicationException("Product Rating should be greater than 0");
            }
            if (string.IsNullOrWhiteSpace(productView.Name)) {
                throw new ApplicationException("Product Name should NOT be empty");
            }
        }
    }
}
