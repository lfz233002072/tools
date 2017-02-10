namespace Lfz.Data.RawSql
{
    public enum TemplateDataSourceType
    { 
        [CustomDescription("Sql查询")]
        SqlClause,

        [CustomDescription("枚举类型")]
        EnumType,

        /// <summary>
        /// 派送自定接口的类型解析
        /// </summary>
        [CustomDescription("Api接口")]
        Api,

        /// <summary>
        /// 派送自定接口的类型解析
        /// </summary>
        [CustomDescription("门店查询")]
        StoreInfo,

        /// <summary>
        /// 派送自定接口的类型解析
        /// </summary>
        [CustomDescription("支付方式")]
        PayMode,

        /// <summary>
        /// 使用,分割多种类型
        /// </summary>
        [CustomDescription("字符串列表")]
        StringList,

        /// <summary>
        ///  
        /// </summary>
        [CustomDescription("字符串")]
        String ,
    }
}