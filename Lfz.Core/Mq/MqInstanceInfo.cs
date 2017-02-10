using System;

namespace Lfz.Mq
{
    /// <summary>
    /// 消息通信配置信息
    /// </summary>
    public class MqInstanceInfo
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
        /// MQ实例Id
        /// </summary>
        public int MqInstanceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DelFalg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ExpiredTime { get; set; }
    }
}