using NHibernate;
using NHibernate.Cfg;

namespace PIM.Data.UnitOfWorks
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
        Configuration Configuration { get; }
        ISessionFactory SessionFactory { get; }
        ISession CurrentSession { get; set; }
        void DisposeUnitOfWork();
    }
}
