//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : LoggerBase
//        DESCRIPTION : 日志基类
//
//        Created By 林芳崽 at  2013-01-04 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;

namespace Lfz.Logging
{
    /// <summary>
    /// 日志基类
    /// </summary>
    public abstract class LoggerBase : ILogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public virtual bool IsEnabled(LogLevel level)
        {
            return false;
        }

        /// <summary>
        /// summary
        /// </summary>
        /// <param name="level"></param>
        /// <param name="exception"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Log(LogLevel level, Exception exception, string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                string mesage = string.Format(format, args);
                Log(level, mesage, exception);
            }
            else Log(level, format, exception);
        }

        #region ILogger 成员

        /// <summary>
        /// summary
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public abstract void Log(LogLevel level, string message, Exception exception);

        #endregion

        #region ILogger 成员

        /// <summary>
        /// 信息记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Log(string message, Exception exception)
        {
            Log(LogLevel.Information, message, exception);
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LogLevel level, string message)
        {
            Log(level, message, null);
        }

        /// <summary>
        /// 提示信息记录
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            Log(message, null);
        }

        #endregion
    }
}