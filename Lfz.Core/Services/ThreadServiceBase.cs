//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : ThreadServiceBase
//        DESCRIPTION : 线程监听服务基类
//
//        Created By 林芳崽 at  2012-11-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Threading;
using Lfz.Logging;

namespace Lfz.Services
{
    /// <summary>
    /// 线程监听服务基类
    /// </summary>
    /// <typeparam name="TEventArgs">扩展参数</typeparam>
    public abstract class ThreadServiceBase  : ServiceBase 
    {
        private Thread _mUdpThread;

        /// <summary>
        /// 
        /// </summary>
        protected ThreadServiceBase()
        {
            var str = "";
            if (ThreadStartParams != null) str = "_" + ThreadStartParams.ToString();
            ServiceName = this.GetType().Name + str;

            Stoping += OnClosing;
            Started += OnStarted;
        }

        /// <summary>
        /// 线程方法
        /// </summary>
        protected abstract void Excute(object obj);

        /// <summary>
        /// 线程启动参数
        /// </summary>
        public object ThreadStartParams { get; set; }

        /// <summary>
        /// 服务正在启动
        /// </summary>
        private void OnStarted()
        {
            _mUdpThread = new Thread(RunThread) { Name = ServiceName };
            _mUdpThread.Start(ThreadStartParams);
        }

        private void RunThread(object obj)
        {
            try
            {
                Excute(obj);
            }
            catch (Exception ex)
            { 
                Status = ServiceStatus.Stoped;
                Logger.Error(this.ServiceName + " 错误信息:" + ex.Message);
            }
            finally
            { 
                Status = ServiceStatus.Stoped;
                Logger.Log(LogLevel.Debug, string.Format("{0}线程服务处理完毕并自动结束", ServiceName));
            }
        }

        /// <summary>
        /// 服务正在停止
        /// </summary>
        private void OnClosing()
        {
            if (_mUdpThread != null)
            {
                int count = 0;//避免死循环
                while (count < 100 && (_mUdpThread.ThreadState == ThreadState.Running || _mUdpThread.ThreadState == ThreadState.WaitSleepJoin))
                {
                    if (_mUdpThread.ThreadState == ThreadState.Running) _mUdpThread.Abort();
                    Thread.Sleep(10);
                    count++;
                }
            }
        } 
    }
     
}