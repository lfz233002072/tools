//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : PacketBufferToken
//        DESCRIPTION : 发送或接收的数据包
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Net;
using System.Net.Sockets;

namespace Lfz.Network
{
    /// <summary>
    /// 发送或接收的数据包
    /// </summary>
    public sealed class PacketBufferToken : IDisposable
    {
        /// <summary>
        /// 缓存区域大小
        /// </summary> 
        public const int BufferSize = 2048;

        /// <summary>
        /// 缓存区值
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 缓存数据长度,套接接收的长度，而Data.Length表示缓存区最大长度
        /// </summary>
        public int DataLength
        {
            get;
            set;
        }

        /// <summary>
        /// UDP 监听Socket或TCP连接,只有在使用TCP时用到了?
        /// </summary>
        public Socket Hanlder { get; set; }

        /// <summary>
        /// 套接协议类型
        /// </summary>
        public ProtocolType ProtocolType { get; private set; }

        /// <summary>
        /// 远程IP
        /// </summary>
        public EndPoint RemoteEndPoint;




        /// <summary>
        /// 
        /// </summary>
        /// <param name="hanlder"></param>
        /// <param name="isUdp"></param>
        public PacketBufferToken(Socket hanlder, bool isUdp)
        {
            if (isUdp)
            {
                ProtocolType = ProtocolType.Udp;
            }
            else
            {
                ProtocolType = ProtocolType.Tcp;
                RemoteEndPoint = hanlder.RemoteEndPoint;
            }
            DataLength = 0;

        }

        #region IDisposable 成员

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (Hanlder != null && ProtocolType == ProtocolType.Tcp)
            {
                if (Hanlder.Connected)
                {
                    Hanlder.Shutdown(SocketShutdown.Both);
                }
                Hanlder.Close();
            }
        }

        #endregion
    }

    
}