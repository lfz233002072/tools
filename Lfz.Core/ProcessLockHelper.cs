using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Lfz.Logging;
using Lfz.Utitlies;

namespace Lfz
{
    /// <summary>
    /// 进程锁ID
    /// </summary>
    public static class ProcessLockHelper
    {
        private static string _processId;
        /// <summary>
        /// 进程锁ID
        /// </summary> 
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static string GetProcessLockId()
        {
            if (!string.IsNullOrEmpty(_processId)) return _processId;
            var filename = Utils.MapPath("~/Config/ProcessLock.cache");
            string content = "";
            string identity = "";
            var macaddress = "";
            var currentMac = Utils.GetNetCardMacAddress();
            try
            {
                var dir = System.IO.Path.GetDirectoryName(filename);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                } 
                if (System.IO.File.Exists(filename))
                {
                    var lines = File.ReadAllLines(filename, Encoding.UTF8);
                    if (lines != null && lines.Length >= 3)
                    {
                        content = lines[0];
                        identity = lines[1];
                        macaddress = lines[2];
                    }
                    if (!string.IsNullOrEmpty(content) &&
                       string.Equals(filename, identity, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(currentMac, macaddress, StringComparison.OrdinalIgnoreCase))
                    {
                        _processId = content;
                        return _processId;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.GetLog().Error(ex, "ProcessLockHelper.GetProcessLockId1");
            }
            try
            {
                content = Guid.NewGuid().ToString("n");
                var list = new string[] {
                    content,
                    filename,currentMac
                };
                File.WriteAllLines(filename, list, Encoding.UTF8);

                _processId = content;
                return _processId;
            }
            catch (Exception ex)
            {
                LoggerFactory.GetLog().Error(ex, "ProcessLockHelper.GetProcessLockId1");
            }
            return _processId;
        }

        /// <summary>
        /// 获取进程目录
        /// </summary>
        /// <returns></returns>
        public static string GetProcessDirectoryName()
        {
            var filename = AppDomain.CurrentDomain.BaseDirectory;
            return filename;
        }

        /// <summary>
        /// 获取进程运行mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetRunningMacAddress()
        {
            return Utils.GetNetCardMacAddress();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetRunningComputerName()
        {
            return Dns.GetHostName();
        }

        /// <summary>
        /// Ipv4获取允许获取多个
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddresses()
        {
            var hostName = Dns.GetHostName();
            var itemList = Dns.GetHostAddresses(hostName);//会返回所有地址，包括IPv4和IPv6  
            string result = "";
            foreach (var item in itemList)
            {
                var ipaddress = item.ToString();
                if (!string.IsNullOrEmpty(ipaddress)
                    && Utils.IsIP(ipaddress)
                    && !IPAddress.IsLoopback(item))
                {
                    result += ipaddress + ",";
                }
            }
            return result;
        }
    }
}
