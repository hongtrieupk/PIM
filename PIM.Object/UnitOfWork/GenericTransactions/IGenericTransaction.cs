using System;

namespace PIM.Object.UnitOfWork.GenericTransactions
{
    public interface IGenericTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
