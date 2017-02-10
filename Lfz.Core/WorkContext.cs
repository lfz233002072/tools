/*======================================================================
 *
 *        Copyright (C)  1996-2012  杭州品茗信息有限公司    
 *        All rights reserved
 *
 *        Filename :WorkContext.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-05-08 17:30
 *        http://www.pinming.cn/
 *
 *======================================================================*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Features.Metadata;
using PMSoft.Caching;
using PMSoft.Config;
using PMSoft.Data;
using PMSoft.Data.Providers;
using PMSoft.Services;

namespace PMSoft
{
    internal class WorkContext
    {
        private static readonly ConcurrentDictionary<Type, RecordBlueprint> CachesRecordTable = new ConcurrentDictionary<Type, RecordBlueprint>();
        private static WorkContext _instance = new WorkContext();

        // ReSharper disable StaticFieldInGenericType
        private static readonly object LockHelper = new object();
        // ReSharper restore StaticFieldInGenericType


        /// <summary>
        /// 获取实例
        /// </summary>
        public static WorkContext CurrentContext
        {
            get
            {
                if (Equals(_instance, default(WorkContext)))
                {
                    lock (LockHelper)
                    {
                        if (Equals(_instance, default(WorkContext)))
                        {
                            _instance = new WorkContext();
                        }
                    }
                }
                return _instance;
            }
        }

        #region 构造、初始化注入容器

        private IContainer _container;

        /// <summary>
        /// 初始化注入容器
        /// </summary>
        /// <param name="registrations"></param>
        /// <returns></returns>
        public IContainer CreateHostContainer(Action<ContainerBuilder> registrations)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new CacheModule());
            builder.RegisterModule(new DataModule());
            builder.RegisterType<DefaultCacheHolder>().As<ICacheHolder>().SingleInstance();
            builder.RegisterType<DefaultCacheContextAccessor>().As<ICacheContextAccessor>().SingleInstance();   
            IDataServicesProviderFactory dataServicesProviderFactory = new DataServicesProviderFactory(new[] {
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
            builder.RegisterInstance(dataServicesProviderFactory).As<IDataServicesProviderFactory>();
            /*
                        builder.Register(x => new EfDataProviderManager(x.Resolve<DataSettings>())).As<BaseDataProviderManager>().InstancePerDependency();
                        builder.Register(x => (IEfDataProvider)x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();
                        builder.Register(x => (IEfDataProvider)x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IEfDataProvider>().InstancePerDependency();
                        //初始化工作
                        var efDataProviderManager = new EfDataProviderManager(DataSettingsHelper.GetBaseConfig());
                        var dataProvider = (IEfDataProvider)efDataProviderManager.LoadDataProvider();
                        dataProvider.InitConnectionFactory();
                        builder.Register<IDbContext>(c => new PmSoftObjectContext(c.Resolve<DataSettings>().DataConnectionString)).InstancePerDependency();*/
            builder.RegisterType<InstallService>().As<IInstallService>().InstancePerDependency();
            if (registrations != null) registrations(builder);
            //注册配置信息
            builder.Register(c => new ShellSettings()
                {
                    Settings = DataSettingsHelper.GetBaseConfig(),
                    RecordBlueprints = CachesRecordTable.Select(x => x.Value)
                }).As<ShellSettings>();
            return builder.Build();
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assembly"></param>
        private void BuilderDenpency2(ContainerBuilder builder, Assembly assembly)
        {
            IEnumerable<Type> list =
                assembly.GetExportedTypes()
                        .Where(t => t.IsClass && !t.IsAbstract && !t.IsSealed && typeof(IDependency).IsAssignableFrom(t));
            var exceptlist = GetRegisterType();
            list = list.Except(exceptlist);
            if (list == null) return;
            foreach (var item in list)
            {
                var ilist = item.GetInterfaces().Where(itf => typeof(IDependency).IsAssignableFrom(itf));

                var registration = builder.RegisterType(item).InstancePerLifetimeScope();
                foreach (var interfaceType in ilist)
                {
                    registration = registration.As(interfaceType);
                    if (typeof(ISingletonDependency).IsAssignableFrom(interfaceType))
                    {
                        registration = registration.SingleInstance();
                    }
                    else if (typeof(IUnitOfWorkDependency).IsAssignableFrom(interfaceType))
                    {
                        registration = registration.InstancePerLifetimeScope();
                    }
                    else if (typeof(ITransientDependency).IsAssignableFrom(interfaceType))
                    {
                        registration = registration.InstancePerDependency();
                    }
                }
            }
        }

        private static void AddOrUpdateMapping(Assembly assembly)
        {
            IEnumerable<Type> list =
                assembly.GetExportedTypes()
                        .Where(t => t.IsClass && !t.IsAbstract && !t.IsSealed && typeof(EntityBase).IsAssignableFrom(t));
           
            foreach (var type in list)
            {
                //排除KeyValueEntity和KeyValueEntity<Guid>两个类
                if (type.Name.Contains("KeyValueEntity")) continue;
                CachesRecordTable.AddOrUpdate(type,
                    x => new RecordBlueprint() { TableName = x.Name, Type = type },
                    (x, r) =>
                    {
                        r.TableName =x.Name ;
                        r.Type = type;
                        return r;
                    });
            }
        }
         
        private static IEnumerable<Type> GetRegisterType()
        {
            return new[] {  
                    typeof(DefaultCacheContextAccessor),  typeof(DefaultCacheManager),   typeof(DefaultCacheHolder), 
                };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblys"></param>
        public void BuilderDenpency(ContainerBuilder builder, params Assembly[] assemblys)
        {
            CachesRecordTable.Clear();
            //注入实体映射信息  
            foreach (var item in assemblys)
            {
                AddOrUpdateMapping(item);
                BuilderDenpency2(builder, item);
            }
        }

        #endregion

        #region 设置注入容器

        public void SetResolver(IContainer container)
        {
            _container = container;
        }

        #endregion

        #region 从注入上下文获取数据

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public T Resolve<T>(IEnumerable<Parameter> parameters)
        {
            return _container.Resolve<T>(parameters);
        }

        public T Resolve<T>(params Parameter[] parameters)
        {
            return _container.Resolve<T>(parameters);
        }

        public object Resolve(Type t)
        {
            return _container.Resolve(t);
        }

        public object Resolve(Type t, IEnumerable<Parameter> parameters)
        {
            return _container.Resolve(t, parameters);
        }

        public object Resolve(Type t, params Parameter[] parameters)
        {
            return _container.Resolve(t, parameters);
        }

        #endregion
    }
}