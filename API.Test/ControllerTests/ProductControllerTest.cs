using NUnit.Framework;
using API.Model;
using API.Repositories;
using API.Controllers;
using API.Views;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace API.Test {
    public class ProductControllerTest {
        IRepository<Product> repository;
        ProductController controller;
        [SetUp]
        public void Setup() {
            repository = new FakeRepository<Product>();
            controller = new ProductController(repository);
        }

        [Test]
        public void CreatedProductsAreStored() {
            IActionResult result = controller.Create(new ProductView() {
                Name = "Test",
                Description = "Test2",
                Amount = 1.1m,
                Price = 1.2m,
                Rating = 1.3,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is OkObjectResult);
            List<Product> allProducts = repository.Get().ToList();
            Assert.AreEqual(allProducts.Count, 1);
            Assert.IsNotNull(allProducts[0]);
            Assert.AreEqual(1, allProducts[0].Id);
            Assert.AreEqual("Test", allProducts[0].Name);
            Assert.AreEqual("Test2", allProducts[0].Description);
            Assert.AreEqual(1.1m, allProducts[0].ProductAmount?.Amount);
            Assert.AreEqual(1.2m, allProducts[0].Price);
            Assert.AreEqual(1.3, allProducts[0].Rating, 0.00001);
            Assert.AreEqual(1, allProducts[0].Category?.Id);
        }

        [Test]
        public void StoredProductsAreShownPaginatedFilteredByCategory() {
            FillRepositoryWithBasicProducts();

            List<ProductView> searchResult = controller.Search(1, 1, 2).ToList();
            Assert.AreEqual(1, searchResult.Count);
            Assert.IsNotNull(searchResult[0]);
            Assert.AreEqual(3, searchResult[0].Id);
            Assert.AreEqual("Test3", searchResult[0].Name);
            Assert.AreEqual("TestDescription3", searchResult[0].Description);
            Assert.AreEqual(1.3m, searchResult[0].Amount);
            Assert.AreEqual(1.3m, searchResult[0].Price);
            Assert.AreEqual(1.3, searchResult[0].Rating, 0.00001);
            Assert.AreEqual(2, searchResult[0].Category?.Id);
        }

        [Test]
        public void StoredProductsAreSortedByPriceCorrectly() {
            FillRepositoryWithBasicProducts();

            List<ProductView> searchResult = controller.Search(sortType: SortType.Price).ToList();
            Assert.AreEqual(3, searchResult.Count);
            Assert.AreEqual(1.1m, searchResult[0].Price);
            Assert.AreEqual(1.2m, searchResult[1].Price);
            Assert.AreEqual(1.3m, searchResult[2].Price);
        }

        [Test]
        public void StoredProductsAreSortedByRatingCorrectly() {
            FillRepositoryWithBasicProducts();

            List<ProductView> searchResult = controller.Search(sortType: SortType.Rating).ToList();
            Assert.AreEqual(3, searchResult.Count);
            Assert.AreEqual(1.3, searchResult[0].Rating);
            Assert.AreEqual(1.2, searchResult[1].Rating);
            Assert.AreEqual(1.1, searchResult[2].Rating);
        }

        [Test]
        public void StoredProductsAreFilteredByText() {
            FillRepositoryWithBasicProducts();
            var product4 = new Product() {
                Id = 1,
                Name = "T_e_s_t4",
                Description = "Description4",
                Price = 1.1m,
                Rating = 1.1,
                Category = new Category() { Id = 1, Name = "TestCategory1" }
            };
            product4.ProductAmount = new ProductAmount() { Amount = 1.1m, Product = product4 };
            repository.Insert(product4);
            string searchText = "Test";
            List<ProductView> searchResult = controller.Search(searchText: searchText).ToList();
            Assert.AreEqual(3, searchResult.Count);
            Assert.IsTrue(searchResult.All(productView => productView.Name.Contains(searchText,
                System.StringComparison.OrdinalIgnoreCase) ||
                productView.Description.Contains(searchText, System.StringComparison.OrdinalIgnoreCase)
                ));
        }

        private void FillRepositoryWithBasicProducts() {
            var product1 = new Product() {
                Id = 1,
                Name = "test1",
                Description = "testDescription1",
                Price = 1.1m,
                Rating = 1.1,
                Category = new Category() { Id = 1, Name = "TestCategory1" }
            };
            product1.ProductAmount = new ProductAmount() { Amount = 1.1m, Product = product1 };
            var product2 = new Product() {
                Id = 2,
                Name = "Tst2",
                Description = "TestDescription2",
                Price = 1.2m,
                Rating = 1.2,
                Category = new Category() { Id = 2, Name = "TestCategory2" }
            };
            product2.ProductAmount = new ProductAmount() { Amount = 1.2m, Product = product2 };
            var product3 = new Product() {
                Id = 3,
                Name = "Test3",
                Description = "TestDescription3",
                Price = 1.3m,
                Rating = 1.3,
                Category = new Category() { Id = 2, Name = "TestCategory2" }
            };
            product3.ProductAmount = new ProductAmount() { Amount = 1.3m, Product = product3 };

            repository.Insert(product1);
            repository.Insert(product2);
            repository.Insert(product3);
        }
    }
}