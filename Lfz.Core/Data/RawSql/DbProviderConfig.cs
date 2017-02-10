using System;

namespace Lfz.Data.RawSql
{
    /// <summary>
    /// 
    /// </summary>
    public class DbProviderConfig : IDbProviderConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual DbProvider DbProvider { get; set; }

        public Guid CustomerId { get; set; }

        public string DataFolder { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public virtual string DbConnectionString { get; set; }
         
    }
}