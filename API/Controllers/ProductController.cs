﻿using API.Model;
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
                filter = (Product product) => product.Name.Contains(searchText) || product.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase);
            }
            if (categoryId != 0) {
                filter = (Product product) => product.Category.Id == categoryId;
            }
            if (categoryId != 0 && !string.IsNullOrWhiteSpace(searchText)) {
                filter = (Product product) =>
                (product.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                product.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase))
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
                _repository.Insert(productView.ToProduct());
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        [HttpPost("Update")]
        public IActionResult Update([FromBody] ProductView productView) {
            if (productView.Id == 0) {
                return BadRequest("Product Id should be specified");
            }
            try {
                _repository.Update(productView.ToProduct());
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        [HttpDelete("Delete")]
        public IActionResult Delete(int productId) {
            try { 
                _repository.Delete(productId);
            } catch (ApplicationException e) {
                return BadRequest(e.Message);
            }
            return Ok();
        }
    }
}
