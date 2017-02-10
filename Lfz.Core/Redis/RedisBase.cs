using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Lfz.Logging;
using Lfz.Rest;
using Lfz.Utitlies;
using ServiceStack.Redis;

namespace Lfz.Redis
{
    /// <summary>
    ///  
    /// </summary>
    public partial class RedisBase
    {
        private readonly IRedisConfigService _redisConfigService;
        protected static ConcurrentDictionary<string, RedisConfig> PooledConfig = new ConcurrentDictionary<string, RedisConfig>();
        protected static ConcurrentDictionary<int, PooledRedisClientManager> PooledRedisClient = new ConcurrentDictionary<int, PooledRedisClientManager>();

        protected static ILogger Logger = LoggerFactory.GetLog();

        /// <summary>Initializes a static instance of the Nop factory.</summary> 
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialize(IRedisConfigService redisConfigService)
        {
            Singleton<IRedisConfigService>.Instance = redisConfigService;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        static void InitializeRedisBase()
        {
            Singleton<RedisBase>.Instance = new RedisBase(Singleton<IRedisConfigService>.Instance);
        }

        /// <summary>
        /// 
        /// </summary>
        public static RedisBase Current
        {
            get
            {
                if (Singleton<RedisBase>.Instance == null)
                {
                    InitializeRedisBase();
                }
                return Singleton<RedisBase>.Instance;
            }
        }

        private RedisBase(IRedisConfigService redisConfigService)
        {
            _redisConfigService = redisConfigService;
        }

        #region -- 连接信息 --

        private RedisConfig GetConfig(string key)
        {
            if (_redisConfigService == null) throw new Exception("IRedisConfigService无效");

            return PooledConfig.AddOrUpdate(key, x =>
            {
                return _redisConfigService.Get(x);
            }, (x, y) =>
            {
                var temp = _redisConfigService.Get(x);
                if (y == null) y = temp;
                else
                {
                    if (temp != null)
                    {
                        y.ConfigId = temp.ConfigId;
                        y.ReadOnlyHosts = temp.ReadOnlyHosts;
                        y.ReadWriteHosts = temp.ReadWriteHosts;
                    }
                }
                return y;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal PooledRedisClientManager GetClientManager(string key)
        {
            RedisConfig config;
            PooledConfig.TryGetValue(key, out config);
            if (config == null || config.ConfigId == 0)
            {
                config = GetConfig(key);
            }
            if (config == null) return null;
            PooledRedisClientManager pml = PooledRedisClient.GetOrAdd(config.ConfigId,
                        x => CreateManager(new[] { config.ReadWriteHosts }, new[] { config.ReadOnlyHosts }));
            return pml;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        internal void RemoveClientFromPool(string key)
        {
            RedisConfig client;
            PooledConfig.TryRemove(key, out client);
            if (client != null)
            {
                PooledRedisClientManager m;
                PooledRedisClient.TryRemove(client.ConfigId, out m);
                if (m != null)
                    m.Dispose();
            }
        }

        private PooledRedisClientManager CreateManager(string[] readWriteHosts, string[] readOnlyHosts)
        {
            // 支持读写分离，均衡负载  
            return new PooledRedisClientManager(readWriteHosts, readOnlyHosts, new RedisClientManagerConfig
            {
                MaxWritePoolSize = 5, // “写”链接池链接数  
                MaxReadPoolSize = 5, // “读”链接池链接数  
                AutoStart = true,
            });
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        public void RemovePattern(string pattern)
        {
            if (_redisConfigService == null) throw new Exception("IRedisConfigService无效");

            try
            {
                var keys = _redisConfigService.GetAll();
                foreach (var config in keys)
                {
                    RemoveLike(CreateManager(new[] { config.ReadWriteHosts }, new[] { config.ReadOnlyHosts }), pattern);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "RedisBase.RemovePattern");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="pattern"></param>
        public void RemoveLike(PooledRedisClientManager manager, string pattern)
        {
            IRedisClient redis = null;
            try
            {
                redis = manager.GetClient();
                var keys = redis.SearchKeys(pattern);
                if (keys != null && keys.Any())
                {
                    redis.RemoveAll(keys);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "RedisBase.RemovePattern");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
        }

        #region -- Item --
        /// <summary> 
        /// 设置单体 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <param name="t"></param>  
        /// <returns></returns> 
        public bool Item_Set<T>(string key, T t)
        {
            return Item_Set(key, t, 60);
        }

        /// <summary> 
        /// 设置单体 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <param name="t"></param> 
        /// <param name="timeout">分钟</param> 
        /// <returns></returns> 
        public bool Item_Set<T>(string key, T t, int timeout)
        {
            if (t == null) return false;
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return false;
                redis = manager.GetClient();
                redis.SetEntry(key, JsonUtils.SerializeObject(t), new TimeSpan(0, timeout, 0));
                return true;
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Item_Set");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return false;
        }

        /// <summary> 
        /// 获取单体 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <returns></returns> 
        public T Item_Get<T>(string key)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return default(T);
                redis = manager.GetClient();
                var value = redis.GetValue(key);
                return JsonUtils.Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Item_Get");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return default(T);
        }


        /// <summary> 
        /// 设置缓存过期 
        /// </summary> 
        /// <param name="key"></param>  
        public bool Item_Remove(string key)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return true;
                redis = manager.GetClient();
                return redis.Remove(key);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Item_Remove");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return false;
        }

        /// <summary> 
        /// 设置缓存过期 
        /// </summary> 
        /// <param name="key"></param>
        /// <param name="timeout"></param> 
        public bool Item_SetExpire(string key, int timeout)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return true;
                redis = manager.GetClient();
                return redis.ExpireEntryIn(key, new TimeSpan(0, timeout, 0));
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Item_SetExpire");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return false;
        }

        #endregion

        #region -- List --

        public void List_Add<T>(string key, T t)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return;
                redis = manager.GetClient();
                var redisTypedClient = redis.As<T>();
                redisTypedClient.AddItemToList(redisTypedClient.Lists[key], t);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.List_Add");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
        }



        public bool List_Remove<T>(string key, T t)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return true;
                redis = manager.GetClient();
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.RemoveItemFromList(redisTypedClient.Lists[key], t) > 0;
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.List_Remove");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return false;
        }
        public void List_RemoveAll<T>(string key)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return;
                redis = manager.GetClient();
                var redisTypedClient = redis.As<T>();
                redisTypedClient.Lists[key].RemoveAll();
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.List_RemoveAll");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
        }

        public long List_Count(string key)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return 0;
                redis = manager.GetClient();
                return redis.GetListCount(key);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.List_Count");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return 0;
        }

        public List<T> List_GetRange<T>(string key, int start, int count)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return new List<T>();
                redis = manager.GetClient();
                var c = redis.As<T>();
                return c.Lists[key].GetRange(start, start + count - 1);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.List_GetRange");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return new List<T>();
        }


        public List<T> List_GetList<T>(string key)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return new List<T>();
                redis = manager.GetClient();
                var c = redis.As<T>();
                return c.Lists[key].GetRange(0, c.Lists[key].Count);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.List_GetList");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return new List<T>();
        }

