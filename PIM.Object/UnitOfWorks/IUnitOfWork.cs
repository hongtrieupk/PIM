using NHibernate;
using PIM.Data.UnitOfWorks.GenericTransactions;
using System;
using System.Data;

namespace PIM.Data.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericTransaction BeginTransaction();
        IGenericTransaction BeginTransaction(IsolationLevel isolationLevel);
        void TransactionalFlush();
        void TransactionalFlush(IsolationLevel isolationLevel);
        bool IsInActiveTransaction { get; }
        IUnitOfWorkFactory Factory { get; }
        ISession Session { get; }
        void Flush();
    }
}
