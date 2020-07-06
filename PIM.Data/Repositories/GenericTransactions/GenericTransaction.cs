using NHibernate;
using System.Threading.Tasks;

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
            _transaction.Commit();
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
