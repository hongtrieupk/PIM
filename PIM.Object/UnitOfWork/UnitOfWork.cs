
using NHibernate;
using NHibernate.Cfg;
using System;

namespace PIM.Object.UnitOfWork
{
    public static class UnitOfWork
    {
        private static IUnitOfWorkFactory _unitOfWorkFactory = new UnitOfWorkFactory();
        private static IUnitOfWork _innerUnitOfWork;

        public static IUnitOfWork Start()
        {
            _innerUnitOfWork = _unitOfWorkFactory.Create();
            return _innerUnitOfWork;
        }

        public static IUnitOfWork Current
        {
            get
            {
                if (_innerUnitOfWork == null)
                    throw new InvalidOperationException("You are not in a unit of work.");
                return _innerUnitOfWork;
            }
        }
        public static ISession CurrentSession
        {
            get { return _unitOfWorkFactory.CurrentSession; }
            internal set { _unitOfWorkFactory.CurrentSession = value; }
        }
        public static bool IsStarted
        {
            get { return _innerUnitOfWork != null; }
        }
        public static Configuration Configuration
        {
            get { return _unitOfWorkFactory.Configuration; }
        }
        public static void DisposeUnitOfWork(IUnitOfWork unitOfWork)
        {
            _innerUnitOfWork = null;
        }
    }
}
