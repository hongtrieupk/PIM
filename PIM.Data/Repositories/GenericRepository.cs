
using NHibernate;
using PIM.Common.CustomExceptions;
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
            return _session.Save(entity);
        }

        public void Update(T entity)
        {
            _session.Update(entity);
        }
        public void Delete(T entity)
        {
            _session.Delete(entity);
        }
        public T GetById(object id)
        {
            return _session.Get<T>(id);
        }
        public T LoadById(object id)
        {
            try
            {
                T result = _session.Load<T>(id);
                NHibernateUtil.Initialize(result);
                return result;
            }
            catch (ObjectNotFoundException notFoundException)
            {
                throw (new NotFoundFromDbException(notFoundException.Message, notFoundException));
            }
        }
        #endregion

    }
}
