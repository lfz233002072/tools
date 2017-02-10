using System;

namespace Lfz.Models
{
    /// <summary>
    /// 包含键值的查询条件
    /// </summary>
    public class KeySearchHit : SearchHit
    {
        /// <summary>
        /// 键值
        /// </summary>
        public Guid Key { get; set; }
    }
}