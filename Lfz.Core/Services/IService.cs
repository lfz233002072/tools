//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : IService
//        DESCRIPTION : 服务基类（提供服务启动、停止、是否正在运行以及日志等功能）
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using Lfz.Logging;

namespace Lfz.Services
{
    /// <summary>
    /// 服务基类（提供服务启动、停止、是否正在运行以及日志等功能）
    /// </summary>
    public interface IService  : IDisposable  
    {
        /// <summary>
        /// 线程是否在运行
        /// </summary>
        bool IsRunning { get; }
         
        /// <summary>
        /// 启动服务
        /// </summary> 
        void Start();

        /// <summary>
        ///  停止服务
        /// </summary>
        void Stop();

        /// <summary>
        /// 日志
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        string ServiceName { get; }


        /// <summary>
        /// 服务是否启动
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 重启服务
        /// </summary>
        /// <returns></returns>
        bool Restart();

        /// <summary>
        /// 
        /// </summary>
        ServiceStatus Status { get; }

    }

}
 