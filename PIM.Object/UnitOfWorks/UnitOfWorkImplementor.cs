using NHibernate;
using PIM.Data.UnitOfWorks.GenericTransactions;
using System.Data;

namespace PIM.Data.UnitOfWorks
{    public class UnitOfWorkImplementor : IUnitOfWork
    {
        #region Fields
        private readonly IUnitOfWorkFactory _factory;
        private readonly ISession _session;
        #endregion
        #region Constructors
        public UnitOfWorkImplementor(IUnitOfWorkFactory factory, ISession session)
        {
            _factory = factory;
            _session = session;
        }
        #endregion
        #region Properties
        public bool IsInActiveTransaction
        {
            get
            {
                return _session.Transaction.IsActive;
            }
        }

        public IUnitOfWorkFactory Factory
        {
            get { return _factory; }
        }

        public ISession Session
        {
            get { return _session; }
        }
        #endregion
        #region Methods
        public IGenericTransaction BeginTransaction()
        {
            return new GenericTransaction(_session.BeginTransaction());
        }
        public IGenericTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return new GenericTransaction(_session.BeginTransaction(isolationLevel));
        }
 
        public void Flush()
        {
            _session.Flush();
        }
        public void Dispose()
        {
            _factory.DisposeUnitOfWork();
            _session.Dispose();
        }
        #endregion
    }
}
