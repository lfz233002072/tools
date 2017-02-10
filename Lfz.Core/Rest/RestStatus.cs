namespace Lfz.Rest
{
    /// <summary>
    /// 
    /// </summary>
    public enum RestStatus
    {
        /// <summary>
        /// 无定义
        /// </summary>
        [CustomDescription("无定义")]
        None = 0,

        /// <summary>
        /// 操作成功
        /// </summary>
        [CustomDescription("操作成功")]
        Success = 1,


        /// <summary>
        /// 无权限
        /// </summary>
        [CustomDescription("无权限")]
        NoPower = 2,

        /// <summary>
        /// 暂无实现
        /// </summary>
        [CustomDescription("暂无实现")]
        NoImp = 4,

        /// <summary>
        /// 其它错误
        /// </summary>
        [CustomDescription("其它错误")]
        Error = 8,

        /// <summary>
        /// 无效验证信息
        /// </summary>
        [CustomDescription("无效验证信息")]
        InvalidCredential = 101,

        /// <summary>
        /// 空请求,没有POST主体内容
        /// </summary>
        [CustomDescription("空请求")]
        InvalidChars = 102,
    }
}