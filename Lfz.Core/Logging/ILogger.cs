//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : ILogger
//        DESCRIPTION : 日志接口
//
//        Created By 林芳崽 at  2013-01-04 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================  

using System;

namespace Lfz.Logging 
{
    /// <summary>
    /// 日志基本接口
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        bool IsEnabled(LogLevel level);
        /// <summary>
        /// summary
        /// </summary>
        /// <param name="level"></param>
        /// <param name="exception"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Log(LogLevel level, Exception exception, string format, params object[] args);
        /// <summary>
        /// summary
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Log(LogLevel level, string message, Exception exception);
        /// <summary>
        /// 错误提示信息记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Log(string message, Exception exception);
        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        void Log(LogLevel level, string message );

        /// <summary>
        /// 提示信息记录
        /// </summary>
        /// <param name="message"></param>
        void Log(string message);
    }
}