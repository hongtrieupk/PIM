using NHibernate;
using NHibernate.Cfg;
using PIM.Data.Repositories.GenericTransactions;
using System;
using System.Data;

namespace PIM.Data.NHibernateConfiguration
{
    public interface IApplicationDbContext : IDisposable
    {
        Configuration Configuration { get; }
        IGenericTransaction BeginTransaction();
        ISession OpenSession();
        void Flush();
    }
}
