//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : ReadOnlyCollectionExtensions
//        DESCRIPTION : 枚举列表只读访问转换
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Lfz.Data;
using Lfz.Utitlies;
using Newtonsoft.Json;

namespace Lfz.Collections
{
    /// <summary>
    /// 集合类型扩展方法实现
    /// </summary>
    public static class CollectionExtensions
    {

        /// <summary>
        /// 根据键值获取字典中Guid类型值。如果不存在或无效返回Guid.Empty
        /// </summary> 
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Guid GetGuid(this IDictionary<string, string> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key)) return Guid.Empty;
            return TypeParse.StrToGuid(dictionary[key]);
        }
        public static string GetValueByParams(this IDictionary<string, string> dictionary, params string[] keyList)
        {
            foreach (var s in keyList)
            {
                if (dictionary.ContainsKey(s)) return dictionary[s];
            }
            return string.Empty;
        }
        /// <summary>
        /// 根据键值获取字典中int类型值。如果不存在或无效返回0
        /// </summary> 
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetInt(this IDictionary<string, string> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key)) return 0;
            return TypeParse.StrToInt(dictionary[key]);
        }
        /// <summary>
        /// 根据键值获取字典中bool类型值。如果不存在或无效返回false
        /// </summary> 
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetBool(this IDictionary<string, string> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key)) return false;
            return TypeParse.StrToBool(dictionary[key]);
        }

        /// <summary>
        /// 根据键值获取字典中string类型值。如果不存在或无效返回string.Empty
        /// </summary> 
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(this IDictionary<string, string> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key)) return string.Empty;
            return dictionary[key];
        }
        /// <summary>
        /// 枚举列表只读访问转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns>返回一个只读访问列表</returns>
        public static IList<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ReadOnlyCollection<T>(enumerable.ToList());
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue<T>(this IEnumerable<T> arguments, string key) where T : KeyValueEntity
        {
            var item = arguments.FirstOrDefault(x => String.Equals(key, x.Key, StringComparison.OrdinalIgnoreCase));
            return item == null ? String.Empty : item.Value;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Guid GetGuidValue<T>(this IEnumerable<T> arguments, string key) where T : KeyValueEntity
        {
            var item = arguments.FirstOrDefault(x => String.Equals(key, x.Key, StringComparison.OrdinalIgnoreCase));
            if (item == null || String.IsNullOrEmpty(item.Value)) return Guid.Empty;
            Guid value;
            return Guid.TryParse(item.Value, out value) ? value : Guid.Empty;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetIntValue<T>(this IEnumerable<T> arguments, string key) where T : KeyValueEntity
        {
            var item = arguments.FirstOrDefault(x => String.Equals(key, x.Key, StringComparison.OrdinalIgnoreCase));
            return item == null ? 0 : TypeParse.StrToInt(item.Value);
        }
 
        /// <summary>
        /// 获取列表中的index个实例数据
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="index">列表数据索引,索引从0开始</param>
        /// <returns></returns>
        public static T GetByIndex<T>(this IEnumerable<T> entities, int index) where T : EntityBase
        {
            int count = entities.Count();
            if (count > index + 1) return null;
            if (index == 0) return entities.FirstOrDefault();
            return entities.Skip(index).FirstOrDefault();
        }

        /// <summary>
        /// 获取指定类型的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetByKey<T>(this IDictionary<string, object> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key)) return default(T);
            var obj = dictionary[key];
            if (obj == null || !(obj is T)) return default(T);
            return (T)obj;
        }

        /// <summary>
        /// 获取指定类型的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetByKey<T>(this IDictionary<string, object> dictionary, object key)
        {
            return dictionary.GetByKey<T>(key.ToString());
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>如果已经存在键值那么更新，否则添加</returns>
        public static void TryAddOrUpdate(this IDictionary<string, object> dictionary, string key, object value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
            {
                dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// 
        /// </summary> 
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void TryAddOrUpdate(this IDictionary<string, object> dictionary, object key, object value)
        {
            dictionary.TryAddOrUpdate(key.ToString(), value);
        }

        /// <summary>
        /// 获取指定类型的值
        /// </summary> 
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Guid GetGuidByKey(this IDictionary<int, Guid> dictionary, int key)
        {
            if (!dictionary.ContainsKey(key)) return Guid.Empty;
            return dictionary[key];
        }

        /// <summary>
        /// 获取指定类型的值
        /// </summary> 
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Guid GetGuidByKey(this IDictionary<Guid, Guid> dictionary, Guid key)
        {
            if (!dictionary.ContainsKey(key)) return Guid.Empty;
            return dictionary[key];
        }

        /// <summary>
        /// 获取指定类型的值
        /// </summary> 
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Guid GetGuidByKey(this IDictionary<string, object> dictionary, object key)
        {
            var strKey = key.ToString();
            if (dictionary.ContainsKey(strKey))
            {
                return TypeParse.StrToGuid(dictionary[strKey]);
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 
        /// </summary> 
        /// <param name="dictionary"></param>
        /// <param name="key"></param> 
        /// <returns></returns>
        public static string GetStrBykey(this IDictionary<string, object> dictionary, object key)
        {
            object value;
            if (!dictionary.TryGetValue(key.ToString(), out value)) return String.Empty;
            return (value ?? String.Empty).ToString();
        }
         
        /// <summary>
        /// 先将字典转换为 KeyValuePair &lt;string, string  &gt; 列表，然后将该列表转换为JSON字符串。最后将JSON字符串转换为Base64字符串
        /// </summary> 
        /// <param name="dictionary"></param> 
        /// <returns>Base64字符串</returns>
        public static string ToJsonBase64Str(this IDictionary<string, object> dictionary)
        {
            var result = string.Empty;
            try
            {
                var list = dictionary.Select(x =>
                                             new KeyValuePair<string, string>(x.Key, x.Value == null ? string.Empty : x.Value.ToString()));
                if (!list.Any()) return string.Empty;
                var str = JsonConvert.SerializeObject(list);
                result = Utils.ToBase64String(str);
            }
            catch
            { 
            }
            return result;
        }
         
    }
}
