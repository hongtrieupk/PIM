
using NHibernate;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PIM.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        #region Fields
        protected readonly ISession _session;
        #endregion
        #region Constructors
        public GenericRepository(ISession session)
        {
            _session = session ?? throw new ArgumentNullException("Can not inject a null ISession");
        }
        #endregion
        #region Methods
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
        /// <summary>
        /// Filter with pagination function
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="pageIndex"> must greater than 0 </param>
        /// <param name="pageSize"> must greater than 0 </param>
        /// <returns></returns>
        public IQueryable<T> FilterBy(Expression<Func<T, bool>> criteria)
        {
            return _session.Query<T>().Where(criteria);
        }
        #endregion

    }
}
