using System;
using System.Collections.Concurrent;
using Lfz.Caching;
using Lfz.Data.Nh.Providers;
using Lfz.Data.RawSql;
using Lfz.Logging;
using Lfz.Utitlies;
using NHibernate;

namespace Lfz.Data.Nh
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISessionFactoryHolder : ISingletonDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        ISessionFactory GetSessionFactory(IDbProviderConfig config);
    }

    /// <summary>
    /// 
    /// </summary>
    public class SessionFactoryHolder : ISessionFactoryHolder, IDisposable
    {
        private readonly IDataServicesProviderFactory _dataServicesProviderFactory;
        private readonly ConcurrentDictionary<Guid, ISessionFactory> _sessionFactories;
        private readonly ConcurrentDictionary<Guid, NHibernate.Cfg.Configuration> _configDictonary;
        private readonly object configLock = new object();
        private readonly FileCacheManager<NHibernate.Cfg.Configuration> _cacheManager;

        /// <summary>
        /// 
        /// </summary> 
        public SessionFactoryHolder(int cacheRegion)
        {
            _sessionFactories = new ConcurrentDictionary<Guid, ISessionFactory>();
            _configDictonary = new ConcurrentDictionary<Guid, NHibernate.Cfg.Configuration>();
            _dataServicesProviderFactory = new DataServicesProviderFactory();
            Logger = LoggerFactory.GetLog();
            _cacheManager = new FileCacheManager<NHibernate.Cfg.Configuration>(cacheRegion);
        }
        /// <summary>
        /// 
        /// </summary>
        public ILogger Logger { get; set; }

        public void Dispose()
        {
            foreach (var pair in _sessionFactories)
            {
                if (pair.Value != null)
                    pair.Value.Dispose();
            }
            _sessionFactories.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public ISessionFactory GetSessionFactory(IDbProviderConfig config)
        {
            ISessionFactory sessionFactory;
            lock (this)
            {
                _sessionFactories.TryGetValue(config.CustomerId, out sessionFactory);
                if (sessionFactory == null)
                {
                    sessionFactory = BuildSessionFactory(config);
                    _sessionFactories.AddOrUpdate(config.CustomerId, x => sessionFactory,
                        (x, y) => { return sessionFactory; });
                }
            }
            return sessionFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public NHibernate.Cfg.Configuration GetConfiguration(IDbProviderConfig config)
        {
            NHibernate.Cfg.Configuration configuration;
            lock (configLock)
            {
                _configDictonary.TryGetValue(config.CustomerId, out configuration);
                if (configuration == null)
                {
                    configuration = BuildConfiguration(config);
                    _configDictonary.AddOrUpdate(config.CustomerId, x => configuration,
                        (x, y) => { return configuration; });
                }
            }
            return configuration;
        }

        private ISessionFactory BuildSessionFactory(IDbProviderConfig providerConfig)
        {
            Logger.Debug("Building session factory");

            if (!Utils.IsFullTrust)
                NHibernate.Cfg.Environment.UseReflectionOptimizer = false;

            NHibernate.Cfg.Configuration config = GetConfiguration(providerConfig);
            var result = config.BuildSessionFactory();
            Logger.Debug("Done building session factory");
            return result;
        }

        private NHibernate.Cfg.Configuration BuildConfiguration(IDbProviderConfig config)
        {
            Logger.Debug("Building configuration");
            //TODO 缓存配置实现
            var providerService = _dataServicesProviderFactory.CreateProvider(config);
            var result= providerService.BuildConfiguration(config);
            Logger.Debug("Done Building configuration");
            return result;
        }
    }


}
