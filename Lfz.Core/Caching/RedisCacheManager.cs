using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Lfz.Collections;
using Lfz.Config;
using Lfz.Redis;
using Lfz.Utitlies;

namespace Lfz.Caching
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisCacheManager
    {
        #region 属性、字段

        /// <summary>
        /// 缓存区域名称前缀
        /// </summary>  
        private readonly int _regionName;
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
        /// <param name="regionName">区域ID</param>
        /// <param name="defaultCacheTime">默认缓存时间120分钟(2个小时)，单位分钟</param>
        public RedisCacheManager(int regionName, int defaultCacheTime = 120)
        {
            _regionName = regionName;
            _defaultCacheTime = defaultCacheTime;
        }

        #endregion

        #region 辅助类

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param> 
        /// <returns></returns>
        public string GetCacheKey<TKey>(TKey key)
        {
            if (typeof (TKey) == typeof (Guid))
            {
                return string.Format("{0}{1}_{2}", AppSettingsHelper.CacheItemPrefix, (int)_regionName, GetGuidString(key));
            }
            return string.Format("{0}{1}_{2}", AppSettingsHelper.CacheItemPrefix, (int)_regionName, key);
        } 

        private string GetGuidString(object key)
        {
            return ((Guid)key).ToString("n");
        }

        #endregion

        /// <summary>
        /// 根据键值获取或更新缓存键值
        /// </summary>
        /// <param name="key">缓存键值</param> 
        /// <param name="func"></param> 
        /// <returns>返回当前键值关联的缓存对象</returns>
        public virtual TResult Get<TKey, TResult>(TKey key, Func<TKey, TResult> func)
        {
            //二级缓存
            var cacheKey = GetCacheKey(key);
            var result = RedisBase.Current.Item_Get<TResult>(cacheKey);
            //如果缓存的值为空，那么移除缓存键值
            if (!TypeParse.IsDefaultValue(result)) return result;
            //计算值
            result = func(key);
            //null ,0,Guid.Empty,string.Empty等值不做缓存
            if (TypeParse.IsDefaultValue(result))
            {
                return result;
            }
            //设置缓存
            Set(key, result, _defaultCacheTime);
            return result;
        }

        /// <summary>
        /// 根据键值获取或更新缓存键值
        /// </summary>
        /// <param name="key">缓存键值</param> 
        /// <param name="func"></param> 
        /// <returns>返回当前键值关联的缓存对象</returns>
        public virtual IEnumerable<TResult> GetList<TKey, TResult>(TKey key, Func<TKey,
            IEnumerable<TResult>> func)
        {
            //二级缓存
            var cacheKey = GetCacheKey(key);
            IEnumerable<TResult> result = RedisBase.Current.List_GetList<TResult>(cacheKey);
            //如果缓存的值为空，那么移除缓存键值  
            if (result != null && result.Any()) return result;
            //计算值
            result = func(key);
            if (result == null)
            {
                return null;
            }
            var cacheList = result.ToReadOnlyCollection();
            //设置缓存
            SetList(key, cacheList, _defaultCacheTime);
            return cacheList;
        }

        /// <summary>
        /// 根据键值获取或更新缓存键值
        /// </summary>
        /// <param name="key">缓存键值</param> 
        /// <param name="func"></param> 
        /// <returns>返回当前键值关联的缓存对象</returns>
        public virtual IDictionary<TKey1, TResult> GetDictionary<TKey, TKey1, TResult>(TKey key, Func<TKey,
            IDictionary<TKey1, TResult>> func)
        {

            //二级缓存
            var cacheKey = GetCacheKey(key);
            IDictionary<TKey1, TResult> result = RedisBase.Current.Item_Get<IDictionary<TKey1, TResult>>(cacheKey);
            //如果缓存的值为空，那么移除缓存键值  
            if (result != null && result.Any()) return result;
            //计算值
            result = func(key);
            if (result == null)
            {
                return null;
            }
            var cacheList = result;
            //设置缓存
            SetDictionary(key, cacheList, _defaultCacheTime);
            return cacheList;
        }

        /// <summary>
        /// 添加或更新缓存数据。如果更新缓存，那么缓存过期时间不做更新
        /// </summary>
        /// <param name="key">key</param> 
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        /// <param name="changeMonitorList">修改监视器</param>
        public void SetDictionary<TKey, TKey1, TResult>(TKey key, IDictionary<TKey1, TResult> data, int cacheTime,
            IEnumerable<ChangeMonitor> changeMonitorList = null)
        {
            if (data == null || data.Count == 0) return;
            //二级缓存
            var cacheKey = GetCacheKey(key);
            var expiration = cacheTime <= 0 ? _defaultCacheTime : cacheTime;
            RedisBase.Current.Item_Set<IDictionary<TKey1, TResult>>(cacheKey, data, expiration);
        }

        /// <summary>
        /// 根据键值获取或更新缓存键值
        /// </summary>
        /// <param name="key">缓存键值</param> 
        /// <returns>返回当前键值关联的缓存对象</returns>
        public virtual TResult Get<TKey, TResult>(TKey key)
        {
            //二级缓存
            var cacheKey = GetCacheKey(key);
            var result = RedisBase.Current.Item_Get<TResult>(cacheKey);
            //如果缓存的值为空，那么移除缓存键值
            if (!TypeParse.IsDefaultValue(result)) return result;
            Remove(key);
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
            if (data == null || data.Count == 0) return;
            //二级缓存
            var cacheKey = GetCacheKey(key);
            var expiration =
                DateTime.Now +
                (cacheTime <= 0 ? TimeSpan.FromMinutes(_defaultCacheTime) : TimeSpan.FromMinutes(cacheTime));
            RedisBase.Current.List_Add<IList<TResult>>(cacheKey, data);
            RedisBase.Current.List_SetExpire(cacheKey, expiration);
        }

        /// <summary>
        /// 添加或更新缓存数据。如果更新缓存，那么缓存过期时间不做更新
        /// </summary>
        /// <param name="key">key</param> 
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        /// <param name="changeMonitorList">修改监视器</param>
        public void Set<TKey, TResult>(TKey key, TResult data, int cacheTime,
            IEnumerable<ChangeMonitor> changeMonitorList = null)
        {
            //二级缓存
            var cacheKey = GetCacheKey(key);
            var expiration = cacheTime <= 0 ? _defaultCacheTime : cacheTime;
            RedisBase.Current.Item_Set<TResult>(cacheKey, data, expiration);
        }

        #region 移除缓存与清空缓存

        /// <summary>
        /// 移除指定缓存
        /// </summary>
        /// <param name="key">key</param> 
        public void Remove<TKey>(TKey key)
        {
            Remove(GetCacheKey(key));
        }

        private void Remove(string key)
        {
            RedisBase.Current.Item_Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public void RemovePattern(string str)
        {
            RedisBase.Current.RemovePattern(string.Format("*{0}*", str));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public void RemoveStartWith(string str)
        {
            str = string.Format("{0}*", str);
            RedisBase.Current.RemovePattern(str);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveRegion()
        {
            var str = string.Format("{0}{1}_*", AppSettingsHelper.CacheItemPrefix, (int)_regionName);
            RedisBase.Current.RemovePattern(str);
        }


        #endregion

    }
}