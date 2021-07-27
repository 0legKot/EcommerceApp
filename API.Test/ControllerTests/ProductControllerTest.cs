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

        [Test]
        public void ProductCanBeRetrievedById() {
            FillRepositoryWithBasicProducts();

            ProductView searchResult = controller.GetProduct(1);
            Assert.AreEqual(1, searchResult?.Id);
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
            Assert.AreEqual(1, allProducts.Count);
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
        public void NewProductCannotHaveId() {
            IActionResult result = controller.Create(new ProductView() {
                Id = 1,
                Name = "Test",
                Description = "Test2",
                Amount = 1.1m,
                Price = 1.2m,
                Rating = 1.3,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Id should NOT be specified", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public void NewProductCannotHaveNonPositivePrice() {
            IActionResult result = controller.Create(new ProductView() {
                Id = 0,
                Name = "Test",
                Description = "Test2",
                Amount = 1.1m,
                Price = 0m,
                Rating = 1.3,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Price should be greater than 0", (result as BadRequestObjectResult).Value);
            result = controller.Create(new ProductView() {
                Id = 0,
                Name = "Test",
                Description = "Test2",
                Amount = 1.1m,
                Price = -1m,
                Rating = 1.3,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Price should be greater than 0", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public void NewProductCannotHaveNonPositiveAmount() {
            IActionResult result = controller.Create(new ProductView() {
                Id = 0,
                Name = "Test",
                Description = "Test2",
                Amount = 0m,
                Price = 1.1m,
                Rating = 1.3,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Amount should be greater than 0", (result as BadRequestObjectResult).Value);
            result = controller.Create(new ProductView() {
                Id = 0,
                Name = "Test",
                Description = "Test2",
                Amount = -1.1m,
                Price = 1m,
                Rating = 1.3,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Amount should be greater than 0", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public void NewProductCannotHaveNonPositiveRating() {
            IActionResult result = controller.Create(new ProductView() {
                Id = 0,
                Name = "Test",
                Description = "Test2",
                Amount = 1m,
                Price = 1.1m,
                Rating = 0,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Rating should be greater than 0", (result as BadRequestObjectResult).Value);
            result = controller.Create(new ProductView() {
                Id = 0,
                Name = "Test",
                Description = "Test2",
                Amount = 1.1m,
                Price = 1m,
                Rating = -1.3,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Rating should be greater than 0", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public void NewProductCannotHaveRatingGreaterThanMaxRating() {
            IActionResult result = controller.Create(new ProductView() {
                Id = 0,
                Name = "Test",
                Description = "Test2",
                Amount = 1m,
                Price = 1.1m,
                Rating = ProductController.MaxRating + 0.1,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual($"Product Rating should NOT be greater than {ProductController.MaxRating}", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public void NewProductCannotHaveEmptyName() {
            IActionResult result = controller.Create(new ProductView() {
                Id = 0,
                Name = "",
                Description = "Test2",
                Amount = 1m,
                Price = 1.1m,
                Rating = 1.1,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual($"Product Name should NOT be empty", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public void UpdatedProductsAreStored() {
            FillRepositoryWithBasicProducts();
            IActionResult result = controller.Update(new ProductView() {
                Id = 1,
                Name = "Test",
                Description = "Test2",
                Amount = 1.4m,
                Price = 1.5m,
                Rating = 1.6,
                Category = new CategoryView() { Id = 2, Name = "TestCategory2" }
            });
            Assert.IsTrue(result is OkResult);
            Product product = repository.GetByID(1);
            Assert.IsNotNull(product);
            Assert.AreEqual(1, product.Id);
            Assert.AreEqual("Test", product.Name);
            Assert.AreEqual("Test2", product.Description);
            Assert.AreEqual(1.4m, product.ProductAmount?.Amount);
            Assert.AreEqual(1.5m, product.Price);
            Assert.AreEqual(1.6, product.Rating, 0.00001);
            Assert.AreEqual(2, product.Category?.Id);
        }

        [Test]
        public void UpdatedProductCannotHaveZeroId() {
            IActionResult result = controller.Update(new ProductView() {
                Id = 0,
                Name = "Test",
                Description = "Test2",
                Amount = 1.1m,
                Price = 1.2m,
                Rating = 1.3,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Id should be specified", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public void UpdatedProductCannotHaveNonPositivePrice() {
            IActionResult result = controller.Update(new ProductView() {
                Id = 1,
                Name = "Test",
                Description = "Test2",
                Amount = 1.1m,
                Price = 0m,
                Rating = 1.3,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Price should be greater than 0", (result as BadRequestObjectResult).Value);
            result = controller.Update(new ProductView() {
                Id = 1,
                Name = "Test",
                Description = "Test2",
                Amount = 1.1m,
                Price = -1m,
                Rating = 1.3,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Price should be greater than 0", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public void UpdatedProductCannotHaveNonPositiveAmount() {
            IActionResult result = controller.Update(new ProductView() {
                Id = 1,
                Name = "Test",
                Description = "Test2",
                Amount = 0m,
                Price = 1.1m,
                Rating = 1.3,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Amount should be greater than 0", (result as BadRequestObjectResult).Value);
            result = controller.Update(new ProductView() {
                Id = 1,
                Name = "Test",
                Description = "Test2",
                Amount = -1.1m,
                Price = 1m,
                Rating = 1.3,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Amount should be greater than 0", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public void UpdatedProductCannotHaveNonPositiveRating() {
            IActionResult result = controller.Update(new ProductView() {
                Id = 1,
                Name = "Test",
                Description = "Test2",
                Amount = 1m,
                Price = 1.1m,
                Rating = 0,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Rating should be greater than 0", (result as BadRequestObjectResult).Value);
            result = controller.Update(new ProductView() {
                Id = 1,
                Name = "Test",
                Description = "Test2",
                Amount = 1.1m,
                Price = 1m,
                Rating = -1.3,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual("Product Rating should be greater than 0", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public void UpdatedProductCannotHaveRatingGreaterThanMaxRating() {
            IActionResult result = controller.Update(new ProductView() {
                Id = 1,
                Name = "Test",
                Description = "Test2",
                Amount = 1m,
                Price = 1.1m,
                Rating = ProductController.MaxRating + 0.1,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual($"Product Rating should NOT be greater than {ProductController.MaxRating}", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public void UpdatedProductCannotHaveEmptyName() {
            IActionResult result = controller.Update(new ProductView() {
                Id = 1,
                Name = "",
                Description = "Test2",
                Amount = 1m,
                Price = 1.1m,
                Rating = 1.1,
                Category = new CategoryView() { Id = 1, Name = "TestCategory" }
            });
            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual($"Product Name should NOT be empty", (result as BadRequestObjectResult).Value);
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