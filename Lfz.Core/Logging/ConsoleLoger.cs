//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : ConsoleLoger
//        DESCRIPTION : 控制台日志
//
//        Created By 林芳崽 at  2013-01-04 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;

namespace Lfz.Logging
{
    /// <summary>
    /// 控制台日志
    /// </summary>
    public class ConsoleLoger : LoggerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public override bool IsEnabled(LogLevel level)
        {
            return true;
        }

        /// <summary>
        /// summary
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public override void Log(LogLevel level, string message, Exception exception)
        { 
            Console.WriteLine("Level:{0} {1}  {2}  ", level.ToString(), message, exception != null ? exception.StackTrace : null);
        }
    }
}