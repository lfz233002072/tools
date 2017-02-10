using System;
using System.Collections.Generic;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Diagnostics;
using Lfz.Data.Nh.Conventions;
using Lfz.Data.RawSql;
using Lfz.Logging;
using Lfz.Utitlies;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;
using NHibernate.Tool.hbm2ddl;

namespace Lfz.Data.Nh.Providers
{
    [Serializable]
    public abstract partial class AbstractDataServicesProvider : IDataServicesProvider
    {
        public ILogger Logger { get; set; }
        private readonly IEnumerable<INhTypeProvider> _typeProviders;
 

        public abstract IPersistenceConfigurer GetPersistenceConfigurer(bool createDatabase);

        /// <summary>
        /// 要求所有表都要手动添加映射类
        /// </summary> 
        /// <returns></returns>
        public NHibernate.Cfg.Configuration BuildConfiguration(IDbProviderConfig config)
        {
            try
            {
                List<Type> typeList = new List<Type>();
                foreach (var provider in _typeProviders)
                { 
                    typeList.AddRange(provider.GetTypes());
                }
                var database = GetPersistenceConfigurer(false); 
                return Fluently.Configure()
                               .Database(database)
                                .Mappings(m => m.AutoMappings.Add(CreateAutomappings(typeList, config)))
                               .ExposeConfiguration(x => { ExposeConfig(config.CustomerId, x); })
                               .BuildConfiguration();
            }
            catch (Exception e)
            {
                Logger.Error(e, this.GetType().Name + "BuildConfiguration");
            }
            return null;
        }

        static AutoPersistenceModel CreateAutomappings(List<Type> typeList, IDbProviderConfig config)
        {
            // This is the actual automapping - use AutoMap to start automapping,
            // then pick one of the static methods to specify what to map (in this case
            // all the classes in the assembly that contains Employee), and then either
            // use the Setup and Where methods to restrict that behaviour, or (preferably)
            // supply a configuration instance of your definition to control the automapper.
            return AutoMap
                .Source(new TypeSource(typeList))
                .Conventions.Add<RecordTableNameConvention>(new RecordTableNameConvention(typeList))
                .Conventions.Add<EnumConvention>()
                .Conventions.Add<StringLengthConvention>()
                .Conventions.Add<PropertyConvertion>(new PropertyConvertion(config))
                .Conventions.Add<CacheConvention>(new CacheConvention(typeList))
            ;
        }

        private void ExposeConfig(Guid customerId, NHibernate.Cfg.Configuration cfg)
        {
            if (!Logger.IsEnabled(LogLevel.Trace)) return;
            var schemaExport = new SchemaExport(cfg);
            schemaExport.SetOutputFile(Utils.MapPath("~/App_Data/Scripts_" + customerId.ToString("n") + ".sql"));
            schemaExport.Execute(false, false, false);
            cfg.EventListeners.LoadEventListeners = new ILoadEventListener[] { new OrchardLoadEventListener() };
        }

        [Serializable]
        class TypeSource : ITypeSource
        {

            private readonly List<Type> typeList;

            public TypeSource(IEnumerable<Type> types)
            {
                typeList = new List<Type>(types);
            }

            public IEnumerable<Type> GetTypes()
            {
                return typeList;
            }

            public void LogSource(IDiagnosticLogger logger)
            {
                throw new NotImplementedException();
            }

            public string GetIdentifier()
            {
                throw new NotImplementedException();
            }
        }

        [Serializable]
        class OrchardLoadEventListener : DefaultLoadEventListener, ILoadEventListener
        {

            public new void OnLoad(LoadEvent @event, LoadType loadType)
            {
                var source = (ISessionImplementor)@event.Session;
                IEntityPersister entityPersister;
                if (@event.InstanceToLoad != null)
                {
                    entityPersister = source.GetEntityPersister(null, @event.InstanceToLoad);
                    @event.EntityClassName = @event.InstanceToLoad.GetType().FullName;
                }
                else
                    entityPersister = source.Factory.GetEntityPersister(@event.EntityClassName);
                if (entityPersister == null)
                    throw new HibernateException("Unable to locate persister: " + @event.EntityClassName);

                //a hack to handle unused ContentPartRecord proxies on ContentItemRecord or ContentItemVersionRecord.
                //I don't know why it actually works, or how to do it right

                //if (!entityPersister.IdentifierType.IsComponentType)
                //{
                //    Type returnedClass = entityPersister.IdentifierType.ReturnedClass;
                //    if (returnedClass != null && !returnedClass.IsInstanceOfType(@event.EntityId))
                //        throw new TypeMismatchException(string.Concat(new object[4]
                //    {
                //      (object) "Provided id of the wrong type. Expected: ",
                //      (object) returnedClass,
                //      (object) ", got ",
                //      (object) @event.EntityId.GetType()
                //    }));
                //}

                var keyToLoad = new EntityKey(@event.EntityId, entityPersister, source.EntityMode);

                if (loadType.IsNakedEntityReturned)
                {
                    @event.Result = Load(@event, entityPersister, keyToLoad, loadType);
                }
                else if (@event.LockMode == LockMode.None)
                {
                    @event.Result = ProxyOrLoad(@event, entityPersister, keyToLoad, loadType);
                }
                else
                {
                    @event.Result = LockAndLoad(@event, entityPersister, keyToLoad, loadType, source);
                }
            }
        }
    }
}