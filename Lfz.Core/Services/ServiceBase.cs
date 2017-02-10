//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : ServiceBase
//        DESCRIPTION : 服务基类,提供IsRunning,Start,Stop等
//
//        Created By 林芳崽 at  2013-01-04 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   
/***************************************************************************
 *  功能描述： 服务基类,提供IsRunning,Start,Stop等
 *  创建人：林芳崽
 *  添加时间：2013-01-04
 *  修改人：      修改时间：
 *  修改描述：
 ****************************************************************************/

using System;
using Lfz.Logging;

namespace Lfz.Services
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    public abstract class ServiceBase : IService
    {

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; protected set; }

        protected readonly object IsRunningLockObj = new object();

        /// <summary>
        /// 获取服务状态 
        /// </summary>
        public ServiceStatus Status
        {

            get
            {
                ServiceStatus flag;
                lock (IsRunningLockObj)
                {
                    flag = _status;
                }
                return flag;
            }
            protected set
            {
                lock (IsRunningLockObj)
                {
                    _status = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected ServiceBase()
        {
            Logger = LoggerFactory.GetLog();
            ServiceName = this.GetType().Name;
            Status = ServiceStatus.UnStarted;
            this._enabled = true;
        }



        /// <summary>
        /// 是否正在运行
        /// </summary>
        public virtual bool IsRunning
        {
            get { return Status == ServiceStatus.Running; }
        }

        /// <summary>
        /// 服务已经停止（事件）
        /// </summary>
        public event ServiceEventHandler Stoped;

        /// <summary>
        /// 服务正在停止（事件）,初始化关闭设置
        /// </summary>
        public event ServiceEventHandler Stoping;

        /// <summary>
        /// 服务已经启动（事件）
        /// </summary>
        public event ServiceEventHandler Started;

        /// <summary>
        /// 服务正在启动（事件），初始化启动服务
        /// </summary>
        public event ServiceEventHandler Starting;

        #region ICommandService 成员

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            //未启动该功能的服务不能启动
            if (!this.Enabled)
            {
                Status = ServiceStatus.UnStarted;
                Logger.Log(LogLevel.Debug, string.Format("{0}禁止使用，启动失败", ServiceName));
                return;
            }
            if (!IsRunning)
            {
                Status = ServiceStatus.Starting;
                if (Starting != null) Starting();
                //如果启动中没有做特殊处理，这里的状态应该为Starting
                if (Status != ServiceStatus.Starting)
                {
                    //不能启动
                    Status = ServiceStatus.UnStarted;
                    Logger.Log(LogLevel.Debug, string.Format("{0}启动失败", ServiceName));
                    return;
                } 
                if (Started != null) Started(); 
                //如果启动中没有做特殊处理，这里的状态应该为Starting
                if (Status != ServiceStatus.Starting)
                {
                    //不能启动 
                    Status = ServiceStatus.UnStarted;
                    Logger.Log(LogLevel.Debug, string.Format("{0}启动失败", ServiceName));
                    return;
                } 
                Status = ServiceStatus.Running;
                Logger.Log(LogLevel.Debug, string.Format("{0}启动", ServiceName));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            try
            {

                Status = ServiceStatus.Stoping;
                if (Stoping != null) Stoping();
                if (Status != ServiceStatus.Stoping)
                {
                    Logger.Log(LogLevel.Debug, string.Format("{0} 终止停止活动", ServiceName));
                    return;
                }
                if (Stoped != null) Stoped();
                Status = ServiceStatus.Stoped;
                Logger.Log(LogLevel.Debug, string.Format("{0}结束", ServiceName));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("{0}结束", ServiceName));
            }
        }

        #endregion

        #region IDisposable 成员

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            Stop();
        }

        #endregion

        #region IService 成员

        /// <summary>
        /// 服务日志
        /// </summary>
        public ILogger Logger { get; set; }

        #endregion

        private bool _enabled;
        private ServiceStatus _status;

        /// <summary>
        /// 服务是否启动
        /// </summary>
        public virtual bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public bool Restart()
        {
            if (this.IsRunning) this.Stop();
            this.Start();
            return true;
        }

    }

    /// <summary>
    /// 服务状态
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>
        /// 未启动(其值等效为已经停止Stoped)
        /// </summary>
        [CustomDescription("未启动")]
        UnStarted = 0,
        /// <summary>
        /// 正在启动
        /// </summary>
        [CustomDescription("正在启动")]
        Starting = 1,
        /// <summary>
        /// 运行中
        /// </summary>
        [CustomDescription("运行中")]
        Running = 2,
        /// <summary>
        /// 正在停止中=3
        /// </summary>
        [CustomDescription("正在停止中")]
        Stoping = 3,
        /// <summary>
        /// 已经停止(其值等效为未启动UnStarted)=0
        /// </summary>
        [CustomDescription("已经停止")]
        Stoped = UnStarted
    }
}