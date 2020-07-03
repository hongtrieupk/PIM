using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using PIM.Common.Constants;
using PIM.Object.Maps;
using System;
using System.Data;

namespace PIM.Object.UnitOfWork
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private static ISession _currentSession;
        private ISessionFactory _sessionFactory;
        private static Configuration _nhibernateConfiguration;
        internal UnitOfWorkFactory()
        { }
        public Configuration Configuration
        {
            get
            {
                if (_nhibernateConfiguration == null)
                {
                    _nhibernateConfiguration = ConfigureNHibernate();
                }
                return _nhibernateConfiguration;
            }
        }
        public ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    _sessionFactory = Configuration.BuildSessionFactory();
                return _sessionFactory;
            }
        }
        public ISession CurrentSession
        {
            get
            {
                if (_currentSession == null)
                    throw new InvalidOperationException("You are not in a unit of work.");
                return _currentSession;
            }
            set { _currentSession = value; }
        }
        public IUnitOfWork Create()
        {
            ISession session = CreateSession();
            session.FlushMode = FlushMode.Commit;
            _currentSession = session;
            return new UnitOfWorkImplementor(this, session);
        }
        private ISession CreateSession()
        {
            return SessionFactory.OpenSession();
        }
        private static Configuration ConfigureNHibernate()
        {
            var configure = new Configuration();
            configure.SessionFactoryName("BuildIt");

            configure.DataBaseIntegration(db => {
                db.Driver<SqlClientDriver>();
                db.Dialect<MsSql2008Dialect>();
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                db.IsolationLevel = IsolationLevel.ReadCommitted;
                db.ConnectionStringName = GlobalConfigurationConstants.CONNECT_STRING_NAME;
                db.Timeout = 10;

                // enabled for testing
                db.LogFormattedSql = true;
                db.LogSqlInConsole = true;
                db.AutoCommentSql = true;
                db.BatchSize = GlobalConfigurationConstants.ADONET_BATCH_SIZE;
            });
            var mapping = GetMappings();
            configure.AddDeserializedMapping(mapping, "NHSchemaTest");
            var dialect = Dialect.GetDialect(configure.Properties);
            SchemaMetadataUpdater.QuoteTableAndColumns(configure, dialect);

            return configure;
        }
        private static HbmMapping GetMappings()
        {
            ModelMapper mapper = new ModelMapper();

            mapper.AddMapping<ProjectMap>();
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            return mapping;
        }

        public void DisposeUnitOfWork()
        {
            CurrentSession = null;
            UnitOfWork.DisposeUnitOfWork();
        }
    }
}
