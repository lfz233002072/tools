using System;
using System.Reflection;

namespace Lfz.Logging
{
    /// <summary>
    /// 日志扩展方法
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void Debug(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Debug, null, message, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void Information(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Information, null, message, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void Warning(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Warning, null, message, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void Error(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Error, null, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void Trace(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Trace, null, message, null);
        }
         

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void Fatal(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Fatal, null, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public static void Debug(this ILogger logger, Exception exception, string message)
        {
            FilteredLog(logger, LogLevel.Debug, exception, message, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public static void Information(this ILogger logger, Exception exception, string message)
        {
            FilteredLog(logger, LogLevel.Information, exception, message, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public static void Warning(this ILogger logger, Exception exception, string message)
        {
            FilteredLog(logger, LogLevel.Warning, exception, message, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public static void Error(this ILogger logger, Exception exception, string message)
        {
            FilteredLog(logger, LogLevel.Error, exception, message, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public static void Fatal(this ILogger logger, Exception exception, string message)
        {
            FilteredLog(logger, LogLevel.Fatal, exception, message, null);
        }

        // ReSharper disable MethodOverloadWithOptionalParameter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="format">string.Format(format,arg) 中的第一个参数</param>
        /// <param name="args">string.Format(format,args) 中的第二个参数</param>
        public static void Debug(this ILogger logger, string format, params object[] args)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            if (args == null) throw new ArgumentNullException("args");
            FilteredLog(logger, LogLevel.Debug, null, format, args);
        }

        // ReSharper disable MethodOverloadWithOptionalParameter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="format">string.Format(format,arg) 中的第一个参数</param>
        /// <param name="args">string.Format(format,args) 中的第二个参数</param>
        public static void Information(this ILogger logger, string format, params object[] args)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            FilteredLog(logger, LogLevel.Information, null, format, args);
        }
        // ReSharper disable MethodOverloadWithOptionalParameter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="format">string.Format(format,arg) 中的第一个参数</param>
        /// <param name="args">string.Format(format,args) 中的第二个参数</param>
        public static void Warning(this ILogger logger, string format, params object[] args)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            FilteredLog(logger, LogLevel.Warning, null, format, args);
        }
        // ReSharper disable MethodOverloadWithOptionalParameter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="format">string.Format(format,arg) 中的第一个参数</param>
        /// <param name="args">string.Format(format,args) 中的第二个参数</param>
        public static void Error(this ILogger logger, string format, params object[] args)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            FilteredLog(logger, LogLevel.Error, null, format, args);
        }
        // ReSharper disable MethodOverloadWithOptionalParameter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="format">string.Format(format,arg) 中的第一个参数</param>
        /// <param name="args">string.Format(format,args) 中的第二个参数</param>
        public static void Fatal(this ILogger logger, string format, params object[] args)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            FilteredLog(logger, LogLevel.Fatal, null, format, args);
        }

        // ReSharper disable MethodOverloadWithOptionalParameter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="format">string.Format(format,arg) 中的第一个参数</param>
        /// <param name="args">string.Format(format,args) 中的第二个参数</param>
        public static void Debug(this ILogger logger, Exception exception, string format, params object[] args)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            FilteredLog(logger, LogLevel.Debug, exception, format, args);
        }
        // ReSharper disable MethodOverloadWithOptionalParameter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="format">string.Format(format,arg) 中的第一个参数</param>
        /// <param name="args">string.Format(format,args) 中的第二个参数</param>
        public static void Information(this ILogger logger, Exception exception, string format, params object[] args)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            FilteredLog(logger, LogLevel.Information, exception, format, args);
        }
        // ReSharper disable MethodOverloadWithOptionalParameter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="format">string.Format(format,arg) 中的第一个参数</param>
        /// <param name="args">string.Format(format,args) 中的第二个参数</param>
        public static void Warning(this ILogger logger, Exception exception, string format, params object[] args)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            FilteredLog(logger, LogLevel.Warning, exception, format, args);
        }
        // ReSharper disable MethodOverloadWithOptionalParameter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="format">string.Format(format,arg) 中的第一个参数</param>
        /// <param name="args">string.Format(format,args) 中的第二个参数</param>
        public static void Error(this ILogger logger, Exception exception, string format, params object[] args)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            FilteredLog(logger, LogLevel.Error, exception, format, args);
        }

        // ReSharper disable MethodOverloadWithOptionalParameter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="methodBase"></param> 
        public static void Error(this ILogger logger, Exception exception, MethodBase methodBase)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            string format = (methodBase.DeclaringType != null ? methodBase.DeclaringType.FullName + "." + methodBase.Name : string.Empty);
            FilteredLog(logger, LogLevel.Error, exception, format, null);
        }



        // ReSharper disable MethodOverloadWithOptionalParameter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="format">string.Format(format,arg) 中的第一个参数</param>
        /// <param name="args">string.Format(format,args) 中的第二个参数</param>
        public static void Fatal(this ILogger logger, Exception exception, string format, params object[] args)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            FilteredLog(logger, LogLevel.Fatal, exception, format, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="exception"></param>
        /// <param name="format">string.Format(format,arg) 中的第一个参数</param>
        /// <param name="objects">string.Format(format,args) 中的第二个参数</param>
        private static void FilteredLog(ILogger logger, LogLevel level, Exception exception, string format, object[] objects)
        {
            if (logger.IsEnabled(level))
            {
                logger.Log(level, exception, format, objects);
            }
        }
    }
}