namespace Lfz.Models
{
    /// <summary>
    /// 属性跟踪状态
    /// </summary>
    public enum PropertyTrackStatus
    { 
        /// <summary>
        /// 未修改
        /// </summary>
        [CustomDescription("未修改")]
        Unchanged = 2,
      
        /// <summary>
        /// 最新添加
        /// </summary>
        [CustomDescription("新添加")]
        Added = 4,

        /// <summary>
        /// 已删除
        /// </summary>
        [CustomDescription("已删除")]
        Deleted = 8,

        /// <summary>
        /// 已修改
        /// </summary>
        [CustomDescription("已修改")]
        Modified = 16,
    }
}