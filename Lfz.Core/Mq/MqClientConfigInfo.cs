using System;

namespace Lfz.Mq
{
    /// <summary>
    /// 消息通信配置信息(指定客户端消息配置信息)
    /// </summary>
    public class MqClientConfigInfo
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// tcp通信端口，支持openwrite协议
        /// </summary> 
        public int Port { get; set; }

        /// <summary>
        /// mqtt协议使用端口
        /// </summary>
        public int? MqttPort { get; set; }

        /// <summary>
        /// 消息队列访问用户
        /// </summary>
        public string AccessUsername { get; set; }

        /// <summary>
        /// 消息队列访问密码
        /// </summary>
        public string AccessPassword { get; set; }

        /// <summary>
        /// 客户ID(可以是门店ID等)
        /// </summary>
        public Guid ClientId { get; set; }

        /// <summary>
        /// MQ实例Id
        /// </summary>
        public int MqInstanceId { get; set; }

        /// <summary>
        /// MQ监听实例ID
        /// </summary>
        public int MqListenerId { get; set; }
        /// <summary>
        /// 配置过期，过期之后需要重新判断配置是否有效
        /// </summary>
        public DateTime ExpiredTime { get; set; }
    }
}