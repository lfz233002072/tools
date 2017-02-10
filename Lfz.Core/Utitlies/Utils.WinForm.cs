using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Printing;
using System.Reflection;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using File = System.IO.File;

namespace Lfz.Utitlies
{
    public partial class Utils
    {
        public static bool RunWhenStart(bool Started, string name, string path)
        {
            bool flag = true;

            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            //打开注册表子项              
            if (key == null)
            {
                key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            }
            if (Started)
            {
                if (Convert.ToString(key.GetValue(name)) != path)
                {
                    //设置开机启动         
                    try
                    {
                        key.SetValue(name, path);
                        key.Close();
                    }
                    catch
                    {
                        flag = false;
                    }
                }
                else flag = true;
            }
            else
            {
                //取消开机启动    
                try
                {
                    if (key.GetValue(name) != null)
                    {
                        key.DeleteValue(name, false);
                        key.Close();
                    }
                }
                catch
                {
                    flag = false;
                }
            }
            return flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Created"></param>
        /// <param name="destopLinkname"></param>
        public static void CreateDesktopLink(bool Created, string destopLinkname)
        {
            if (Created)
            {
                //先判断是否存在          
                if (
                    !File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" +
                                 destopLinkname + ".lnk"))
                {
                    var shell = new WshShell();
                    var shortcut = (IWshShortcut) shell.CreateShortcut(
                        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + destopLinkname +
                        ".lnk");
                    shortcut.TargetPath = Assembly.GetExecutingAssembly().Location;
                    shortcut.WorkingDirectory = Environment.CurrentDirectory;
                    shortcut.WindowStyle = 1;
                    //Normal window                      
                    shortcut.Description = destopLinkname;
                    //
                    shortcut.IconLocation = Environment.SystemDirectory + "\\" + "shell32.dll, 165";
                    shortcut.Save();
                }
            }
            else
            {
                //先判断是否存在                   
                if (
                    File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" +
                                destopLinkname + ".lnk"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" +
                                destopLinkname + ".lnk");
                }
            }
        }

        /// <summary>
        /// 通过打印机名(路径)称获取打印队列，如果是共享打印机，则前面包含计算机名称
        /// </summary>
        /// <returns></returns>
        public static PrintQueue GetPrintQueueByPrintName(string printName)
        {
            PrintQueue pQueue = null;
            try
            {
                if (string.IsNullOrWhiteSpace(printName) == false)
                {
                    //如果是共享打印机，则其名称必然打印 = 三个斜杠+共享主机名称+打印机名称
                    if (printName.Contains(@"\\") && printName.Length >= 5)
                    {
                        //共享打印机
                        int lastIndex = printName.LastIndexOf('\\');
                        pQueue = new PrintQueue(
                            new PrintServer(printName.Substring(0, lastIndex))
                            , printName.Substring(lastIndex + 1));
                    }
                    else
                        pQueue = new PrintQueue(new LocalPrintServer(), printName);
                }
            }
            catch
            {
            }
            return pQueue;
        }

        /// <summary>
        /// 获取局域网内电脑名称
        /// </summary>
        /// <returns></returns>
        public static List<string> EnumComputers()
        {
            var computernames = new List<string>();
            using (var root = new DirectoryEntry("WinNT:"))
            {
                foreach (DirectoryEntry domain in root.Children)
                {
                    foreach (DirectoryEntry computer in domain.Children)
                    {
                        if (computer.Name != "Schema") computernames.Add(computer.Name);
                    }
                }
            }
            return computernames;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object Expression)
        {
            return TypeParse.IsDouble(Expression);
        }

        /// <summary>
        /// 格式化输出字符串
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatLong(CarryMode mode, int length, object value)
        {
            string result = string.Empty;
            double tempValue = 0, power = 1;
            if (IsDouble(value) == false)
                result = string.Empty;
            else
            {
                if (Double.TryParse(Convert.ToString(value), out tempValue) == false)
                    tempValue = 0;
                if (length < 0) length = 0;
                if (length > 15) length = 15;
                switch (mode)
                {
                    case CarryMode.Lower:
                        power = Math.Pow(10, length);
                        tempValue = Math.Round(Math.Floor(tempValue*power)/power, length);
                        break;
                    case CarryMode.Upper:
                        power = Math.Pow(10, length);
                        tempValue = Math.Round(Math.Ceiling(tempValue*power)/power, length);
                        break;
                    default:
                        tempValue = Math.Round(tempValue, length);
                        break;
                }
                string format = "f" + length.ToString();
                if (length > 0)
                    result = tempValue.ToString(format);
                else result = tempValue.ToString();
            }
            return result;
        }


        #region 是否为超级密码

        /// <summary>
        /// 是否为超级密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsSupper(string password)
        {
            var pwd = (TypeParse.StrToInt(DateTime.Now.ToString("yyyyMMdd")) + 18273645).ToString();
            return string.Equals(password, pwd, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }

    /// <summary>
    /// 进位方式
    /// </summary>
    public enum CarryMode
    {
        /// <summary>
        /// 是四舍五入的
        /// </summary>
        Round = 1,

        /// <summary>
        /// 进一法
        /// </summary>
        Upper = 2,

        /// <summary>
        /// 去尾法
        /// </summary>
        Lower = 3
    }
}