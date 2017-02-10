// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :JsonConfigBase.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-29 11:06
// *        https://git.oschina.net/lfz/tools
// *
// *======================================================================*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lfz.Caching;
using Lfz.Utitlies;

namespace Lfz.Config
{
    public abstract class JsonConfigBase
    {
        /// <summary>
        /// 返回配置项当前值或默认值（默认值不能为空）
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="getConfigfileFunc"></param>
        /// <param name="newFunc"></param>
        /// <returns></returns>
        internal static object Load(Type elementType, Func<string> getConfigfileFunc, Func<object> newFunc)
        {
            return JsonConfigHelper.GetCurrent(elementType, getConfigfileFunc, newFunc);
        }

        /// <summary>
        /// 返回配置项当前值或默认值（默认值不能为空）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>返回配置项当前值或默认值（默认值不能为空）</returns>
        protected static T Load<T>() where T : JsonConfigBase, new()
        {
            return JsonConfigHelper.GetCurrent<T>(() =>
            {
                var temp = new T();
                return temp.GetConfigFile();
            });
        }

        /// <summary>
        /// 保存并刷新配置
        /// </summary>
        public void Save()
        {
            JsonConfigHelper.Save(this, this.GetConfigFile);
        } 

        /// <summary>
        /// 保存并刷新配置
        /// </summary>
        public void Flush()
        {
            JsonConfigHelper.Save(this, this.GetConfigFile);
        }

        /// <summary>
        /// 获取配置文件的相对路径（相对应用程序域的跟目录）
        /// </summary>
        /// <returns></returns>
        public abstract string GetConfigFile();

    }

    /// <summary>
    /// Json配置文件
    /// </summary>
    public class JsonConfigHelper
    {
        private static ConcurrentDictionary<string, string> ConfigFiles =
            new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<Type, object> _allSingletons;

        private static readonly ICacheSerializater serializater;
        private static readonly string _configFilePath;

        static JsonConfigHelper()
        {
            serializater = new JsonFileCacheSerializater();
            _allSingletons = new ConcurrentDictionary<Type, object>();
            _configFilePath = Utils.MapPath("~/Config/jsonfilelist.json");
            Init();
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void Init()
        {
            ConfigFiles.Clear();
            var dic = serializater.Load<Dictionary<string, string>>(_configFilePath);
            if (dic == null) return;
            foreach (var pair in dic)
            {
                if (string.IsNullOrEmpty(pair.Value) || string.IsNullOrEmpty(pair.Key)) continue;
                ConfigFiles.TryAdd(pair.Key, Utils.MapPath(pair.Value));
            }
        }

        private static void AddItem(string key, string value)
        {
            var dic = serializater.Load<Dictionary<string, string>>(_configFilePath) ??
                      new Dictionary<string, string>();
            if (dic.ContainsKey(key))
                dic[key] = value;
            else
                dic.Add(key, value);
            Init();
        }

        private static string GetConfigFile(Type elementType, Func<string> configFileFunc)
        {
            var key = elementType.FullName;
            string item;
            ConfigFiles.TryGetValue(key, out item);
            if (!string.IsNullOrEmpty(item))
                return item;
            if (configFileFunc == null) return string.Empty;
            var file = configFileFunc();
            if (string.IsNullOrEmpty(file)) return string.Empty;
            AddItem(key, file);
            return Utils.MapPath(file);
        }

        /// <summary>
        /// 返回配置项当前值或默认值（默认值不能为空）
        /// </summary>
        public static object GetCurrent(Type elementType, Func<string> configFileFunc, Func<object> newFunc)
        {
            object result;
            _allSingletons.TryGetValue(elementType, out result);
            if (result != null) return result;
            if (configFileFunc != null)
            {
                var file = GetConfigFile(elementType, configFileFunc);
                if (!string.IsNullOrEmpty(file))
                {
                    result = serializater.Load(elementType, file);
                }
            }
            if (result == null)
            {
                result = newFunc();
            }
            else
            {
                throw new Exception("默认值构造函数无效");
            }
            if (result == null)
            {
                throw new Exception("默认值构造无效,默认值不能为空");
            }
            _allSingletons.AddOrUpdate(elementType, result, (x, y) => result);
            return result;
        }

        /// <summary>
        /// 返回配置项当前值或默认值（默认值不能为空）
        /// </summary>
        public static T GetCurrent<T>(Func<string> configFileFunc) where T : JsonConfigBase, new()
        {
            object temp;
            var elementType = typeof(T);
            _allSingletons.TryGetValue(elementType, out temp);
            var result = temp as T;
            if (result != null) return result;
            var file = GetConfigFile(elementType, () => { var temp2 = new T(); return temp2.GetConfigFile(); });
            if (!string.IsNullOrEmpty(file))
            {
                result = serializater.Load<T>(file);
            }
            if (result == null) result = new T();
            _allSingletons.AddOrUpdate(elementType, result, (x, y) => result);
            return result;
        }


        public static void Save(object obj, Func<string> configFileFunc)
        {
            if (obj != null)
            {
                var type = obj.GetType();
                 serializater.Save(obj, GetConfigFile(type, configFileFunc));
                _allSingletons.AddOrUpdate(type, obj, (x, y) => obj);
            }
        }

    }
}