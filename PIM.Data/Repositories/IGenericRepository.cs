
using NHibernate;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PIM.Data.Repositories
{
    /// <summary>
    /// SetSession method must be call to inject ISession to the Repository by  before using 
    /// </summary>
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
