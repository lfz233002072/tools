using System;
using System.IO;
using System.Text;
using Lfz.Config;
using Lfz.Logging;
using Lfz.Rest;
using Lfz.Security.Cryptography;
using Newtonsoft.Json;

namespace Lfz.Caching
{
    /// <summary>
    /// 缓存数据序列化
    /// </summary>
    public interface ICacheSerializater
    {

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        bool Save(object obj, string filename);

        /// <summary>
        /// 反序列化,从XML文件中反序列化数据
        /// </summary>
        /// <param name="filename"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Load<T>(string filename) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        object Load(Type elementType, string filename);
    }

    /// <summary>
    /// 缓存数据序列化默认实现
    /// </summary>
    public class DefaultCacheSerializater : ICacheSerializater
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        public virtual bool Save(object obj, string filename)
        {
            return SerializationHelper.Save(obj, filename);
        }

        /// <summary>
        /// 反序列化,从XML文件中反序列化数据
        /// </summary>
        /// <param name="filename"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T Load<T>(string filename) where T : class
        {
            return SerializationHelper.Load<T>(filename);
        }

        public object Load(Type elementType, string filename)
        {
            return SerializationHelper.Load(elementType, filename);
        }
    }


    /// <summary>
    /// 缓存数据序列化默认实现
    /// </summary>
    public class JsonFileCacheSerializater : ICacheSerializater
    {
        private static readonly ILogger Logger;

        static JsonFileCacheSerializater()
        {
            Logger = LoggerFactory.GetLog(LoggerType.NLog);
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        public virtual bool Save(object obj, string filename)
        {
            if (obj == null) return true;
            FileStream fs = null;
            try
            {
                string data;
                var elementType = obj.GetType();
                var runConfigType = typeof(RunConfig);
                if (elementType != runConfigType && !runConfigType.IsAssignableFrom(elementType))
                {
                    if (RunConfig.Current.EnabledEncrypt)
                    {
                        data = JsonUtils.SerializeObject(obj, Formatting.None);
                        data = TripleDESHelper.Encrypt(data);
                    }
                    else
                    {
                        data = JsonUtils.SerializeObject(obj, Formatting.Indented);
                    }
                }
                else data = JsonUtils.SerializeObject(obj, Formatting.Indented);
                if (string.IsNullOrEmpty(data)) return true;
                var bytes = System.Text.Encoding.UTF8.GetBytes(data);
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                fs.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("JsonFileCacheSerializater.Save Msg:{0} StackTrace:{1} Source:{2} TargetSite:{3}", ex.Message, ex.StackTrace, ex.Source, ex.TargetSite));
                return false;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return true;
        }

        /// <summary>
        /// 反序列化,从XML文件中反序列化数据
        /// </summary>
        /// <param name="filename"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T Load<T>(string filename) where T : class
        {
            StreamReader reader = null;
            try
            {
                if (!File.Exists(filename)) return null;
                reader = new StreamReader(filename, Encoding.UTF8);
                var str = reader.ReadToEnd();
                if (string.IsNullOrEmpty(str)) return null;
                var elementType = typeof(T);
                var runConfigType = typeof(RunConfig);
                if (elementType != runConfigType
                    && !runConfigType.IsAssignableFrom(elementType)
                    && !str.Contains("{") && !str.Contains("["))
                {
                    //不是个有效json数据,那么判断是否为机密过的数据
                    //要求加载数据时，不管配置信息是否要求加密，都能根据实际数据解析出结果
                    str = TripleDESHelper.Decrypt(str);
                }
                return JsonUtils.Deserialize<T>(str);
            }
            catch (Exception ex)
            {
                Logger.Error("SerializationHelper.Load:" + ex.Message);
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// 反序列化,从XML文件中反序列化数据
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="filename"></param> 
        /// <returns></returns>
        public virtual object Load(Type elementType, string filename)
        {
            StreamReader reader = null;
            try
            {
                if (!File.Exists(filename)) return null;
                reader = new StreamReader(filename, Encoding.UTF8);
                var str = reader.ReadToEnd();
                var runConfigType = typeof(RunConfig);
                if (elementType != runConfigType
                  && !runConfigType.IsAssignableFrom(elementType)
                  && !str.Contains("{") && !str.Contains("["))
                {
                    //不是个有效json数据,那么判断是否为机密过的数据
                    //要求加载数据时，不管配置信息是否要求加密，都能根据实际数据解析出结果
                    str = TripleDESHelper.Decrypt(str);
                }
                return JsonUtils.DeserializeObject(str, elementType);
            }
            catch (Exception ex)
            {
                Logger.Error("SerializationHelper.Load:" + ex.Message);
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}