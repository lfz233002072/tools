using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;

namespace PMSoft.Data.Providers {
    /// <summary>
    /// 
    /// </summary>
    public interface IDataServicesProvider  {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionFactoryParameters"></param>
        /// <returns></returns>
        Configuration BuildConfiguration(SessionFactoryParameters sessionFactoryParameters);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="createDatabase"></param>
        /// <returns></returns>
        IPersistenceConfigurer GetPersistenceConfigurer(bool createDatabase);
    }
}
