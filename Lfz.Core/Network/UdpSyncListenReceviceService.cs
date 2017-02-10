//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : UdpSyncListenReceviceService
//        DESCRIPTION : 高性能Udp 异步接收数据服务
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Lfz.Logging;
using Lfz.Services;

namespace Lfz.Network
{
    /// <summary>
    /// 高性能Udp 异步接收数据服务
    /// </summary>
    internal sealed class UdpSyncListenReceviceService : ThreadServiceBase
    {
        #region 属性、字段（端口扣、线程锁等）

        /// <summary>
        /// 设置或获取通信Socket
        /// </summary>
        public Socket ListenSocket { get; set; }


        /// <summary>
        ///  the maximum number of connections the Service is designed to handle simultaneously 
        /// </summary>
        private readonly int _maxConnections;


        /// <summary>
        /// 表示一个大数据缓存
        /// </summary>
        readonly BufferManager _mBufferManager;


        /// <summary>
        ///  pool of reusable SocketAsyncEventArgs objects for  read and accept socket operations
        /// </summary>
        readonly SocketAsyncEventArgsPool _mReadPool;


        readonly Semaphore _mMaxNumberReceviceClients;


        private long _mConnectionClient = 0;
        #endregion


        /// <summary>
        /// Create an uninitialized server instance.  
        /// To start the server listening for connection requests
        /// call the Init method followed by Start method
        /// </summary>   
        /// <param name="maxConnections">the maximum number of connections the Service is designed to handle simultaneously</param>
        /// <param name="receiveBufferSize">buffer size to use for each socket I/O operation</param> 
        public UdpSyncListenReceviceService(int maxConnections, int receiveBufferSize)
        {
             
            _maxConnections = maxConnections;

            _mBufferManager = new BufferManager(receiveBufferSize * maxConnections, receiveBufferSize);

            _mReadPool = new SocketAsyncEventArgsPool(maxConnections);

            _mMaxNumberReceviceClients = new Semaphore(maxConnections, maxConnections);

            Init();
        }

        /// <summary>
        /// Initializes the server by preallocating reusable buffers and 
        /// context objects.  These objects do not need to be preallocated 
        /// or reused, but it is done this way to illustrate how the API can 
        /// easily be used to create reusable objects to increase server performance. 
        /// </summary>
        private void Init()
        {
            // Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds 
            // against memory fragmentation
            _mBufferManager.InitBuffer();
            // preallocate pool of SocketAsyncEventArgs objects 
            for (int i = 0; i < _maxConnections; i++)
            {
                //Pre-allocate a set of reusable SocketAsyncEventArgs
                var readEventArg = new SocketAsyncEventArgs();
                readEventArg.Completed += OnSocketEventCompleted;
                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                _mBufferManager.SetBuffer(readEventArg);

                // add SocketAsyncEventArg to the pool
                _mReadPool.Push(readEventArg);
            }
        }

        void BeginReceive()
        {
            _mMaxNumberReceviceClients.WaitOne();
            Interlocked.Increment(ref _mConnectionClient);

            Logger.Log(LogLevel.Trace, string.Format("BeginReceive 并发等待数量{0}", _mConnectionClient));
            try
            {
                //ReadEventArg object user token
                SocketAsyncEventArgs readEventArgs = _mReadPool.Pop();
                readEventArgs.AcceptSocket = ListenSocket;
                readEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0); ;
                bool willRaiseEvent = ListenSocket.ReceiveFromAsync(readEventArgs);
                if (!willRaiseEvent)
                {
                    ProcessReceive(readEventArgs);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, this.GetType().FullName + ex.Message, ex);
            }
            if (IsRunning) BeginReceive();
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
                case SocketAsyncOperation.ReceiveFrom:
                    ProcessReceive(e);
                    break;
                default:
                    Logger.Log(LogLevel.Error, this.GetType().FullName + "完成的操作不是ReceiveFrom");
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
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    if (ReceviceCompleted != null) ReceviceCompleted(this, e);
                }
            }
            finally
            {
                ReleaseEventArgs(e);
            }
        }

        private void ReleaseEventArgs(SocketAsyncEventArgs e)
        {
            try
            {

                // Free the SocketAsyncEventArg so they can be reused by another client
                _mReadPool.Push(e);

            }
            finally
            {
                // decrement the counter keeping track of the total number of clients connected to the server
                _mMaxNumberReceviceClients.Release();

                Interlocked.Decrement(ref _mConnectionClient);

                Logger.Log(LogLevel.Debug, string.Format("EndReceive 并发等待数量{0}", _mConnectionClient));
            }

        }

        /// <summary>
        ///  
        /// </summary>
        public event EventHandler<SocketAsyncEventArgs> ReceviceCompleted;

        public override void Dispose()
        {
            _mMaxNumberReceviceClients.Close();
            base.Dispose();
        }

        protected override void Excute(object obj)
        {
            //开始异步接收数据
            if (IsRunning) BeginReceive();
        }

        /// <summary>
        /// 并发等待数量
        /// </summary>
        public long ConcurrentWaitRecClients
        {
            get { return Interlocked.Read(ref _mConnectionClient); }
        }

    }
}