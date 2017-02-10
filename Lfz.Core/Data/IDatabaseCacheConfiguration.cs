using NHibernate.Cfg.Loquacious;

namespace PMSoft.Data {
    /// <summary>
    /// 
    /// </summary>
    public interface IDatabaseCacheConfiguration  {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        void Configure(ICacheConfigurationProperties cache);
    }
}