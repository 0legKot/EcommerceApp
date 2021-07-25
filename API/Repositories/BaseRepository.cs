using API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace API.Repositories {
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity {
        internal StoreDbContext context;
        internal DbSet<TEntity> dbSet;

        public BaseRepository(StoreDbContext context) {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<TEntity, object> orderBy = null, int skip = 0, int take = 0) {
            IQueryable<TEntity> query = CreateQuery(filter, skip, take);
            if (orderBy != null) {
                return query.OrderBy(orderBy);
            }
            return query;
        }

        protected IQueryable<TEntity> CreateQuery(Expression<Func<TEntity, bool>> filter, int skip, int take) {
            IQueryable<TEntity> query = dbSet;

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

        public virtual TEntity GetByID(int id) {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity) {
            dbSet.Add(entity);
            context.SaveChanges();
        }

        public virtual void Delete(int id) {
            dbSet.Remove(dbSet.Find(id));
            context.SaveChanges();
        }

        public virtual void Update(TEntity entityToUpdate) {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}