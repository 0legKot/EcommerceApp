using NUnit.Framework;
using API.Model;
using API.Repositories;
using API.Controllers;
using API.Views;
using System.Linq;
using System.Collections.Generic;

namespace API.Test
{
    public class CategoryControllerTest
    {
        IRepository<Category> repository;
        CategoryController controller;
        [SetUp]
        public void Setup()
        {
            repository = new FakeRepository<Category>();
            controller = new CategoryController(repository);
        }

        [Test]
        public void CreatedCategoriesAreStored()
        {
            controller.Create(new CategoryView() { Id = 1, Name = "Test" });
            var allCategories = repository.Get().ToList();
            Assert.IsTrue(allCategories.Count == 1);
            Assert.IsTrue(allCategories[0].Id == 1);
            Assert.IsTrue(allCategories[0].Name == "Test");
        }

        [Test]
        public void StoredCategoriesAreShown()
        {
            var category1 = new Category() { Id = 1, Name = "Test1" };
            var category2 = new Category() { Id = 2, Name = "Test2" };
            repository.Insert(category1);
            repository.Insert(category2);
            List<CategoryView> allCategories = controller.GetAllCategories().ToList();
            Assert.IsTrue(allCategories.Count == 2);
            Assert.IsTrue(allCategories.Any(category => category.Id == category1.Id && category.Name == category1.Name));
            Assert.IsTrue(allCategories.Any(category => category.Id == category2.Id && category.Name == category2.Name));
        }
    }
}