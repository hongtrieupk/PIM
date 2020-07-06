

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using PIM.Data.Maps;
using PIM.Data.NHibernateConfiguration;
using PIM.Data.Repositories.GenericTransactions;
using System;
using System.Data;

namespace PIM.Data.NHibernateConfiguration
{
    public class ApplicationDbContext : IApplicationDbContext
    {
        #region Fields
        private static ISessionFactory _sessionFactory;
        private static ISession _currentSession;
        private static string _connectionStringName = "ProjectExcerciseContext";
        private static Configuration _nhibernateConfiguration;
        #endregion
        #region Constructor
        public ApplicationDbContext()
        {
            CreateSession();
        }
        public ApplicationDbContext(string connectionStringName)
        {
            _connectionStringName = !string.IsNullOrWhiteSpace(_connectionStringName) ? connectionStringName : _connectionStringName;
            CreateSession();
        }
        #endregion
        #region Properties
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
        public bool IsInActiveTransaction
        {
            get
            {
                return _currentSession.Transaction.IsActive;
            }
        }
        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    AppConfigure();

                return _sessionFactory;
            }
        }
        #endregion
        #region Methods
        public IGenericTransaction BeginTransaction()
        {
            return new GenericTransaction(_currentSession.BeginTransaction());
        }
        public IGenericTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return new GenericTransaction(_currentSession.BeginTransaction(isolationLevel));
        }

        public void Flush()
        {
            _currentSession.Flush();
        }
        public void Dispose()
        {
            _currentSession.Dispose();
            _sessionFactory.Dispose();
        }
        private void CreateSession()
        {
            _currentSession =  SessionFactory.OpenSession();
        }

        public static void AppConfigure()
        {
            _nhibernateConfiguration = ConfigureNHibernate();

            _sessionFactory = _nhibernateConfiguration.BuildSessionFactory();
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
                db.ConnectionStringName = _connectionStringName;
                db.Timeout = 10;

                // enabled for testing
                db.LogFormattedSql = true;
                db.LogSqlInConsole = true;
                db.AutoCommentSql = true;
            });

            var mapping = GetMappings();
            configure.AddDeserializedMapping(mapping, "NHSchemaTest");
            Dialect dialect = Dialect.GetDialect(configure.Properties);
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
        #endregion

    }
}
