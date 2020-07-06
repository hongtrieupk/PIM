
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PIM.Data.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        #region Methods
        object Add(T entity);
		void Update(T entity);
        void Delete(T entity);
		T GetById(object id);
        /// <summary>
        /// Filter with pagination function
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="pageIndex"> must greater than 0 </param>
        /// <param name="pageSize"> must greater than 0 </param>
        /// <returns></returns>
        IQueryable<T> FilterBy(Expression<Func<T, bool>> criteria);
        #endregion
    }
}
