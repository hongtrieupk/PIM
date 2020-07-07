using NHibernate;
using PIM.Common.CustomExceptions;

namespace PIM.Data.Repositories.GenericTransactions
{
    public class GenericTransaction : IGenericTransaction
    {
        #region Fields
        private readonly ITransaction _transaction;
        #endregion

        #region Constructors
        public GenericTransaction(ITransaction transaction)
        {
            _transaction = transaction;
        }
        #endregion

        #region Methods
        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch (StaleStateException exception)
            {
                _transaction.Rollback();
                throw new ConcurrencyDbException(exception.Message, exception);
            }
        }
        public void Rollback()
        {
            _transaction.Rollback();
        }
        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }
        }
        #endregion
    }
}
