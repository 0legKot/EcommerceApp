using API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class ProductRepository:BaseRepository<Product>
    {
        public ProductRepository(StoreDbContext context) : base(context) { }
        public override IEnumerable<Product> Get(Expression<Func<Product, bool>> filter = null, Func<Product, object> orderBy = null, int skip = 0, int take = 0)
        {
            var result = CreateQuery(filter, orderBy, skip, take)
                .Include(product => product.ProductAmount)
                .Include(product => product.Category)
                .ToList();
            return result;
        }
        public override void Update(Product entityToUpdate)
        {
            Product product = context.Products.Include(product=>product.ProductAmount).First(product => product.Id == entityToUpdate.Id);
            if (entityToUpdate.Category != null) {
                Category category = context.Categories.Find(entityToUpdate.Category.Id);
                product.Category = category;
            }
            product.Name = entityToUpdate.Name;
            product.Description = entityToUpdate.Description;
            product.Price = entityToUpdate.Price;
            product.Rating = entityToUpdate.Rating;
            product.ProductAmount.Amount = entityToUpdate.ProductAmount?.Amount ?? 0;
            context.SaveChanges();
        }
        public override void Insert(Product entity)
        {
            if (entity.Category != null)
            {
                Category category = context.Categories.Find(entity.Category.Id);
                entity.Category = category;
            }
            base.Insert(entity);
        }
    }
}
