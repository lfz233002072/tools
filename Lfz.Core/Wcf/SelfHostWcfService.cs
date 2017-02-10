/*======================================================================
 *
 *        Copyright (C)  1996-2012  lfz    
 *        All rights reserved
 *
 *        Filename :SelfHostWcfService.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-01-15  13:47
 *        https://git.oschina.net/lfz/tools
 *
 *======================================================================*/

using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using Lfz.Logging;
using Lfz.Services;

namespace Lfz.Wcf
{
    /// <summary>
    /// 包含一个Wcf服务的宿主服务
    /// </summary>
    public class SelfHostWcfService : ServiceBase
    {
        private readonly Type _serviceType;
        private readonly Type _implementedContract;
        private ServiceHost _host;
        private readonly object _singletonInstance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="implementedContract">服务实现的契约类型</param>
        public SelfHostWcfService(Type serviceType, Type implementedContract)
        { 
            _serviceType = serviceType;
            _implementedContract = implementedContract;
            Stoping += OnClosing;
            Starting += OnStarting;
            _singletonInstance = null;
            ServiceName = "流程引擎Http接口服务";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="singletonInstance"></param>
        /// <param name="implementedContract">服务实现的契约类型</param>
        public SelfHostWcfService(object singletonInstance, Type implementedContract)
            : this(singletonInstance.GetType(), implementedContract)
        {
            _singletonInstance = singletonInstance;
        }

        /// <summary>
        /// 服务正在启动
        /// </summary>
        private void OnStarting()
        {
            try
            {
                //如果对象不为空，那么为单一实例模式
                _host = _singletonInstance != null ? new ServiceHost(_singletonInstance) : new ServiceHost(_serviceType);
                
                _host.Opened += delegate
                    { 
                        foreach (ServiceEndpoint se in _host.Description.Endpoints)
                        {
                            var message = new StringBuilder();
                            message.Append("启动 Listener: "); 
                            message.AppendFormat(" address: {0}", se.Address);  
                            Logger.Information(message.ToString());
                        }  
                    };

                _host.Faulted += (sender, args) => Logger.Error(ServiceName + "发送异常!");
                _host.Closed += (sender, args) =>
                    Logger.Information(ServiceName + "已经关闭 ");
                _host.Open();

            }
            catch (Exception e)
            {
                Logger.Error(e, ServiceName + ".OnStarting");
                _host.Abort();
            }
        }
         
        /// <summary>
        /// 服务正在停止
        /// </summary>
        private void OnClosing()
        {
            try
            {
                if (_host != null && (_host.State == CommunicationState.Opened || _host.State == CommunicationState.Opening))
                    _host.Close();
            }
            catch (Exception e)
            {
                Logger.Error(e, ServiceName + ".OnClosing");
            }
        }
    }
}