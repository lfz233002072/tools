//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : LoggerType
//        DESCRIPTION : 日志类型,日志等级
//
//        Created By 林芳崽 at  2013-01-04 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   
namespace Lfz.Logging
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LoggerType
    {
        /// <summary>
        /// 空日志
        /// </summary>
        NullLog = 0,
        /// <summary>
        /// 控制台日志
        /// </summary>
        ConsoleLog = 1,
        /// <summary>
        /// Log4Net日志
        /// </summary>
        Log4Net = 2,
        /// <summary>
        /// NLog日志
        /// </summary>
        NLog = 3,
    }
}