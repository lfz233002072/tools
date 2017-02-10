//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : MemoryCacheManager
//        DESCRIPTION :  
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using Lfz.Collections;
using Lfz.Utitlies;

namespace Lfz.Caching
{
    /// <summary>
    /// 表示一个内存缓存 
    /// </summary>
    public partial class NetMemoryCacheManager
    {
        #region 属性、字段

        /// <summary>
        /// 缓存区域名称
        /// </summary>
        private readonly int _regionName;

        /// <summary>
        ///  
        /// </summary> 

        private readonly int _defaultCacheTime;



        /// <summary>
        /// 
        /// </summary>
        public int DefaultCacheTime
        {
            get { return _defaultCacheTime; }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 缓存区域名称前缀
        /// </summary>  
        /// <param name="regionName"></param>
        /// <param name="defaultCacheTime">默认缓存时间120分钟(2个小时)，单位分钟</param>
        public NetMemoryCacheManager(int regionName, int defaultCacheTime = 120)
        {
            _regionName = regionName;
            _defaultCacheTime = defaultCacheTime;
        }

        #endregion

        #region 辅助类

        /// <summary>
        /// 当前缓存对象获取
        /// </summary>
        public ObjectCache Cache
        {
            get { return MemoryCache.Default; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param> 
        /// <returns></returns>
        public string GetCacheKey<TKey>(TKey key)
        {
            return string.Format("{0}_{1}", (int)_regionName, key);
        }


        #endregion

        /// <summary>
        /// 根据键值获取或更新缓存键值
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="regionName">缓存区域</param>
        /// <param name="func"></param>
        /// <param name="changeMonitorList"></param> 
        /// <returns>返回当前键值关联的缓存对象</returns>
        public virtual TResult Get<TKey, TResult>(TKey key, Func<TKey, TResult> func, IEnumerable<ChangeMonitor> changeMonitorList = null)
        {
            var cacheKey = GetCacheKey(key);
            if (Cache.Contains(cacheKey))
            {
                var obj = Cache.Get(cacheKey);
                //如果缓存的值为空，那么移除缓存键值
                if (!TypeParse.IsDefaultValue(obj) && obj is TResult)
                {
                    return (TResult)Convert.ChangeType(obj, typeof(TResult));
                }
                //移除无效值
                Cache.Remove(cacheKey);
            }
            //计算值
            var result = func(key);
            //null ,0,Guid.Empty,string.Empty等值不做缓存
            if (object.Equals(result, null) || object.Equals(result, 0) || object.Equals(result, Guid.Empty) || object.Equals(result, string.Empty))
            {
                return result;
            }
            //设置缓存
            Set(key, result, _defaultCacheTime, changeMonitorList);
            return result;
        }

        /// <summary>
        /// 根据键值获取或更新缓存键值
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="regionName">缓存区域</param>
        /// <param name="func"></param>
        /// <param name="changeMonitorList"></param> 
        /// <returns>返回当前键值关联的缓存对象</returns>
        public virtual IEnumerable<TResult> GetList<TKey, TResult>(TKey key, Func<TKey,
            IEnumerable<TResult>> func, IEnumerable<ChangeMonitor> changeMonitorList = null)
        {
            var cacheKey = GetCacheKey(key);
            if (Cache.Contains(cacheKey))
            {
                var obj = Cache.Get(cacheKey);
                //如果缓存的值为空，那么移除缓存键值 
                var tempList = obj as IList<TResult>;
                if (tempList != null && tempList.Any()) return tempList;
                //移除无效值
                Cache.Remove(cacheKey);
            }
            //计算值
            var result = func(key);
            if (result == null)
            {
                return null;
            }
            var cacheList = result.ToReadOnlyCollection();
            //设置缓存
            SetList(key, cacheList, _defaultCacheTime, changeMonitorList);
            return cacheList;
        }

        /// <summary>
        /// 根据键值获取或更新缓存键值
        /// </summary>
        /// <param name="key">缓存键值</param> 
        /// <param name="func"></param> 
        /// <returns>返回当前键值关联的缓存对象</returns>
        public virtual IDictionary<TKey1, TResult> GetDictionary<TKey, TKey1, TResult>(TKey key)
        {
            var cacheKey = GetCacheKey(key);
            if (Cache.Contains(cacheKey))
            {
                var obj = Cache.Get(cacheKey);
                //如果缓存的值为空，那么移除缓存键值 
                var tempList = obj as IDictionary<TKey1, TResult>;
                if (tempList != null && tempList.Any()) return tempList;
                //移除无效值
                Cache.Remove(cacheKey);
            }
            return null;
        }

        /// <summary>
        /// 根据键值获取或更新缓存键值
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="data"></param>
        /// <returns>返回当前键值关联的缓存对象</returns>
        public virtual void SetDictionary<TKey, TKey1, TValue>(TKey key, IDictionary<TKey1, TValue> data)
        {
            if (data == null) return;
            IDictionary<TKey1, TValue> content = new Dictionary<TKey1, TValue>(data);
            Set<TKey, IDictionary<TKey1, TValue>>(key, content, _defaultCacheTime);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key"></param>
        /// <param name="changeMonitorList"></param>
        /// <returns></returns>
        public virtual IEnumerable<TResult> GetList<TKey, TResult>(TKey key, IEnumerable<ChangeMonitor> changeMonitorList = null)
        {
            var cacheKey = GetCacheKey(key);
            if (Cache.Contains(cacheKey))
            {
                var obj = Cache.Get(cacheKey);
                //如果缓存的值为空，那么移除缓存键值 
                var tempList = obj as IList<TResult>;
                if (tempList != null && tempList.Any()) return tempList;
                //移除无效值
                Cache.Remove(cacheKey);
            }
            return Enumerable.Empty<TResult>();
        }
        /// <summary>
        /// 根据键值获取或更新缓存键值
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="regionName">缓存区域</param> 
        /// <returns>返回当前键值关联的缓存对象</returns>
        public virtual TResult Get<TKey, TResult>(TKey key)
        {
            var cacheKey = GetCacheKey(key);
            if (Cache.Contains(cacheKey))
            {
                var obj = Cache.Get(cacheKey);
                //如果缓存的值为空，那么移除缓存键值
                if (!TypeParse.IsDefaultValue(obj) && obj is TResult)
                {
                    return (TResult)Convert.ChangeType(obj, typeof(TResult));
                }
                //移除无效值
                Cache.Remove(cacheKey);
            }
            return default(TResult);
        }


        /// <summary>
        /// 添加或更新缓存数据。如果更新缓存，那么缓存过期时间不做更新
        /// </summary>
        /// <param name="key">key</param> 
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        /// <param name="changeMonitorList">修改监视器</param>
        public void SetList<TKey, TResult>(TKey key, IList<TResult> data, int cacheTime,
            IEnumerable<ChangeMonitor> changeMonitorList = null)
        {
            var cacheKey = GetCacheKey(key);
            if (data == null || data.Count == 0) return;
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration =
                    DateTime.Now + (cacheTime <= 0 ? TimeSpan.FromMinutes(_defaultCacheTime) : TimeSpan.FromMinutes(cacheTime))
            };
            if (changeMonitorList != null)
            {
                foreach (var changeMonitor in changeMonitorList)
                {
                    policy.ChangeMonitors.Add(changeMonitor);
                }
            }
            var flag = Cache.Add(cacheKey, data, policy);
        }

        /// <summary>
        /// 添加或更新缓存数据。如果更新缓存，那么缓存过期时间不做更新
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="regionName"></param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        /// <param name="changeMonitorList">修改监视器</param>
        public void Set<TKey, TResult>(TKey key, TResult data, int cacheTime, IEnumerable<ChangeMonitor> changeMonitorList = null)
        {
            if (TypeParse.IsDefaultValue(data)) return;
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration =
                    DateTime.Now + (cacheTime <= 0 ? TimeSpan.FromMinutes(_defaultCacheTime) : TimeSpan.FromMinutes(cacheTime))
            };
            if (changeMonitorList != null)
            {
                foreach (var changeMonitor in changeMonitorList)
                {
                    policy.ChangeMonitors.Add(changeMonitor);
                }
            }
            var cacheKey = GetCacheKey(key);
            var flag = Cache.Add(cacheKey, data, policy);
        }

     


        #region 移除缓存与清空缓存

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsSet<TKey>(TKey key)
        {
            var cacheKey = GetCacheKey(key);
            return Cache.Contains(cacheKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        public void Remove<TKey>(TKey key)
        {
            var cacheKey = GetCacheKey(key);
            Cache.Remove(cacheKey);
        }


        /// <summary>
        /// 根据正则表达式移除缓存
        /// </summary>
        /// <param name="pattern">正则表达式</param> 
        public void RemoveByPattern(string pattern)
        {
            var pattern2 = pattern.Trim('*');
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<String>();
            foreach (var item in Cache)
            {
                if (regex.IsMatch(item.Key) || string.Equals(item.Key, pattern2, StringComparison.OrdinalIgnoreCase))
                    keysToRemove.Add(item.Key);
                else if (pattern.EndsWith("*") && pattern.StartsWith("*") && item.Key.Contains(pattern2))
                {
                    keysToRemove.Add(item.Key);
                }
                else if (pattern.StartsWith("*") && item.Key.EndsWith(pattern2))
                {
                    keysToRemove.Add(item.Key);
                }
                else if (pattern.EndsWith("*") && item.Key.StartsWith(pattern2))
                {
                    keysToRemove.Add(item.Key);
                }

            }
            foreach (string key in keysToRemove)
            {
                Cache.Remove(key);
            }
        }

        /// <summary>
        /// 根据正则表达式移除缓存
        /// </summary>
        /// <param name="regionName"> </param> 
        public void RemoveByRegion()
        {
            var keyPrefix = string.Format("{0}_", (int)_regionName);
            var keysToRemove = (from item in Cache where item.Key.StartsWith(keyPrefix) select item.Key).ToList();
            foreach (string key in keysToRemove)
            {
                Cache.Remove(key);
            }
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void Clear()
        {
            foreach (var item in Cache)
            {
                Cache.Remove(item.Key);
            }
        }

        #endregion

    }
}
