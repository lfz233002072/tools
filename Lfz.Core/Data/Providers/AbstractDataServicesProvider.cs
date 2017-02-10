using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Diagnostics;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;
using NHibernate.Tool.hbm2ddl;
using PMSoft.Data.Conventions;
using PMSoft.Logging;
using PMSoft.Utitlies;

namespace PMSoft.Data.Providers
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class AbstractDataServicesProvider : IDataServicesProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected AbstractDataServicesProvider()
        {
            Logger = LoggerFactory.GetLog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createDatabase"></param>
        /// <returns></returns>
        public abstract IPersistenceConfigurer GetPersistenceConfigurer(bool createDatabase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Configuration BuildConfiguration(SessionFactoryParameters parameters)
        {
            try
            {
                var database = GetPersistenceConfigurer(parameters.CreateDatabase);
                var assemblies = parameters.RecordDescriptors.Select(x => x.Type.Assembly).Distinct();
                return Fluently.Configure()
                               .Database(database)
                               .Mappings(m =>
                                   {
                                       foreach (var assembly in assemblies)
                                       {
                                           m.FluentMappings.AddFromAssembly(assembly);
                                       }
                                       m.FluentMappings
                                        .Conventions.AddSource(new TypeSource(parameters.RecordDescriptors))
                                        .Conventions.Setup(x => x.Add(AutoImport.Never()))
                                        .Conventions.Add(new RecordTableNameConvention(parameters))
                                        .Conventions.Add(new CacheConvention(parameters.RecordDescriptors))
                                        .Conventions.AddFromAssemblyOf<DataModule>();
                                   })
                               .ExposeConfiguration(ExposeConfig)
                               .BuildConfiguration();
            }
            catch (Exception e)
            {
                Logger.Error(e, "AbstractDataServicesProvider.BuildConfiguration");               
            }
            return null;
        }

        private void ExposeConfig(Configuration cfg)          
        {
            try
            {
                var schemaExport = new SchemaExport(cfg);
                schemaExport.SetOutputFile(Utils.MapPath("Config/Scripts.sql"));
                schemaExport.Execute(false, false, false);
                cfg.EventListeners.LoadEventListeners = new ILoadEventListener[] { new OrchardLoadEventListener() };
            }
            catch (Exception e)
            {
                Logger.Error(e, "AbstractDataServicesProvider.ExposeConfig");
            }
        }

        [Serializable]
        class TypeSource : ITypeSource
        {
            private readonly IEnumerable<RecordBlueprint> _recordDescriptors;

            public TypeSource(IEnumerable<RecordBlueprint> recordDescriptors) { _recordDescriptors = recordDescriptors; }

            public IEnumerable<Type> GetTypes() { return _recordDescriptors.Select(descriptor => descriptor.Type); }

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