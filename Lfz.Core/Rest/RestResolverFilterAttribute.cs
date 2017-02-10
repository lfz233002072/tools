/*======================================================================
 *
 *        Copyright (C)  1996-2012  lfz    
 *        All rights reserved
 *
 *        Filename :RestResolverFilterAttribute.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-07-22 13:41
 *        https://git.oschina.net/lfz/tools
 *
 *======================================================================*/

using System;

namespace Lfz.Rest
{
    /// <summary>
    /// Json属性解析过滤器。主要用于某些抽象类反序列化化查找具体实现类。
    /// 反序列化时，需要调用类对象的默认构造函数，所以需要一个包含默认函数的具体实现类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RestResolverFilterAttribute : Attribute
    {
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueKey">过滤器唯一键值</param>
        public RestResolverFilterAttribute(JsonPropertyFilterEnum uniqueKey)
        {
            UniqueKey = uniqueKey;
        }
         

        /// <summary>
        /// 过滤器唯一键值
        /// </summary>
        public JsonPropertyFilterEnum UniqueKey { get; set; }
    }

}