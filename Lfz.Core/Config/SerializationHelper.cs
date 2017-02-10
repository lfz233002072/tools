using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Lfz.Logging;

namespace Lfz.Config
{

    /// <summary>
    /// 使用编码格式为UTF8序列化与反序列化实现辅助类
    /// </summary>
    public class SerializationHelper
    {
        private static readonly ILogger Logger;

        static SerializationHelper()
        {
            Logger = LoggerFactory.GetLog();
        }

        private static readonly Dictionary<Type, XmlSerializer> SerializerDict = new Dictionary<Type, XmlSerializer>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        private static XmlSerializer GetSerializer(Type t)
        {
            if (!SerializerDict.ContainsKey(t))
                SerializerDict.Add(t, new XmlSerializer(t));

            return SerializerDict[t];
        }

        /// <summary>
        /// 反序列化,从XML文件中反序列化数据
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="filename">文件路径</param>
        /// <returns></returns>
        public static object Load(Type type, string filename)
        {
            FileStream fs = null;
            try
            {
                // open the stream...
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return GetSerializer(type).Deserialize(fs);
            }
            catch (Exception ex)
            {
                Logger.Error("SerializationHelper.Load:" + ex.Message);
                return null;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        /// <summary>
        /// 反序列化,从XML文件中反序列化数据
        /// </summary>
        /// <param name="filename"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Load<T>(string filename) where T : class
        {
            return Load(typeof(T), filename) as T;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        public static bool Save(object obj, string filename)
        {
            FileStream fs = null;
            XmlTextWriter xtw = null;
            try
            {
                var serializer = GetSerializer(obj.GetType());
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                //导出文件时，使用缩进格式导出.
                xtw = new XmlTextWriter(fs, Encoding.UTF8) { Formatting = Formatting.None };
                serializer.Serialize(xtw, obj);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("SerializationHelper.Save Msg:{0} StackTrace:{1} Source:{2} TargetSite:{3}", ex.Message, ex.StackTrace, ex.Source, ex.TargetSite));
                return false;
            }
            finally
            {
                if (xtw != null)
                    xtw.Close();
                if (fs != null)
                    fs.Close();
            }
            return true;

        }


        /// <summary>
        /// xml序列化成字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>xml字符串</returns>
        public static string Serialize(object obj)
        {
            string returnStr = "";

            XmlSerializer serializer = GetSerializer(obj.GetType());
            var ms = new MemoryStream();
            XmlTextWriter xtw = null;
            StreamReader sr = null;
            try
            {
                xtw = new XmlTextWriter(ms, Encoding.UTF8);
                //是否使用缩进格式设置
                xtw.Formatting = Formatting.None;
                serializer.Serialize(xtw, obj);
                ms.Seek(0, SeekOrigin.Begin);
                sr = new StreamReader(ms);
                returnStr = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (xtw != null)
                    xtw.Close();
                if (sr != null)
                    sr.Close();
                ms.Close();
            }
            return returnStr;

        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="Exception">反序列化化异常</exception>
        public static object Deserialize(Type type, string s)
        {
            byte[] b = Encoding.UTF8.GetBytes(s);
            var stream = new MemoryStream(b);
            try
            {
                return GetSerializer(type).Deserialize(stream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
            }
        }
         
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns>如果反序列化化成功，那么返回类型T的实体数据，否则返回Null</returns>
        public static T Deserialize<T>(string s) where T : class
        {
            return Deserialize(typeof(T), s) as T;
        }

    }
}
