namespace Lfz.Data.Nh.Entities
{
    /// <summary>
    /// 提供搜索优化相关的META信息
    /// </summary>
    public abstract class MetaEntityBase : EntityBase<int>
    {
        /// <summary>
        /// Meta标题
        /// </summary>
        public virtual string MetaTitle { get; set; }

        /// <summary>
        /// Meta描述
        /// </summary>
        public virtual string MetaDescription { get; set; }

        /// <summary>
        /// Meta键值
        /// </summary>
        public virtual string MetaKeywords { get; set; }
    }
}