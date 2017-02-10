namespace PMSoft.Data.Providers {
    /// <summary>
    /// 数据服务参数
    /// </summary>
    public class DataServiceParameters {
        /// <summary>
        /// 数据库提供者（Sqlserver、MYsql、Sqlce等）
        /// </summary>
        public string Provider { get; set; }
        /// <summary>
        /// 数据文件夹
        /// </summary>
        public string DataFolder { get; set; }
        /// <summary>
        /// 数据表前缀
        /// </summary>
        public string TablePrefix { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }
    }
}