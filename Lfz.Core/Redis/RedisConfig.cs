using System;

namespace Lfz.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisConfig
    {
        /// <summary>
        /// ≈‰÷√ID
        /// </summary>
        public int ConfigId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReadWriteHosts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReadOnlyHosts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ExpiredTime { get; set; }
    }
}