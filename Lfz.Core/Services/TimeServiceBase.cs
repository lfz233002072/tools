//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : TimeServiceBase
//        DESCRIPTION : 定时服务基类
//
//        Created By 林芳崽 at  2012-12-12 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Timers;

namespace Lfz.Services
{
    /// <summary>
    /// 定时服务基类
    /// </summary> 
    public abstract class TimeServiceBase : ServiceBase 
    {
        private readonly Timer _timer;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        protected TimeServiceBase(TimeSpan interval)
        {
            _timer = new Timer(interval.TotalMilliseconds);
            _timer.Elapsed += Excute;
            _timer.Enabled = false;

            Stoping += OnClosing;
            Starting += OnStarting;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void Excute(object sender, ElapsedEventArgs e);

        /// <summary>
        /// 重置引发 System.Timers.Timer.Elapsed 事件的间隔时间（以毫秒为单位）。默认为 100 毫秒
        /// </summary>
        /// <param name="interval"></param>
        public void ResetInterval(TimeSpan interval)
        {
            _timer.Interval = interval.TotalMilliseconds;
        }

        /// <summary>
        /// 服务正在启动
        /// </summary>
        private void OnStarting()
        {
            _timer.Start();
        }

        /// <summary>
        /// 服务正在停止
        /// </summary>
        private void OnClosing()
        {
            _timer.Stop();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            _timer.Close();
            _timer.Dispose();
            base.Dispose();
        }
    }

     
}