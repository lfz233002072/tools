/*======================================================================
 *
 *        Copyright (C)  1996-2012  lfz    
 *        All rights reserved
 *
 *        Filename :NLoger.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-01-31  17:26
 *        https://git.oschina.net/lfz/tools
 *
 *======================================================================*/

using System;

namespace Lfz.Logging
{
    /// <summary>
    /// NLog封装实现
    /// </summary>
    public class NLoger : LoggerBase
    {
        private readonly NLog.Logger _log;
        /// <summary>
        /// 
        /// </summary>
        public NLoger()
        {
            _log = NLog.LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public override bool IsEnabled(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Information:
                    return _log.IsInfoEnabled;
                case LogLevel.Debug:
                    return _log.IsDebugEnabled;
                case LogLevel.Error:
                    return _log.IsErrorEnabled;
                case LogLevel.Warning:
                    return _log.IsWarnEnabled;
                case LogLevel.Fatal:
                    return _log.IsFatalEnabled;
                default:
                    return false;
            }
        }

        /// <summary>
        /// summary
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public override void Log(LogLevel level, string message, Exception exception)
        {
            if (!this.IsEnabled(level))
                return;

            if (level == LogLevel.Information)
                _log.Info(message);
            else if (level == LogLevel.Debug)
                _log.Debug(message);
            else if (level == LogLevel.Error)
                _log.Log(NLog.LogLevel.Error, message, exception); 
            else if (level == LogLevel.Warning) 
                _log.Log(NLog.LogLevel.Warn, message, exception);
            else if (level == LogLevel.Fatal)
                _log.Log(NLog.LogLevel.Fatal, message, exception);
            else if (level == LogLevel.Trace)
                _log.Log(NLog.LogLevel.Trace, message, exception); 
        }
    }
}