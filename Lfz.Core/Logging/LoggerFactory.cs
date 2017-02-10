//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : LoggerFactory
//        DESCRIPTION : 日志工厂
//
//        Created By 林芳崽 at  2013-01-04 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lfz.Config;
using Lfz.Utitlies;

namespace Lfz.Logging
{
    /// <summary>
    /// 日志工厂
    /// </summary>
    public class LoggerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggerType"></param>
        /// <returns></returns>
        public static ILogger GetLog(LoggerType loggerType)
        {
            switch (loggerType)
            {
                case LoggerType.ConsoleLog:
                    return DefaultSingleton<ConsoleLoger>.GetInstance();
                case LoggerType.NLog:
                    return DefaultSingleton<NLoger>.GetInstance();
                default:
                    return DefaultSingleton<NLoger>.GetInstance();
            }
        }

        /// <summary>
        /// 根据AppSettings["LoggerType"]配置获取日志类型
        /// </summary>
        /// <returns></returns>
        public static ILogger GetLog()
        {
            return GetLog(CurrentLoggerType);
        }

        /// <summary>
        /// 获取当前AppSettings["LoggerType"]配置的日志类型
        /// </summary>
        public static LoggerType CurrentLoggerType
        {
            get
            {
                return RunConfig.Current.LoggerType; 
            }
        }


        /// <summary>
        /// 删除过期日志文件
        /// </summary>
        /// <param name="keepDays">保留天数</param>
        public static void DeleteLogs(int keepDays = 7)
        {
            try
            {
                string pathlist = "";
                var path = Utils.MapPath("~/Logs");
                if (System.IO.Directory.Exists(path))
                {
                    var cannotDeleteFiles = new List<string>();
                    var currentDate = DateTime.Now.Date;
                    cannotDeleteFiles.Add(currentDate.ToString("yyyyMMdd"));
                    for (int i = 1; i < keepDays; i++)
                    {
                        cannotDeleteFiles.Add(currentDate.AddDays(-i).ToString("yyyyMMdd")); 
                    } 
                    DirectoryInfo info = new DirectoryInfo(path);
                    var directoryList = info.GetDirectories();
                    List<string> deletePath = new List<string>();
                    foreach (var directoryInfo in directoryList)
                    {
                        if (!cannotDeleteFiles.Any(x => string.Equals(x, directoryInfo.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            deletePath.Add(directoryInfo.FullName);
                        } 
                    }
                    foreach (var delItem in deletePath)
                    {
                        pathlist += delItem + ",";
                        Directory.Delete(delItem,true);
                    }
                }
                GetLog().Information("删除过期日志文件：" + pathlist);
            }
            catch (Exception ex)
            {
                GetLog().Error(ex, ex.Message);
            }
        }

        /// <summary>
        /// 解决删除目录提示：System.IO.IOException: 目录不是空的。
        /// 删除一个目录，先遍历删除其下所有文件和目录（递归）
        /// </summary>
        /// <param name="strPath">绝对路径</param>
        /// <returns>是否已经删除</returns>
        public static bool DeleteADirectory(string strPath)
        {
            string[] strTemp;
            try
            { 
                //删除该目录
                System.IO.Directory.Delete(strPath,true);
                return true;
            }
            catch (Exception ex)
            {
                GetLog().Error(ex, ex.Message);
                return false;
            }
        }

    }


}