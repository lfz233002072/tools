using FluentNHibernate.Cfg.Db;
using Lfz.Data.RawSql;

namespace Lfz.Data.Nh.Providers {
    public interface IDataServicesProvider  {

        NHibernate.Cfg.Configuration BuildConfiguration(IDbProviderConfig config);

        IPersistenceConfigurer GetPersistenceConfigurer(bool createDatabase);
    }
}
