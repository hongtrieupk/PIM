using log4net;
using NHibernate;
using PIM.Common.CustomExceptions;

namespace PIM.Data.Repositories.GenericTransactions
{
    public class GenericTransaction : IGenericTransaction
    {
        #region Fields
        private static readonly ILog _logger = LogManager.GetLogger(typeof(GenericTransaction));
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
                _logger.Error("Error was happen when committing a transaction", exception);
                throw new ConcurrencyUpdateException(exception.Message, exception);
            }
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }
        public void Dispose()
        {
            _transaction.Dispose();
        }
        #endregion
    }
}
