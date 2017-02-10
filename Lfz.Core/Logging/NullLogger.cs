//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : NullLogger
//        DESCRIPTION : 日志类型
//
//        Created By 林芳崽 at  2013-01-04 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;

namespace Lfz.Logging
{ 
    /// <summary>
    /// 空日志
    /// </summary>
    public class NullLogger : LoggerBase
    {
        /// <summary>
        /// 空日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public override void Log(LogLevel level, string message, Exception exception)
        {

        }
    }
}
