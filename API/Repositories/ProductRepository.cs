using API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repositories {
    public class ProductRepository : BaseRepository<Product> {
        public ProductRepository(StoreDbContext context) : base(context) { }
        public override IEnumerable<Product> Get(Expression<Func<Product, bool>> filter = null, Func<Product, object> orderBy = null, int skip = 0, int take = 0) {
            var query = CreateQuery(filter, skip, take)
                .Include(product => product.ProductAmount)
                .Include(product => product.Category);
            if (orderBy != null) {
                return query.OrderBy(orderBy).ToList();
            }
            return query.ToList();
        }
        public override Product GetByID(int id) {
            return context.Products
                .Include(product => product.Category)
                .Include(product => product.ProductAmount)
                .FirstOrDefault(product => product.Id == id);
        }
        public override void Update(Product updatedEntity) {
            Product entityToUpdate = context.Products
                .Include(product => product.ProductAmount)
                .FirstOrDefault(product => product.Id == updatedEntity.Id) ?? throw new ApplicationException("No Products found");
            SetCategory(updatedEntity, entityToUpdate);
            entityToUpdate.Name = updatedEntity.Name;
            entityToUpdate.Description = updatedEntity.Description;
            entityToUpdate.Price = updatedEntity.Price;
            entityToUpdate.Rating = updatedEntity.Rating;
            entityToUpdate.ProductAmount.Amount = updatedEntity.ProductAmount?.Amount ?? 0;
            context.SaveChanges();
        }

        public override void Insert(Product entity) {
            SetCategory(entity, entity);
            base.Insert(entity);
        }
        private void SetCategory(Product updatedEntity, Product entityToUpdate) {
            if (updatedEntity.Category != null) {
                Category category = context.Categories.Find(updatedEntity.Category.Id) ?? throw new ApplicationException("Incorrect category");
                entityToUpdate.Category = category;
            }
        }
    }
}
