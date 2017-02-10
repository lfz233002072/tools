using Lfz.Data.RawSql;
using Lfz.Logging;

namespace Lfz.Data.Nh
{
    public partial class NhRepository<T> : INhRepository<T> where T : class
    {
        public NhRepository(IDbProviderConfig config)
        {
            _config = config;
            Logger = LoggerFactory.GetLog();
            _sessionFactory = null; //.Current.Resolve<ISessionFactoryHolder>();
        }
    }
}