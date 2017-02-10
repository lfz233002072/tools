namespace Lfz.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public enum ServerApiStatus
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 访问token无效
        /// </summary> 
        InvalidAccessToken = 1,
        /// <summary>
        /// Api无效
        /// </summary> 
        InvalidApiAndSecret = 2,

        /// <summary>
        /// 客户ID无效
        /// </summary> 
        InvalidClientId = 3,
        /// <summary>
        /// 无状态
        /// </summary>
        None = -1,

        /// <summary>
        /// 无状态
        /// </summary>
        Error = -99
    }
}