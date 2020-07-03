
using NHibernate;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
        public async Task<object> AddAsync(T entity)
        {
            return await _session.SaveAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            await _session.UpdateAsync(entity);
        }
        public async Task DeleteAsync(T entity)
        {
            await _session.DeleteAsync(entity);
        }
        public void DeleteByIds(object[] ids)
        {
           
        }
        public async Task<T> GetByIdAsync(object id)
        {
            return await _session.GetAsync<T>(id);
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
