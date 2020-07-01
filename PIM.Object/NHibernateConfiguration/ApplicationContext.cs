

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using System.Data;

namespace PIM.Object.NHibernateConfiguration
{
    public class ApplicationContext
    {
        #region Fields
        private static ISessionFactory _sessionFactory;
        private const string CONNECT_STRING_NAME = "ProjectExcerciseContext";
        private static Configuration _nhibernateConfiguration;
        #endregion

        #region Methods
        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    AppConfigure();

                return _sessionFactory;
            }
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
                db.ConnectionStringName = CONNECT_STRING_NAME;
                db.Timeout = 10;

                // enabled for testing
                db.LogFormattedSql = true;
                db.LogSqlInConsole = true;
                db.AutoCommentSql = true;
            });

            var mapping = GetMappings();
            configure.AddDeserializedMapping(mapping, "NHSchemaTest");
            SchemaMetadataUpdater.QuoteTableAndColumns(configure, DB2400Dialect.GetDialect());

            return configure;
        }
        #endregion

    }
}
