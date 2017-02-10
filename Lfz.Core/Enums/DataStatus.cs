namespace Lfz.Enums
{
    /// <summary>
    /// 数据状态
    /// </summary>
    public enum DataStatus
    {
        /// <summary>
        /// 过期、无效的、已经关闭
        /// </summary>
        [CustomDescription("关闭")]
        InValid = 0,

        /// <summary>
        /// 有效、正在使用、已经开启
        /// </summary>
        [CustomDescription("正常")]
        Valid = 1,

        /// <summary>
        /// 其它
        /// </summary>
        [CustomDescription("其它")]
        Other = 99
    }
}