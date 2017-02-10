using System;
using System.Configuration;
using System.IO;
using System.Xml;
using Lfz.IO;
using Lfz.Logging;
using Lfz.Security.Cryptography;
using Lfz.Utitlies;

namespace Lfz.Config
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultConfigFileManager
    {
        private static readonly ILogger Logger;
        protected static bool ShouldEncrypt;

        /// <summary>
        /// 存储信息接口提供者
        /// </summary>
        protected static IStorageProvider StorageProvider { get; set; }

        /// <summary>
        /// 配置文件存储目录获取
        /// </summary>
        protected   static string DirectoryName
        {
            get { return "Config"; }
        }

        static DefaultConfigFileManager()
        {
            Logger = LoggerFactory.GetLog();
            var value = ConfigurationManager.AppSettings["ShouldEncrypt"];
            ShouldEncrypt = TypeParse.StrToBool(value);
            StorageProvider = new FileSystemStorageProvider(new FileSystemSettings() { DirectoryName = DirectoryName });
        }

        /// <summary>
        /// 文件所在路径变量
        /// </summary>
        private static string m_configfilepath;

        /// <summary>
        /// 临时配置对象变量
        /// </summary>
        private static IConfigInfo m_configinfo = null;

        /// <summary>
        /// 锁对象
        /// </summary>
        private static object m_lockHelper = new object();


        /// <summary>
        /// 文件所在路径
        /// </summary>
        public static string ConfigFilePath
        {
            get { return m_configfilepath; }
            set { m_configfilepath = value; }
        }


        /// <summary>
        /// 临时配置对象
        /// </summary>
        public static IConfigInfo ConfigInfo
        {
            get { return m_configinfo; }
            set { m_configinfo = value; }
        }

        /// <summary>
        /// 加载(反序列化)指定对象类型的配置对象
        /// </summary>
        /// <param name="fileoldchange">文件加载时间</param>
        /// <param name="configFilePath">配置文件所在路径</param>
        /// <param name="configinfo">相应的变量 注:该参数主要用于设置m_configinfo变量 和 获取类型.GetType()</param>
        /// <returns></returns>
        protected static IConfigInfo LoadConfig(ref DateTime fileoldchange, string configFilePath, IConfigInfo configinfo)
        {
            return LoadConfig(ref fileoldchange, configFilePath, configinfo, true);
        }

        /// <summary>
        /// 加载(反序列化)指定对象类型的配置对象
        /// </summary>
        /// <param name="fileoldchange">文件加载时间</param>
        /// <param name="configFilePath">配置文件所在路径(包括文件名)</param>
        /// <param name="configinfo">相应的变量 注:该参数主要用于设置m_configinfo变量 和 获取类型.GetType()</param>
        /// <param name="checkTime">是否检查并更新传递进来的"文件加载时间"变量</param>
        /// <returns></returns>
        protected static IConfigInfo LoadConfig(ref DateTime fileoldchange, string configFilePath, IConfigInfo configinfo, bool checkTime)
        {
            m_configfilepath = configFilePath;
            m_configinfo = configinfo;

            if (checkTime)
            {
                DateTime m_filenewchange = System.IO.File.GetLastWriteTime(configFilePath);

                //当程序运行中config文件发生变化时则对config重新赋值
                if (fileoldchange != m_filenewchange)
                {
                    fileoldchange = m_filenewchange;
                    lock (m_lockHelper)
                    {
                        m_configinfo = DeserializeInfo(configFilePath, configinfo.GetType());
                    }
                }
            }
            else
            {
                lock (m_lockHelper)
                {
                    m_configinfo = DeserializeInfo(configFilePath, configinfo.GetType());
                }

            }


            return m_configinfo;
        }

        /// <summary>
        /// 反序列化指定的类
        /// </summary>
        /// <param name="configfilepath">config 文件的路径</param>
        /// <param name="configtype">相应的类型</param>
        /// <returns></returns>
        public static IConfigInfo DeserializeInfo(string configfilepath, Type configtype)
        {
            if (!File.Exists(configfilepath))
            {
                Logger.Error(string.Format("文件[{0}]不存在!", configfilepath));
                return null;
            }
            if (ShouldEncrypt)
            {
                XmlDocument document = new XmlDocument();
                document.Load(configfilepath);
                if (document.DocumentElement != null && document.DocumentElement.Name == "EncryptContent")
                {
                    var configInfo = SerializationHelper.Load<EncryptContent>(configfilepath);
                    if (configInfo == null || string.IsNullOrEmpty(configInfo.Content)) return null;
                    //机密配置内容
                    var content = TripleDESHelper.Decrypt(configInfo.Content);
                    return (IConfigInfo)SerializationHelper.Deserialize(configtype, content);
                }
            }
            return (IConfigInfo)SerializationHelper.Load(configtype, configfilepath);
        }

        /// <summary>
        /// 保存配置实例(虚方法需继承)
        /// </summary>
        /// <returns></returns>
        public virtual bool SaveConfig()
        {
            return true;
        }

        /// <summary>
        /// 保存(序列化)指定路径下的配置文件
        /// </summary>
        /// <param name="configFilePath">指定的配置文件所在的路径(包括文件名)</param>
        /// <param name="configinfo">被保存(序列化)的对象</param>
        /// <returns></returns>
        public bool SaveConfig(string configFilePath, IConfigInfo configinfo)
        {
            if (ShouldEncrypt)
            {
                EncryptContent content = new EncryptContent();
                var configstr = SerializationHelper.Serialize(configinfo);
                //需要额外加密
                content.Content = TripleDESHelper.Encrypt(configstr);
                SerializationHelper.Save(content, configFilePath);
            }
            else
                return SerializationHelper.Save(configinfo, configFilePath);
            return true;
        }
    }
}
