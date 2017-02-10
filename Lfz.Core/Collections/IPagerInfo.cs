namespace Lfz.Collections
{
    /// <summary>
    /// 分页基本信息
    /// </summary>
    public interface IPagerInfo
    { 
        /// <summary>
        /// 页码 页码序号从0开始 
        /// </summary>
        int PageIndex { get; set; }

        /// <summary>
        /// 页面大小
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// 总记录数量[大于0的数字]
        /// </summary>
        int TotalItemCount { get; set; }
        /// <summary>
        /// 总页数[最少有一页，哪怕是没有记录，也会显示一页]
        /// </summary>
        int TotalPageCount { get; }
        /// <summary>
        /// 从0开始计数
        /// </summary>
        int StartIndex { get; }
        /// <summary>
        /// 最后条数 取值范围从0到TotalItemCount-1
        /// </summary>
        int EndIndex { get; }
    }
}