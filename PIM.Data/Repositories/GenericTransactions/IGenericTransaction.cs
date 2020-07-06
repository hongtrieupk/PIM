using System;
using System.Threading.Tasks;

namespace PIM.Data.Repositories.GenericTransactions
{
    public interface IGenericTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
