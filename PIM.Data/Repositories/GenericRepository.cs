
using NHibernate;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PIM.Data.Repositories
{
    /// <summary>
    /// SetSession method must be call to inject ISession to the Repository by  before using 
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        #region Fields        
        protected ISession _session;
        #endregion
        #region Constructors
        #endregion
        #region Methods
        public void SetSession(ISession session)
        {
            _session = session ?? throw new ArgumentNullException("Can not inject a null ISession");
        }
        public object Add(T entity)
        {
            return  _session.Save(entity);
        }

        public  void Update(T entity)
        {
             _session.Update(entity);
        }
        public  void Delete(T entity)
        {
            _session.Delete(entity);
        }
        public T GetById(object id)
        {
            return _session.Get<T>(id);
        }
        public IQueryable<T> FilterBy(Expression<Func<T, bool>> criteria)
        {
            return _session.Query<T>().Where(criteria);
        }
        #endregion

    }
}