        public List<T> List_GetList<T>(string key, int pageIndex, int pageSize)
        {
            int start = pageSize * (pageIndex - 1);
            return List_GetRange<T>(key, start, pageSize);
        }

        /// <summary> 
        /// 设置缓存过期 
        /// </summary> 
        /// <param name="key"></param> 
        /// <param name="datetime"></param> 
        public void List_SetExpire(string key, DateTime datetime)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return;
                redis = manager.GetClient();
                redis.ExpireEntryAt(key, datetime);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.List_GetList");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
        }
        #endregion

        #region -- Set --
        public void Set_Add<T>(string key, T t)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return;
                redis = manager.GetClient();
                var redisTypedClient = redis.As<T>();
                redisTypedClient.Sets[key].Add(t);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Set_Add");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
        }
        public bool Set_Contains<T>(string key, T t)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return false;
                redis = manager.GetClient();
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.Sets[key].Contains(t);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Set_Contains");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return false;
        }
        public bool Set_Remove<T>(string key, T t)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return true;
                redis = manager.GetClient();
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.Sets[key].Remove(t);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Set_Remove");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return false;
        }
        #endregion

        #region -- Hash --
        /// <summary> 
        /// 判断某个数据是否已经被缓存 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <param name="dataKey"></param> 
        /// <returns></returns> 
        public bool Hash_Exist<T>(string key, string dataKey)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return false;
                redis = manager.GetClient();
                return redis.HashContainsEntry(key, dataKey);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Set_Remove");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return false;
        }

        /// <summary> 
        /// 存储数据到hash表 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <param name="dataKey"></param> 
        /// <returns></returns> 
        public bool Hash_Set<T>(string key, string dataKey, T t, int? timeout = null)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return false;
                redis = manager.GetClient();
                string value = JsonUtils.SerializeObject(t);
                var result = redis.SetEntryInHash(key, dataKey, value);
                if (timeout.HasValue && timeout.Value > 0)
                    redis.ExpireEntryIn(key, new TimeSpan(0, timeout.Value, 0));
                return result;
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Set_Remove");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return false;
        }
        /// <summary> 
        /// 移除hash中的某值 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <param name="dataKey"></param> 
        /// <returns></returns> 
        public bool Hash_Remove(string key, string dataKey)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return false;
                redis = manager.GetClient();
                return redis.RemoveEntryFromHash(key, dataKey);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Set_Remove");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return false;
        }
        /// <summary> 
        /// 移除整个hash 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <param name="dataKey"></param> 
        /// <returns></returns> 
        public bool Hash_Remove(string key)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return false;
                redis = manager.GetClient();
                return redis.Remove(key);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Set_Remove");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return false;
        }
        /// <summary> 
        /// 从hash表获取数据 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <param name="dataKey"></param> 
        /// <returns></returns> 
        public T Hash_Get<T>(string key, string dataKey)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return default(T);
                redis = manager.GetClient();
                string value = redis.GetValueFromHash(key, dataKey);
                return JsonUtils.Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Set_Remove");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return default(T);
        }
        /// <summary> 
        /// 获取整个hash的数据 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <returns></returns> 
        public List<T> Hash_GetAll<T>(string key)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return new List<T>();
                redis = manager.GetClient();
                var list = redis.GetHashValues(key);
                if (list != null && list.Count > 0)
                {
                    List<T> result = new List<T>();
                    foreach (var item in list)
                    {
                        var value = JsonUtils.Deserialize<T>(item);
                        result.Add(value);
                    }
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Set_Remove");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return new List<T>();
        }
        /// <summary> 
        /// 设置缓存过期 
        /// </summary> 
        /// <param name="key"></param> 
        /// <param name="datetime"></param> 
        public void Hash_SetExpire(string key, DateTime datetime)
        {

            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return;
                redis = manager.GetClient();
                redis.ExpireEntryAt(key, datetime);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Set_Remove");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
        }

        /// <summary> 
        /// 获取Hash集合数量 
        /// </summary> 
        /// <param name="key">Hashid</param> 
        public long Hash_GetCount(string key)
        {
            IRedisClient redis = null;
            try
            {
                var manager = GetClientManager(key);
                if (manager == null) return 0;
                redis = manager.GetClient();
                return redis.GetHashCount(key);
            }
            catch (Exception ex)
            {
                RemoveClientFromPool(key);
                Logger.Error(ex, "RedisBase.Set_Remove");
            }
            finally
            {
                if (redis != null) redis.Dispose();
            }
            return 0;
        }
        #endregion

        #region -- SortedSet --
        /// <summary> 
        ///  添加数据到 SortedSet 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <param name="t"></param> 
        /// <param name="score"></param> 
        public bool SortedSet_Add<T>(string key, T t, double score)
        {
            using (IRedisClient redis = GetClientManager(key).GetClient())
            {
                string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
                return redis.AddItemToSortedSet(key, value, score);
            }
        }
        /// <summary> 
        /// 移除数据从SortedSet 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <param name="t"></param> 
        /// <returns></returns> 
        public bool SortedSet_Remove<T>(string key, T t)
        {
            using (IRedisClient redis = GetClientManager(key).GetClient())
            {
                string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
                return redis.RemoveItemFromSortedSet(key, value);
            }
        }
        /// <summary> 
        /// 修剪SortedSet 
        /// </summary> 
        /// <param name="key"></param> 
        /// <param name="size">保留的条数</param> 
        /// <returns></returns> 
        public long SortedSet_Trim(string key, int size)
        {
            using (IRedisClient redis = GetClientManager(key).GetClient())
            {
                return redis.RemoveRangeFromSortedSet(key, size, 9999999);
            }
        }
        /// <summary> 
        /// 获取SortedSet的长度 
        /// </summary> 
        /// <param name="key"></param> 
        /// <returns></returns> 
        public long SortedSet_Count(string key)
        {
            using (IRedisClient redis = GetClientManager(key).GetReadOnlyClient())
            {
                return redis.GetSortedSetCount(key);
            }
        }

        /// <summary> 
        /// 获取SortedSet的分页数据 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <param name="pageIndex"></param> 
        /// <param name="pageSize"></param> 
        /// <returns></returns> 
        public List<T> SortedSet_GetList<T>(string key, int pageIndex, int pageSize)
        {
            using (IRedisClient redis = GetClientManager(key).GetReadOnlyClient())
            {
                var list = redis.GetRangeFromSortedSet(key, (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                if (list != null && list.Count > 0)
                {
                    List<T> result = new List<T>();
                    foreach (var item in list)
                    {
                        var data = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                        result.Add(data);
                    }
                    return result;
                }
            }
            return null;
        }


        /// <summary> 
        /// 获取SortedSet的全部数据 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <param name="pageIndex"></param> 
        /// <param name="pageSize"></param> 
        /// <returns></returns> 
        public List<T> SortedSet_GetListALL<T>(string key)
        {
            using (IRedisClient redis = GetClientManager(key).GetReadOnlyClient())
            {
                var list = redis.GetRangeFromSortedSet(key, 0, 9999999);
                if (list != null && list.Count > 0)
                {
                    List<T> result = new List<T>();
                    foreach (var item in list)
                    {
                        var data = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                        result.Add(data);
                    }
                    return result;
                }
            }
            return null;
        }

        /// <summary> 
        /// 设置缓存过期 
        /// </summary> 
        /// <param name="key"></param> 
        /// <param name="datetime"></param> 
        public void SortedSet_SetExpire(string key, DateTime datetime)
        {
            using (IRedisClient redis = GetClientManager(key).GetClient())
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }
        #endregion
    }
}