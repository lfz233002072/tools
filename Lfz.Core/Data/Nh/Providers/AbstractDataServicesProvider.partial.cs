using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lfz.Logging;
using Lfz.Utitlies;

namespace Lfz.Data.Nh.Providers
{ 
    public abstract partial class AbstractDataServicesProvider : IDataServicesProvider
    {
        /// <summary>Initializes a static instance of the Nop factory.</summary> 
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialize(IEnumerable<INhTypeProvider> redisConfigService)
        {
            Singleton<IEnumerable<INhTypeProvider>>.Instance = redisConfigService;
        }


        protected AbstractDataServicesProvider( )
        {
            _typeProviders = Singleton<IEnumerable<INhTypeProvider>>.Instance;
            if (_typeProviders == null)
                throw new Exception("FluentNHibernate 需要注册的类型提供器未初始化");
            Logger = LoggerFactory.GetLog();
           // _typeProviders = EngineContext.Current.Resolve<IEnumerable<INhTypeProvider>>();
        }
    }
}