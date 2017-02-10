/*======================================================================
 *
 *        Copyright (C)  1996-2012  lfz    
 *        All rights reserved
 *
 *        Filename :DefaultServiceHostFactory.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-07-24 16:14
 *        https://git.oschina.net/lfz/tools
 *
 *======================================================================*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;
using Lfz.Logging;
using Lfz.Security;
using Lfz.Utitlies;

namespace Lfz.Wcf
{
    /// <summary>
    /// 默认服务宿主工厂
    /// </summary>
    public class DefaultServiceHostFactory
    {
        private readonly ILogger _logger;
        private readonly IAuthenticationService _authorizationService;
        private readonly Uri _baseUriAddress;
        private readonly bool _containMeta;
        private readonly bool _includeExceptionDetailInFaults;
        private readonly bool _authorizationEnabled;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizationService">授权服务</param>
        /// <param name="baseUriAddress"></param>
        /// <param name="containMeta">是否包含元数据</param>
        /// <param name="includeExceptionDetailInFaults"> 获取或设置一个值，该值指定在返回客户端以供调试的 SOAP 错误详细信息中是否包含托管异常信息</param>
        /// <param name="authorizationEnabled">是否启动授权</param>
        public DefaultServiceHostFactory(IAuthenticationService authorizationService,
                 Uri baseUriAddress, bool containMeta = true,
                 bool includeExceptionDetailInFaults = true,
                 bool authorizationEnabled = true)
        {
            _logger = LoggerFactory.GetLog();
            _authorizationService = authorizationService;
            _baseUriAddress = baseUriAddress;
            _containMeta = containMeta;
            _includeExceptionDetailInFaults = includeExceptionDetailInFaults;
            _authorizationEnabled = authorizationEnabled;
        }

        /// <summary>
        /// 创建WCF服务宿主列表
        /// </summary>
        /// <param name="serviceInfoDictionary">服务列表</param>
        /// <returns></returns>
        public IDictionary<string, ServiceHostBase> CreateHosts(IDictionary<string, ServiceInfo> serviceInfoDictionary)
        {
            IDictionary<string, ServiceHostBase> serviceHostList = new Dictionary<string, ServiceHostBase>();
            foreach (var pairItem in serviceInfoDictionary)
            {
                if (!IsValid(pairItem.Key, pairItem.Value)) continue;
                var serviceInfo = pairItem.Value;
                var currentBaseUri = new Uri(_baseUriAddress, pairItem.Key);
                var host = new ServiceHost(serviceInfo.ServiceType, currentBaseUri);
                host.AddServiceEndpoint(serviceInfo.ContractType, GetDefaultBinding(), string.Empty);
                AddDefaultBehaviors(host.Description);
                //是否启动授权
                if (_authorizationEnabled) AddAuthorizateBehaviors(host.Description);
                host.Opened += HostOnOpened;
                host.Closed += HostOnClosed;
                host.Faulted += HostOnFaulted;
                serviceHostList.Add(pairItem.Key, host);
            }
            return serviceHostList;
        }

        private void HostOnFaulted(object sender, EventArgs eventArgs)
        {
            var host = sender as ServiceHostBase;
            if (host == null) return;
            try
            {
                foreach (ServiceEndpoint se in host.Description.Endpoints)
                {
                    var message = new StringBuilder();
                    message.AppendFormat("通信发生异常 地址：{0}", se.Address);
                    _logger.Error(message.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "HostOnFaulted");
            }
        }

        private void HostOnClosed(object sender, EventArgs eventArgs)
        {
            var host = sender as ServiceHostBase;
            if (host == null) return;
            try
            {
                foreach (ServiceEndpoint se in host.Description.Endpoints)
                {
                    var message = new StringBuilder();
                    message.AppendFormat("关闭服务成功！地址:{0}", se.Address);
                    _logger.Information(message.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "HostOnClosed");
            }

        }

        private void HostOnOpened(object sender, EventArgs eventArgs)
        {
            var host = sender as ServiceHostBase;
            if (host == null) return;
            try
            {
                foreach (ServiceEndpoint se in host.Description.Endpoints)
                {
                    var message = new StringBuilder();
                    message.AppendFormat("启动服务成功！地址:{0}", se.Address);
                    _logger.Information(message.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "HostOnOpened");
            }

        }

        /// <summary>
        /// 授权方法添加
        /// </summary>
        /// <param name="description"></param>
        private void AddAuthorizateBehaviors(ServiceDescription description)
        {
            var serviceCredentials = description.Behaviors.Find<ServiceCredentials>();
            if (serviceCredentials == null)
            {
                serviceCredentials = new ServiceCredentials();
                description.Behaviors.Add(serviceCredentials);
            }
            serviceCredentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            serviceCredentials.UserNameAuthentication.CustomUserNamePasswordValidator = new UserNamePasswordCustomValidator(_authorizationService);
            /*
            //使用测试实现安全验证
            var serviceAuthorization = description.Behaviors.Find<ServiceAuthorizationBehavior>();
            if (serviceAuthorization == null)
            {
                serviceAuthorization = new ServiceAuthorizationBehavior();
                description.Behaviors.Add(serviceAuthorization);
            }
            serviceAuthorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            serviceAuthorization.ServiceAuthorizationManager = new AuthorizationManager();
            //添加策略
             */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        private void AddDefaultBehaviors(ServiceDescription description)
        {
            #region ServiceMetadataBehavior

            if (_containMeta)
            {
                var behavior = description.Behaviors.Find<ServiceMetadataBehavior>();
                if (behavior == null)
                {
                    behavior = new ServiceMetadataBehavior();
                    description.Behaviors.Add(behavior);
                }
                behavior.HttpGetEnabled = true;
            }

            #endregion

            #region ServiceDebugBehavior

            if (_includeExceptionDetailInFaults)
            {
                var serviceDebug = description.Behaviors.Find<ServiceDebugBehavior>();
                if (serviceDebug == null)
                {
                    serviceDebug = new ServiceDebugBehavior();
                    description.Behaviors.Add(serviceDebug);
                }
                serviceDebug.IncludeExceptionDetailInFaults = true;
            }

            #endregion

        }

        private Binding GetDefaultBinding()
        {
            var binding = new BasicHttpBinding
                {
                    MaxReceivedMessageSize = 73400320,
                    MaxBufferSize = 73400320,
                    TransferMode = TransferMode.Buffered
                };
            if (binding.ReaderQuotas == null)
                binding.ReaderQuotas = new XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.MaxStringContentLength = 73400320;
            binding.ReaderQuotas.MaxArrayLength = 73400320;
            if (_authorizationEnabled)
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            else binding.Security.Mode = BasicHttpSecurityMode.None;

            if (binding.Security == null) binding.Security = new BasicHttpSecurity();
            if (binding.Security.Transport == null) binding.Security.Transport = new HttpTransportSecurity();
            if (_authorizationEnabled)
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            else
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            return binding;
        }

        private bool IsValid(string key, ServiceInfo info)
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger.Error("CreateHosts,WCF服务名称不能为空");
                return false;
            }
            if (info == null || info.ServiceType == null || info.ContractType == null)
            {
                _logger.Error(string.Format("CreateHosts,WCF服务[{0}]契约类型或实现类型为空。", key));
                return false;
            }
            return true;
        }

    }

    /// <summary>
    /// 服务基本信息
    /// </summary>
    public class ServiceInfo
    {
        /// <summary>
        /// WCF服务的具体实现类
        /// </summary>
        public Type ServiceType { get; set; }
        /// <summary>
        /// 实现WCF契约的服务类型（可以是接口或类）
        /// </summary>
        public Type ContractType { get; set; }
    }

    /// <summary>
    /// 客户端绑定辅助类
    /// </summary>
    public static class ClientServiceBindingHelper
    {
        /// <summary>
        /// GetClient 绑定信息
        /// </summary>   
        public static BasicHttpBinding GetClientBinding(bool authorizationEnabled = true)
        {
            var binding = new BasicHttpBinding
                {
                    TransferMode = TransferMode.Buffered,
                    MaxReceivedMessageSize = 73400320,
                    MaxBufferSize = 73400320
                };
            if (binding.ReaderQuotas == null)
                binding.ReaderQuotas = new XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.MaxStringContentLength = 73400320;
            binding.ReaderQuotas.MaxArrayLength = 73400320; 
            if (binding.Security == null) binding.Security = new BasicHttpSecurity();
            binding.Security.Mode = authorizationEnabled ? BasicHttpSecurityMode.TransportCredentialOnly : BasicHttpSecurityMode.None;
            binding.Security.Transport = new HttpTransportSecurity()
                {
                    ClientCredentialType = authorizationEnabled ? HttpClientCredentialType.Basic : HttpClientCredentialType.None,
                    ProxyCredentialType = HttpProxyCredentialType.None
                };
            return binding;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="username"></param>
        /// <param name="rawPassword"></param>
        /// <param name="authorizationEnabled">安全设置是否启动</param>
        /// <typeparam name="T"></typeparam>
        public static void SetCredentials<T>(this ClientBase<T> client, string username, string rawPassword,bool authorizationEnabled = true) where T : class
        {
            if (authorizationEnabled)
            {
                client.ClientCredentials.UserName.UserName = username;
                client.ClientCredentials.UserName.Password = Utils.Md5(rawPassword);
            }
        }
    }
}