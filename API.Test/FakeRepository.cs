using API.Model;
using API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace API.Test {
    class FakeRepository<TEntity> : IRepository<TEntity> where TEntity : IEntity {
        private List<TEntity> entities = new List<TEntity>();
        public void Delete(int id) {
            TEntity found = entities.First(entity => entity.Id == id);
            entities.Remove(found);
        }
        IQueryable<TEntity> CreateQuery(Expression<Func<TEntity, bool>> filter, int skip, int take) {
            IQueryable<TEntity> query = entities.AsQueryable();

            if (filter != null) {
                query = query.Where(filter);
            }

            if (skip != 0) {
                query = query.Skip(skip);
            }

            if (take != 0) {
                query = query.Take(take);
            }

            return query;
        }
        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<TEntity, object> orderBy = null, int skip = 0, int take = 0) {
            IQueryable<TEntity> query = CreateQuery(filter, skip, take);
            if (orderBy != null) {
                return query.OrderBy(orderBy);
            }
            return query;
        }

        public TEntity GetByID(int id) {
            return entities.First(entity => entity.Id == id);
        }

        public void Insert(TEntity entity) {
            entity.Id = entities.Count + 1;
            entities.Add(entity);
        }

        public void Update(TEntity entityToUpdate) {
            TEntity found = entities.First(entity => entity.Id == entityToUpdate.Id);
            entities.Remove(found);
            entities.Add(entityToUpdate);
        }
    }
}
