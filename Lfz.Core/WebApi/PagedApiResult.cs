using System.Collections.Generic;

namespace Lfz.WebApi
{
    /// <summary>
    /// 分页数据结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedApiResult<T> : ApiResult<IEnumerable<T>>
    {

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总记录数量
        /// </summary>
        public int TotalItemCount { get; set; }
    }
}