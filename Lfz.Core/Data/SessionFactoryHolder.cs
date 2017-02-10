using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac.Features.Metadata;
using NHibernate;
using NHibernate.Cfg;
using PMSoft.Config;
using PMSoft.Data.Providers;
using PMSoft.Logging;
using PMSoft.Utitlies;

namespace PMSoft.Data
{
    /// <summary>
    /// 数据库Session工厂控制器
    /// </summary>
    public interface ISessionFactoryHolder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ISessionFactory GetSessionFactory();

        /// <summary>
        /// 获取数据库配置
        /// </summary>
        /// <returns></returns>
        Configuration GetConfiguration();
        /// <summary>
        /// 获取数据库Session参数
        /// </summary>
        /// <returns></returns>
        SessionFactoryParameters GetSessionFactoryParameters();
    }

    /// <summary>
    /// 数据库Session工厂
    /// </summary>
    public class SessionFactoryHolder : ISessionFactoryHolder, IDisposable
    {
        private readonly IDatabaseCacheConfiguration _cacheConfiguration;
        private readonly ShellSettings _shellSettings;
        private readonly IDataServicesProviderFactory _dataServicesProviderFactory;
        private readonly ISessionConfigurationCache _sessionConfigurationCache;

        private ISessionFactory _sessionFactory;
        private Configuration _configuration;
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shellSettings"></param>
        public SessionFactoryHolder(
            ShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
            _dataServicesProviderFactory = new DataServicesProviderFactory(new[] {
                            new Meta<CreateDataServicesProvider>(
                                (dataFolder, connectionString) => new SqlServerDataServicesProvider(dataFolder, connectionString), 
                                new Dictionary<string, object> {{"ProviderName", "SqlServer"}}),
                            new Meta<CreateDataServicesProvider>(
                                (dataFolder, connectionString) => new SqlCeDataServicesProvider(dataFolder, connectionString),
                                new Dictionary<string, object> {{"ProviderName", "SqlCe"}}),
                            new Meta<CreateDataServicesProvider>(
                                (dataFolder, connectionString) => new MySqlDataServicesProvider(dataFolder, connectionString), 
                                new Dictionary<string, object> {{"ProviderName", "MySql"}})
                        });
            _sessionConfigurationCache = new SessionConfigurationCache(_shellSettings);
            _cacheConfiguration = new DefaultDatabaseCacheConfiguration();

            Logger = LoggerFactory.GetLog();
        }

        /// <summary>
        /// 
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// 是否是有资源
        /// </summary>
        public void Dispose()
        {
            if (_sessionFactory != null)
            {
                _sessionFactory.Dispose();
                _sessionFactory = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ISessionFactory GetSessionFactory()
        {
            lock (this)
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = BuildSessionFactory();
                }
            }
            return _sessionFactory;
        }

        /// <summary>
        /// 获取数据库配置
        /// </summary>
        /// <returns></returns>
        public Configuration GetConfiguration()
        {
            lock (this)
            {
                if (_configuration == null)
                {
                    _configuration = BuildConfiguration();
                }
            }
            return _configuration;
        }

        private ISessionFactory BuildSessionFactory()
        {
            Logger.Debug("Building session factory");

            if (!Utils.IsFullTrust)
                NHibernate.Cfg.Environment.UseReflectionOptimizer = false;

            Configuration config = GetConfiguration();
            var result = config.BuildSessionFactory();
            Logger.Debug("Done building session factory");
            return result;
        }

        private Configuration BuildConfiguration()
        {
            Logger.Debug("Building configuration");
            var parameters = GetSessionFactoryParameters();

            var config = _sessionConfigurationCache.GetConfiguration(() =>
                _dataServicesProviderFactory
                    .CreateProvider(parameters)
                    .BuildConfiguration(parameters)
                .Cache(c => _cacheConfiguration.Configure(c))
            );

            #region NH specific optimization
            // cannot be done in fluent config
            // the IsSelectable = false prevents unused ContentPartRecord proxies from being created 
            // for each ContentItemRecord or ContentItemVersionRecord.
            // done for perf reasons - has no other side-effect

            //foreach (var persistentClass in config.ClassMappings)
            //{
            //    if (persistentClass.EntityName.StartsWith("Orchard.ContentManagement.Records."))
            //    {
            //        foreach (var property in persistentClass.PropertyIterator)
            //        {
            //            if (property.Name.EndsWith("Record") && !property.IsBasicPropertyAccessor)
            //            {
            //                property.IsSelectable = false;
            //            }
            //        }
            //    }
            //}
            #endregion

            Logger.Debug("Done Building configuration");
            return config;
        }

        /// <summary>
        /// 获取数据库Session参数
        /// </summary>
        /// <returns></returns>
        public SessionFactoryParameters GetSessionFactoryParameters()
        {
            return new SessionFactoryParameters
            {
                Provider = _shellSettings.Settings.DataProvider,
                DataFolder = _shellSettings.Settings.DataFolder,
                ConnectionString = _shellSettings.Settings.DataConnectionString,
                RecordDescriptors = _shellSettings.RecordBlueprints,
                TablePrefix = _shellSettings.Settings.TablePrefix,
            };
        }

    }


}
