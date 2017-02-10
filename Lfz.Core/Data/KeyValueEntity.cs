using System.Runtime.Serialization;

namespace Lfz.Data
{
    /// <summary>
    /// 键值数据实体
    /// </summary> 
    [DataContract]
    public abstract class KeyValueEntity : EntityBase
    {
        /// <summary>
        /// 参数键
        /// </summary>
        [DataMember]
        public virtual string Key { get; set; }

        /// <summary>
        /// 参数键值
        /// </summary>
        [DataMember]
        public virtual string Value { get; set; }
    }
}