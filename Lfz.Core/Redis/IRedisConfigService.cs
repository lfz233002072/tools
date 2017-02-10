using System.Collections.Generic;

namespace Lfz.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRedisConfigService : ISingletonDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        RedisConfig Get(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<RedisConfig> GetAll();

    }
}