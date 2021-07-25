using API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace API.Repositories {
    public interface IRepository<TEntity> where TEntity : IEntity {
        void Delete(int id);
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<TEntity, object> orderBy = null, int skip = 0, int take = 0);
        TEntity GetByID(int id);
        void Insert(TEntity entity);
        void Update(TEntity entityToUpdate);
    }
}