//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : TcpClientService
//        DESCRIPTION : Tcp客户端封装
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Net;
using System.Net.Sockets;
using Lfz.Logging;

namespace Lfz.Network
{
    /// <summary>
    /// Tcp客户端封装
    /// </summary>
    public class TcpClientService
    {
        private readonly string _serverIp;
        private readonly int _port;
        private readonly IPEndPoint _localEP;
        private readonly int _sendTimeout;
        private readonly int _revTimeout;
        private readonly int _maxRecBufSize;

        /// <summary>
        /// 日志
        /// </summary>
        public ILogger Logger { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverIp">服务IP</param>
        /// <param name="port">端口</param>
        /// <param name="localEP">本地IP</param>
        /// <param name="sendTimeout">发送超时时间</param>
        /// <param name="revTimeout">接收超时时间</param> 
        public TcpClientService(string serverIp, int port, IPEndPoint localEP, int sendTimeout, int revTimeout)
            : this(serverIp, port, localEP, sendTimeout, revTimeout,2048)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverIp">服务IP</param>
        /// <param name="port">端口</param>
        /// <param name="localEP">本地IP</param>
        /// <param name="sendTimeout">发送超时时间</param>
        /// <param name="revTimeout">接收超时时间</param>
        /// <param name="maxRecBufSize">最大接收缓存大小</param>
        public TcpClientService(string serverIp, int port, IPEndPoint localEP, int sendTimeout, int revTimeout, int maxRecBufSize)
        {
            Logger = LoggerFactory.GetLog(); 
            _serverIp = serverIp;
            _port = port;
            _localEP = localEP;
            _sendTimeout = sendTimeout;
            _revTimeout = revTimeout;
            _maxRecBufSize = maxRecBufSize;
        }

        /// <summary>
        /// TCP客户端开始发送数据
        /// </summary>
        /// <param name="data">待发送的数据</param>
        public void BeginSend(byte[] data)
        {
            TcpClient client;
            client = _localEP != null ? new TcpClient(_localEP) : new TcpClient();
            client.SendTimeout = _sendTimeout;
            client.ReceiveTimeout = _revTimeout;
            client.BeginConnect(_serverIp, _port, BeginConnectCallBack,
                new CallBackState
                {
                    Client = client,
                    Data = data
                });
        }

        private void BeginConnectCallBack(IAsyncResult result)
        {
            var state = result.AsyncState as CallBackState;
            if (state != null)
                try
                {
                    state.Client.EndConnect(result);
                    var stream = state.Client.GetStream();
                    state.NetworkStream = stream;
                    stream.BeginWrite(state.Data, 0, state.Data.Length, BeginWriteCallBack, state);
                }
                catch (Exception e)
                {
                    CloseClient(state);
                    Logger.Log(LogLevel.Error, "BeginConnectCallBack:" + e.Message);
                }
        }

        private void CloseClient(CallBackState state)
        {
            var client = state.Client;
            client.Client.Shutdown(SocketShutdown.Send);
            client.Client.Close();
            state.NetworkStream.Dispose();
            client.Close();
            (client as IDisposable).Dispose();
        }

        private void BeginWriteCallBack(IAsyncResult result)
        {
            string ip = string.Empty;
            int port = 0;
            var state = result.AsyncState as CallBackState;
            if (state != null)
                try
                {
                    state.NetworkStream.EndWrite(result);
                    if (ReadCompleted != null)
                    {
                        state.Data = new byte[2000];
                        state.NetworkStream.BeginRead(state.Data, 0, state.Data.Length, BeginReadCallBack, state);
                    }
                    else CloseClient(state);
                }
                catch (Exception e)
                {
                    if (ReadCompleted != null)
                    {
                        CloseClient(state);
                    }
                    Logger.Log(LogLevel.Error, string.Format("BeginWriteCallBack:{0} IP:{1} port:{2}", e.Message, ip, port));
                }
        }

        private void BeginReadCallBack(IAsyncResult result)
        {
            var state = result.AsyncState as CallBackState;
            if (state != null)
                try
                {
                    var args = new DataEventArgs<byte[]>();
                    var readCount = state.NetworkStream.EndRead(result);
                    args.Data = new byte[readCount];
                    Buffer.BlockCopy(state.Data, 0, args.Data, 0, readCount);
                    ReadCompleted(this, args);
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, "BeginReadCallBack:" + e.Message);
                }
                finally
                {
                    CloseClient(state);
                }
        }


        /// <summary>
        /// TCP客户端读取完成事件
        /// </summary>
        public event EventHandler<DataEventArgs<byte[]>> ReadCompleted;


    }
}