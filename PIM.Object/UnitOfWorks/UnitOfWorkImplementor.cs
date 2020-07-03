using NHibernate;
using PIM.Data.UnitOfWorks.GenericTransactions;
using System.Data;

namespace PIM.Data.UnitOfWorks
{    public class UnitOfWorkImplementor : IUnitOfWork
    {
        private readonly IUnitOfWorkFactory _factory;
        private readonly ISession _session;

        public UnitOfWorkImplementor(IUnitOfWorkFactory factory, ISession session)
        {
            _factory = factory;
            _session = session;
        }
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
        public IGenericTransaction BeginTransaction()
        {
            return new GenericTransaction(_session.BeginTransaction());
        }
        public IGenericTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return new GenericTransaction(_session.BeginTransaction(isolationLevel));
        }
        public void TransactionalFlush()
        {
            TransactionalFlush(IsolationLevel.ReadCommitted);
        }
        public void TransactionalFlush(IsolationLevel isolationLevel)
        {
            IGenericTransaction tx = BeginTransaction(isolationLevel);
            try
            {
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
            finally
            {
                tx.Dispose();
            }
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
    }
}
