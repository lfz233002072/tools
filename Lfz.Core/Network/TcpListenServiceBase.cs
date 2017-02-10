//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : TcpListenServiceBase
//        DESCRIPTION : 高性能Tcp监听服务
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Lfz.Logging;
using Lfz.Services;

namespace Lfz.Network
{
    /// <summary>
    ///  高性能Tcp监听服务
    /// </summary>
    public abstract class TcpListenServiceBase : ServiceBase
    {
        #region 属性、字段（端口扣、线程锁等）

        /// <summary>
        /// 读取套接事件参数表
        /// </summary>
        public ConcurrentDictionary<string, SocketAsyncEventArgs> ReadSocketEventArgsTable { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int ListenPort { get; private set; }
        private Socket _listenSocket;

        /// <summary>
        /// 监听IP
        /// </summary>
        public IPAddress ListenIp { get; private set; }

        /// <summary>
        ///  the maximum number of connections the Service is designed to handle simultaneously 
        /// </summary>
        private readonly int _mNumConnections;

        private readonly int _receiveBufferSize;

        readonly BufferManager _mBufferManager;  // represents a large reusable set of buffers for all socket operations
        const int OpsToPreAlloc = 2;    // read, write (don't alloc buffer space for accepts)

        /// <summary>
        ///  pool of reusable SocketAsyncEventArgs objects for write, read and accept socket operations
        /// </summary>
        readonly SocketAsyncEventArgsPool _mReadPool;

        /// <summary>
        /// the total number of clients connected to the server 
        /// </summary>
        int _mNumConnectedSockets;

        readonly Semaphore _mMaxNumberAcceptedClients;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        protected TcpListenServiceBase(int port)
            : this(1000, 2048, string.Empty, port)
        {
        }

        /// <summary>
        /// Create an uninitialized server instance.  
        /// To start the server listening for connection requests
        /// call the Init method followed by Start method
        /// </summary>   
        /// <param name="numConnections">the maximum number of connections the Service is designed to handle simultaneously</param>
        /// <param name="receiveBufferSize">buffer size to use for each socket I/O operation</param>
        /// <param name="listenIp"></param>
        /// <param name="port"> </param>
        protected TcpListenServiceBase(int numConnections, int receiveBufferSize, string listenIp, int port)
        {
            ServiceName = string.Format("端口:{0}并发:{1}缓存:{2}", port, numConnections, receiveBufferSize);
            Stoped += OnStoped;
            Starting += OnStarting;

            ReadSocketEventArgsTable = new ConcurrentDictionary<string, SocketAsyncEventArgs>();
          
            ListenPort = port;

            IPAddress temp;
            if (!IPAddress.TryParse(listenIp, out temp)) temp = IPAddress.Any;
            ListenIp = temp;
            _mNumConnectedSockets = 0;
            _mNumConnections = numConnections;
            _receiveBufferSize = receiveBufferSize;
            // allocate buffers such that the maximum number of sockets can have one outstanding read and 
            //write posted to the socket simultaneously  
            //1W台设备，每台4096B缓存空间，Opt=2 那么需要缓存10000*4096*2/(1024.0*1024)=78.125M
            _mBufferManager = new BufferManager(receiveBufferSize * numConnections * OpsToPreAlloc,
                                                receiveBufferSize);

            _mReadPool = new SocketAsyncEventArgsPool(numConnections);
            _mMaxNumberAcceptedClients = new Semaphore(numConnections, numConnections);
            Init();
        }

        private void OnStarting()
        {
            var ipep = new IPEndPoint(ListenIp, ListenPort);
            _listenSocket = new Socket(ipep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            _listenSocket.Bind(ipep);
            _listenSocket.Listen(_mNumConnections);

            //开始异步接收数据
            StartAccept(null);
        }

        private void OnStoped()
        {
            _listenSocket.Close();
            var list = ReadSocketEventArgsTable.ToArray();
            ReadSocketEventArgsTable.Clear();
            foreach (var socketAsyncEventArgse in list)
            {
                var clientsocket = (Socket)socketAsyncEventArgse.Value.UserToken;
                if (clientsocket != null && clientsocket.Connected)
                {
                    // close the socket associated with the client
                    try
                    {
                        clientsocket.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception)
                    {
                        // throws if client process has already closed
                    }
                    finally
                    {
                        if (clientsocket.Connected)
                        {
                            clientsocket.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the server by preallocating reusable buffers and 
        /// context objects.  These objects do not need to be preallocated 
        /// or reused, but it is done this way to illustrate how the API can 
        /// easily be used to create reusable objects to increase server performance. 
        /// </summary>
        public void Init()
        {
            // Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds 
            // against memory fragmentation
            _mBufferManager.InitBuffer();

            // preallocate pool of SocketAsyncEventArgs objects 
            for (int i = 0; i < _mNumConnections; i++)
            {
                //Pre-allocate a set of reusable SocketAsyncEventArgs
                var readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += OnSocketEventCompleted;
                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                _mBufferManager.SetBuffer(readWriteEventArg);

                // add SocketAsyncEventArg to the pool
                _mReadPool.Push(readWriteEventArg);
            }
        }

        /// <summary>
        ///  Begins an operation to accept a connection request from the client   
        /// </summary>
        /// <param name="acceptEventArg"></param>
        protected void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += AcceptEventArgCompleted;
            }
            else
            {
                // socket must be cleared since the context object is being reused
                acceptEventArg.AcceptSocket = null;
            }
            _mMaxNumberAcceptedClients.WaitOne();
            bool willRaiseEvent = _listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// This method is the callback method associated with Socket.AcceptAsync 
        /// operations and is invoked when an accept operation is complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AcceptEventArgCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                try
                {
                    if (e.AcceptSocket != null)
                    {
                        Interlocked.Increment(ref _mNumConnectedSockets);
                        Logger.Log(LogLevel.Trace, string.Format("建立连接. 目前有{0}客户连接",
                                                 _mNumConnectedSockets));

                        //ReadEventArg object user token
                        SocketAsyncEventArgs readEventArgs = _mReadPool.Pop();
                        readEventArgs.UserToken = e.AcceptSocket;
                        //记录目前连接客户端
                        string key = e.AcceptSocket.RemoteEndPoint.ToString();
                        ReadSocketEventArgsTable.TryAdd(key, readEventArgs);
                        bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
                        if (!willRaiseEvent)
                        {
                            ProcessReceive(readEventArgs);
                        }
                    }
                    else Logger.Error("AcceptEventArgCompleted SocketError:" + e.SocketError);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, ServiceName + ".ProcessAccept");
                }
            }
            else
            {
                Logger.Log(LogLevel.Trace, string.Format("连接状态{0}", e.SocketError));
            }
            // Accept the next connection request
            if (IsRunning) StartAccept(e);
        }

        /// <summary> 
        /// This method is called whenever a receive or send operation is completed on a socket 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
        void OnSocketEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    Logger.Log(LogLevel.Error, "The last operation completed on the socket was not a receive or send");
                    break;
            }
        }

        /// <summary> 
        /// This method is invoked when an asynchronous receive operation completes. 
        /// If the remote host closed the connection, then the socket is closed.  
        /// If data was received then the data is echoed back to the client.
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                // check if the remote host closed the connection
                Socket token = (Socket)e.UserToken;
                // check if the remote host closed the connection 
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    if (ReceviceCompleted != null) ReceviceCompleted(token, e);  //对接收到的数据进行处理

                    if (IsRunning)
                    {
                        // read the next block of data send from the client
                        bool willRaiseEvent = token.ReceiveAsync(e);
                        if (!willRaiseEvent)
                        {
                            ProcessReceive(e);
                        }
                    }
                }
                else
                {
                    CloseClientSocket(e);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ServiceName + ".ProcessReceive"); 
                CloseClientSocket(e);
            }
        }

        /// <summary>
        /// This method is invoked when an asynchronous send operation completes.  
        /// The method issues another receive on the socket to read any additional 
        /// data sent from the client 
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            string key = ((Socket)e.UserToken).RemoteEndPoint.ToString();
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    if (SendCompleted != null) SendCompleted(e.UserToken, e);
                }
                else
                {
                    CloseClientSocket(key);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ServiceName + ".ProcessSend");  
                CloseClientSocket(key);
            }
            finally
            {
                e.Dispose();
            }
        }


        /// <summary>
        /// 根据键值关闭套件信息
        /// </summary>
        /// <param name="key"></param>
        public void CloseClientSocket(string key)
        {
            SocketAsyncEventArgs readSocketEventArgs;
            var flag = ReadSocketEventArgsTable.TryRemove(key, out   readSocketEventArgs);
            if (flag && readSocketEventArgs != null)
            {
                CloseClientSocket(readSocketEventArgs);
            }
        }

        /// <summary>
        /// 关闭套件信息
        /// </summary>
        /// <param name="e"></param>
        protected void CloseClientSocket(SocketAsyncEventArgs e)
        {
            var client = e.UserToken as Socket;
            if (client == null)
                return;

            // close the socket associated with the client
            try
            {
                //保证资源清理
                SocketAsyncEventArgs readSocketEventArgs;
                ReadSocketEventArgsTable.TryRemove(client.RemoteEndPoint.ToString(), out   readSocketEventArgs);
                e.UserToken = null;
                client.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            catch (Exception ex)
            {
                Logger.Error(ex, "CloseClientSocket");
            }
            client.Close();
            // decrement the counter keeping track of the total number of clients connected to the server
            Interlocked.Decrement(ref _mNumConnectedSockets);
            _mMaxNumberAcceptedClients.Release();
            Logger.Log(LogLevel.Trace, string.Format("关闭连接. 目前仍有{0}客户连接", _mNumConnectedSockets));
            // Free the SocketAsyncEventArg so they can be reused by another client    
            _mReadPool.Push(e);
        }

        /// <summary>
        ///  
        /// </summary>
        public event EventHandler<SocketAsyncEventArgs> SendCompleted;

        /// <summary>
        ///  
        /// </summary>
        public event EventHandler<SocketAsyncEventArgs> ReceviceCompleted;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param> 
        /// <param name="repoteEndPointStr"></param>
        protected virtual void SendAsync(byte[] data, string repoteEndPointStr)
        {
            SocketAsyncEventArgs readEventArgs;
            var flag = ReadSocketEventArgsTable.TryGetValue(repoteEndPointStr, out readEventArgs);
            if (!flag || readEventArgs == null)
            {
                Logger.Error("{0}客户连接【{1}】无效", ServiceName, repoteEndPointStr);
                return;
            }
            var write = new SocketAsyncEventArgs();
            write.Completed += OnSocketEventCompleted;
            write.AcceptSocket = (readEventArgs.UserToken as Socket);
            write.UserToken = readEventArgs.UserToken;
            var client = (readEventArgs.UserToken as Socket);
            if (client != null)
            {
                write.SetBuffer(data, 0, data.Length);
                bool willRaiseEvent = client.SendAsync(write);
                if (!willRaiseEvent)
                {
                    ProcessSend(write);
                }
            }
            else
            {
                Logger.Error(ServiceName + ".SendAsync 连接已经断开");   
            }
        }

        /// <summary>
        /// 异常键值
        /// </summary>
        /// <param name="key"></param>
        public void RemoveKey(string key)
        {
            CloseClientSocket(key);
        }
    }
}