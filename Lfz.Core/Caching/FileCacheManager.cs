//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : FileCacheManager
//        DESCRIPTION :  
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using Lfz.Logging;

namespace Lfz.Caching
{
    /// <summary>
    /// 文件缓存策略 
    /// <para>　　1、数据缓存内存中，有效期为1天</para>
    /// <para>　　2、数据缓存提示备份一份在缓存文件中。数据更新时，必须更新文件。</para>
    /// <para>　　3、缓存监视文件修改，如果有修改，那么缓存失效。</para>
    /// <para>　　4、查询数据时，如果缓存中无效，那么从文件中获取；如果文件无效，那么直接读取数据库。</para>
    /// <para>要求：</para>
    /// <para>　　数据更新必须同步缓存文件</para>   
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public partial class FileCacheManager<TResult> where TResult : class
    {
        private readonly ILogger _logger;
        private readonly ICacheSerializater _cacheSerializater;
        private readonly int _regionName;
        private readonly int _defaultCacheTime;
        private readonly NetMemoryCacheManager _cacheManager;

        /// <summary>
        /// 默认缓存文件夹
        /// </summary>
        private readonly string _defaultCacheFolder;

        /// <summary>
        /// 构造函数
        /// <para>　　使用默认序列化实现初始化文件缓存管理器</para>
        /// </summary>
        public FileCacheManager(int regionName)
            : this(new JsonFileCacheSerializater(), string.Empty, regionName)
        {
        }


        /// <summary>
        /// 构造函数
        /// <para>　　初始化文件缓存管理器</para>
        /// </summary>
        /// <param name="cacheSerializater">缓存序列化接口</param>
        /// <param name="defaultCacheFolder">默认缓存文件夹(根目录下的文件夹名称)</param> 
        /// <param name="regionName"></param> 
        public FileCacheManager(ICacheSerializater cacheSerializater, string defaultCacheFolder, int regionName)
        {
            _logger = LoggerFactory.GetLog();
            _cacheSerializater = cacheSerializater;
            _regionName = regionName;
            _defaultCacheTime = 3 * 24 * 60;
            _cacheManager = new NetMemoryCacheManager(regionName, _defaultCacheTime);
            //要求文件具有读写权限
            if (string.IsNullOrEmpty(defaultCacheFolder))
                _defaultCacheFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Caches");
            else _defaultCacheFolder = defaultCacheFolder.Trim('\\');

            Init();
        }

        private void Init()
        {
            try
            {
                string path = Path.Combine(_defaultCacheFolder, _regionName.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, string.Format("FileCacheManager.Init _regionName:{0}", _regionName));
            }
        }

        /// <summary>
        /// 更新缓存,通过更新文件触发缓存失效
        /// </summary>
        /// <param name="key"></param>
        /// <param name="regionName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool UpdateCache(Guid key, TResult data)
        {
            if (data == null) return false;
            Delete(key);
            AddCache(key, data);
            return true;
        }

        private string GetCacheFile(Guid key)
        {
            string cachefile = Path.Combine(_defaultCacheFolder, _regionName.ToString(), key.ToString() + ".cache");
            return cachefile;
        }

        /// <summary>
        /// 使用一级缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TResult Get(Guid key)
        {
            string cachefile = GetCacheFile(key);
            return _cacheSerializater.Load<TResult>(cachefile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param> 
        /// <param name="func"></param> 
        /// <param name="hasChanged">数据版本是否已经发生变更(重复验证数据有效性)</param>
        /// <returns></returns>
        public TResult Get(Guid key, Func<Guid, TResult> func, Func<Guid, TResult, bool> hasChanged = null)
        {
            TResult result = null;
            var changeList = new List<ChangeMonitor>();
            string cachefile = null;
            //直接从缓存中获取数据 
            bool isFromFile = false;
            result = _cacheManager.Get<Guid, TResult>(key);
            if (result == null)
            {
                cachefile = GetCacheFile(key);
                if (File.Exists(cachefile))
                {
                    result = _cacheSerializater.Load<TResult>(cachefile);
                    isFromFile = true;
                }
            }
            if (result != null && hasChanged != null && hasChanged(key, result))
            {
                //如果数据已经发生变化（比如直接修改数据库、或其他服务程序更新） 
                result = null;
            }
            if (result != null)
            {
                if (!isFromFile)
                    _cacheManager.Set(key, result, _defaultCacheTime, changeList);
                return result;
            }
            //如果文件中加载的数据无效，那么重新从数据库中获取数据
            result = func(key);
            if (result == null) return null;
            cachefile = cachefile ?? GetCacheFile(key);
            if (_cacheSerializater.Save(result, cachefile))
                changeList.Add(new HostFileChangeMonitor(new List<string>() { cachefile }));
            //设置缓存值
            _cacheManager.Set(key, result, _defaultCacheTime, changeList);
            return result;
        }

        /// <summary>
        /// 仅文件缓存一级缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void AddOnlyFileCache(Guid key, TResult data)
        {
            if (data == null || key == Guid.Empty) return;
            string cachefile = GetCacheFile(key);
            _cacheSerializater.Save(data, cachefile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">键值</param> 
        /// <param name="data">缓存值</param>
        public void AddCache(Guid key, TResult data)
        {
            if (data == null || key == Guid.Empty) return;
            var changeList = new List<ChangeMonitor>();
            string cachefile = GetCacheFile(key);
            changeList.Add(new HostFileChangeMonitor(new List<string>() { cachefile }));
            _cacheSerializater.Save(data, cachefile);
            //设置缓存值
            _cacheManager.Set(key, data, _defaultCacheTime, changeList);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param> 
        public void Delete(Guid key)
        {
            _cacheManager.Remove(key);
            string cachefile = GetCacheFile(key);
            if (File.Exists(cachefile))
            {
                try
                {
                    File.Delete(cachefile);
                }
                catch (Exception e)
                {

                    _logger.Error(e, "FileCacheManager.Delete");
                }
            }
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void ClearCaches()
        {
            try
            {
                _cacheManager.RemoveByRegion();
                //File.get
                var files = Directory.GetFiles(_defaultCacheFolder);
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "FileCacheManager.ClearCaches");
            }
        }
    }
}