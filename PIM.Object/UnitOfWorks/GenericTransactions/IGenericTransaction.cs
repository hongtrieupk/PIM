using System;
using System.Threading.Tasks;

namespace PIM.Data.UnitOfWorks.GenericTransactions
{
    public interface IGenericTransaction : IDisposable
    {
        void Commit();
        Task CommitAsync();
        void Rollback();
        Task RollbackAsync();
    }
}
