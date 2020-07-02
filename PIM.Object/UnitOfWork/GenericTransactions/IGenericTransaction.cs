using System;
using System.Threading.Tasks;

namespace PIM.Object.UnitOfWork.GenericTransactions
{
    public interface IGenericTransaction : IDisposable
    {
        void Commit();
        Task CommitAsync();
        void Rollback();
        Task RollbackAsync();
    }
}
