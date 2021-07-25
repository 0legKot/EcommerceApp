using API.Model;
using API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace API.Test
{
    class FakeRepository<TEntity> : IRepository<TEntity> where TEntity : IEntity
    {
        private List<TEntity> entities = new List<TEntity>();
        public void Delete(int id)
        {
            TEntity found = entities.First(entity => entity.Id == id);
            entities.Remove(found);
        }
         IQueryable<TEntity> CreateQuery(Expression<Func<TEntity, bool>> filter, Func<TEntity, object> orderBy, int skip, int take)
        {
            IQueryable<TEntity> query = entities.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (skip != 0)
            {
                query.Skip(skip);
            }

            if (take != 0)
            {
                query.Take(take);
            }

            if (orderBy != null)
            {
                query.OrderBy(orderBy);
            }
            return query;
        }
        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<TEntity, object> orderBy = null, int skip = 0, int take = 0)
        {
            return CreateQuery(filter, orderBy, skip, take).ToList();
        }

        public TEntity GetByID(int id)
        {
            return entities.First(entity => entity.Id == id);
        }

        public void Insert(TEntity entity)
        {
            entities.Add(entity);
        }

        public void Update(TEntity entityToUpdate)
        {
            TEntity found = entities.First(entity => entity.Id == entityToUpdate.Id);
            entities.Remove(found);
            entities.Add(entityToUpdate); 
        }
    }
}
