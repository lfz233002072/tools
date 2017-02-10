using System;
using System.IO;
using System.Linq;
using Lfz.Config;
using Lfz.Logging;
using Lfz.Utitlies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TCSoft.Core.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ZipHelper.CreateZipFile(@"D:\Common", @"D:\common001.zip");
            var filelist =
                Directory.EnumerateFiles(@"D:\Common", "*", SearchOption.AllDirectories);
            ZipHelper.CreateZipFile(@"D:\Common", filelist.ToList(), @"D:\common002.zip");

            ZipHelper.CreateZipFile(@"D:\Common", @"D:\中国.zip");
            ZipHelper.CreateZipFile(@"D:\Common", filelist.ToList(), @"D:\中国2.zip");
        }


        [TestMethod]
        public void RunconfigTest()
        {
            var config = RunConfig.Current;
            config.LoggerType = 0;
            config.Save();
            config = RunConfig.Current;
            config.LoggerType = LoggerType.NLog;
            config.EnabledEncrypt = false;
            config.Save();
        }

        [TestMethod]
        public void TestHelperInitConfig()
        {
            var str =
                "data source=192.168.1.245;initial catalog=Dev_Callnumber;persist security info=True;user id=sa;password=Lfz2015";
            var config = HelperInitConfig.Current;
            config.DbConnectionString = str;
            config.Save();
        }


        [TestMethod]
        public void TestHelperInitConfigLoad()
        {
            var str =
                "data source=192.168.1.245;initial catalog=Dev_Callnumber;persist security info=True;user id=sa;password=Lfz2015";
            var config = HelperInitConfig.Current;
            Assert.AreEqual(str, config.DbConnectionString);
            config.Save();
            //RunConfig.Current.EnabledEncrypt = false;
            //RunConfig.Current.Save();
        }
    }

    public partial class HelperInitConfig : JsonConfigBase
    {
        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static HelperInitConfig Current
        {
            get
            {
                var result = Load<HelperInitConfig>();
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DbConnectionString { get; set; }

        public bool WaveAutoStartEnabled { get; set; }

        public int LockTimeout { get; set; }


        public override string GetConfigFile()
        {
            return "~/Config/HelperInit.json";
        }

    }
}
