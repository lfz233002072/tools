using System;
using System.Net;

namespace Lfz.Network
{
    /// <summary>
    /// Udp接收数据事件参数
    /// </summary>
    public class UdpReceviceEventArgs : EventArgs
    {
        /// <summary>
        /// 接收的数据内容
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// UDP远程IP地址信息
        /// </summary>
        public EndPoint RemoteEndPoint { get; set; }
    }
}