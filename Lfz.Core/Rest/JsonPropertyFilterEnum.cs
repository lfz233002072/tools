namespace Lfz.Rest
{
    /// <summary>
    /// JSON属性过滤器列表（提供给<see cref="RestResolverFilterAttribute"/>使用）。
    /// 该枚举值作为过滤器的Json属性解析过滤器唯一键值存在。
    /// </summary>
    public enum JsonPropertyFilterEnum
    {
        /// <summary>
        /// 不需要过滤处理
        /// </summary>
        None = 0,

        /// <summary>
        /// RestResponse类型的Result Json属性需要被替换
        /// </summary>
        RestResponseResult = 100,

        /// <summary>
        ///  SettingModel类型的Data数据Json属性需要使用过滤器
        /// </summary>
        SettingModelData = 200
    }
}