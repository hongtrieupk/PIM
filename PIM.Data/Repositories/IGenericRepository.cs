
using NHibernate;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PIM.Data.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        #region Methods
        void SetSession(ISession session);
        object Add(T entity);
		void Update(T entity);
        void Delete(T entity);
		T GetById(object id);
        IQueryable<T> FilterBy(Expression<Func<T, bool>> criteria);
        #endregion
    }
}
