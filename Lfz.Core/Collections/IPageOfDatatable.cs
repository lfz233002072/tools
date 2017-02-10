using System.Data;

namespace Lfz.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPageOfDatatable : IPagerInfo
    {
        /// <summary>
        /// 
        /// </summary>
        DataTable Data { get; set; }
    }
}