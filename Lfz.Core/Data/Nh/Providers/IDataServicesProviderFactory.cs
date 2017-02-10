using Lfz.Data.RawSql;

namespace Lfz.Data.Nh.Providers {
    public interface IDataServicesProviderFactory : IDependency {
        IDataServicesProvider CreateProvider(IDbProviderConfig sessionFactoryParameters);
    }

}
