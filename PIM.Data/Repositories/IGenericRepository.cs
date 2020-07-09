
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return null in case of no record founded</returns>
		T GetById(object id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>throw ObjectNotFoundException exception in case of no record founded</returns>
        T LoadById(object id);
        #endregion
    }
}
