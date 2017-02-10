namespace Lfz.Logging
{
    /// <summary>
    /// 日志等级 Fatal > Error > Warning> Information > Debug > Trace
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 跟踪信息
        /// </summary>
        [CustomDescription("跟踪")]
        Trace,

        /// <summary>
        /// 调试
        /// </summary>
        [CustomDescription("调试")]
        Debug,
        /// <summary>
        /// 信息
        /// </summary>
        [CustomDescription("提示")]
        Information,
        /// <summary>
        /// 警报
        /// </summary>
        [CustomDescription("警报")]
        Warning,
        /// <summary>
        /// 错误
        /// </summary>
        [CustomDescription("错误")]
        Error,
        /// <summary>
        /// 致命错误
        /// </summary>
        [CustomDescription("致命")]
        Fatal,
    }
}