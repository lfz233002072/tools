/*======================================================================
 *
 *        Copyright (C)  1996-2012  lfz
 *        All rights reserved
 *
 *        Filename :EntityBase.cs
 *        DESCRIPTION :Base class for entities
 *
 *        Created By 林芳崽 at  2013-05-08 18:47
 *        https://git.oschina.net/lfz/tools
 *
 *======================================================================*/

using System;
using Lfz.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lfz.Data
{

    /// <summary>
    /// 数据实体基类
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam> 
    [Serializable]
    public abstract partial class EntityBase<TKey> : CloneEntityBase
    {
        /// <summary>
        /// 获取主键值
        /// </summary>
        /// <returns></returns>
        public abstract TKey GetKeyValue();
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class CloneEntityBase : RestContentBase
    {
        /// <summary>
        /// 创建一个新对象，然后将当前对象的非静态字段复制到该新对象。 
        /// 如果字段是值类型的，则对该字段执行逐位复制。 
        /// 如果字段是引用类型，则复制引用但不复制引用的对象；因此，原始对象及其复本引用同一对象。
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract partial class EntityBase : EntityBase<Guid>
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }

        public override Guid GetKeyValue()
        {
            return Id;
        }
    }

    [Serializable]
    public sealed class KeyValuePair : EntityBase
    {
        public string Value { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class EntityBaseExtenstions
    {
        /// <summary>
        /// 创建一个新对象，然后将当前对象的非静态字段复制到该新对象。 
        /// 如果字段是值类型的，则对该字段执行逐位复制。 
        /// 如果字段是引用类型，则复制引用但不复制引用的对象；因此，原始对象及其复本引用同一对象。
        /// </summary>
        /// <returns></returns>
        public static T DeepClone<T>(this CloneEntityBase entity) where T : CloneEntityBase
        {
            return entity.Clone() as T;
        }

        /// <summary>
        /// 派生自EntityBase的对象转换为JSOn格式字符串
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>JSON格式字符串</returns>
        public static string ToJson(this EntityBase entity)
        {
            var dataConvert = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            };
            return JsonConvert.SerializeObject(entity, Formatting.None, dataConvert);
        }

        /// <summary>
        /// 从JSON格式字符串中反序列化实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonContent">JSON格式字符串</param>
        /// <returns></returns>
        public static T FromJson<T>(this string jsonContent) where T : EntityBase
        {
            if (!string.IsNullOrEmpty(jsonContent))
                return JsonConvert.DeserializeObject<T>(jsonContent);
            return null;
        }

        /// <summary>
        /// 创建一个新对象，然后将当前对象的非静态字段复制到该新对象。 
        /// 如果字段是值类型的，则对该字段执行逐位复制。 
        /// 如果字段是引用类型，则复制引用但不复制引用的对象；因此，原始对象及其复本引用同一对象。
        /// </summary>
        /// <returns></returns>
        public static T Clone<T>(this EntityBase entity) where T : EntityBase
        {
            return entity.Clone() as T;
        }
    }


     
}
