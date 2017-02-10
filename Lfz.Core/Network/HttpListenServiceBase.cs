//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : HttpListenServiceBase
//        DESCRIPTION : 使用TCP实现HTTP监听服务
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Lfz.Logging;
using Lfz.Services;

namespace Lfz.Network
{
    /// <summary>
    ///   使用TCP实现HTTP监听服务
    ///   http监听一旦数据处理完，就会关掉连接，下次有数据来就会新开一个连接，是短连接。TCP是长连接。这是httplisten与tcplisten的不同。条件允许时可以合并到一起
    /// </summary>
    public abstract class HttpListenServiceBase : ServiceBase
    {
        #region 属性、字段（端口扣、线程锁等）

        /// <summary>
        /// HTTP监控信息标识
        /// </summary>
        public string Identity
        {
            get { return string.Format("{2}:{0}:{1}", ListenIp, ListenPort, "HTTP"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ListenPort { get; protected set; }
        private Socket _listenSocket;

        /// <summary>
        /// 监听IP
        /// </summary>
        public IPAddress ListenIp { get; protected set; }

        /// <summary>
        ///  the maximum number of connections the Service is designed to handle simultaneously 
        /// </summary>
        private readonly int _mNumConnections;

        private readonly int _receiveBufferSize;
        private readonly int _maxPageSize;
        private readonly Encoding _encoding;

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
        protected HttpListenServiceBase(int port)
            : this(1000, 10240, 64000, string.Empty, port, Encoding.UTF8)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listenIp"></param>
        /// <param name="port"></param>
        protected HttpListenServiceBase(string listenIp, int port)
            : this(100, 10240, 64000, listenIp, port, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Create an uninitialized server instance.  
        /// To start the server listening for connection requests
        /// call the Init method followed by Start method
        /// </summary>   
        /// <param name="numConnections">the maximum number of connections the Service is designed to handle simultaneously</param>
        /// <param name="receiveBufferSize">buffer size to use for each socket I/O operation</param>
        /// <param name="maxPageSize"></param>
        /// <param name="listenIp"></param>
        /// <param name="port"> </param>
        /// <param name="encoding"></param>
        protected HttpListenServiceBase(int numConnections, int receiveBufferSize, int maxPageSize, string listenIp, int port, Encoding encoding)
        {
            ServiceName = string.Format("端口:{0}并发:{1}缓存:{2}", port, numConnections, receiveBufferSize);
            Stoped += OnStoped;
            Starting += OnStarting;
             
            ListenPort = port;

            IPAddress temp;
            if (!IPAddress.TryParse(listenIp, out temp)) temp = IPAddress.Any;
            ListenIp = temp;
            _mNumConnectedSockets = 0;
            _mNumConnections = numConnections;
            _receiveBufferSize = receiveBufferSize;
            _maxPageSize = maxPageSize;
            _encoding = encoding;
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
                readWriteEventArg.UserToken = new HttpListenUserToken();
                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                _mBufferManager.SetBuffer(readWriteEventArg);
                // add SocketAsyncEventArg to the pool
                _mReadPool.Push(readWriteEventArg);
            }
        }


        /// <summary>
        /// Begins an operation to accept a connection request from the client  
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
                        Logger.Log(LogLevel.Information, string.Format("建立连接. 目前有{0}客户连接",
                                                 _mNumConnectedSockets));

                        //ReadEventArg object user token
                        SocketAsyncEventArgs readEventArgs = _mReadPool.Pop();
                        ((HttpListenUserToken)readEventArgs.UserToken).ClientSocket = e.AcceptSocket;
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
                    Logger.Log(LogLevel.Error, ex.StackTrace, ex);
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
                var token = ((HttpListenUserToken)e.UserToken);

                Logger.Warning(string.Format("IP:{0} SocketError:{1} BufOffsize:{2} BufCount:{3} Transfer:{4}", e.RemoteEndPoint, e.SocketError, e.Offset, e.Count, e.BytesTransferred));
                if (e.SocketError == SocketError.Success)
                {
                    #region 数据接收处理

                    if (e.BytesTransferred > 0)
                    {
                        if (token.Data == null) token.Data = new byte[_maxPageSize];
                        int count = Math.Min(_maxPageSize - token.DataLength, e.BytesTransferred);
                        Buffer.BlockCopy(e.Buffer, e.Offset, token.Data, token.DataLength, count);
                        token.DataLength += count;
                    }

                    #endregion

                    //if (hasFinished)
                    //{
                    #region 数据发送

                    //解析数据
                    string recContent = token.Data != null
                                            ? _encoding.GetString(token.Data, 0, token.DataLength)
                                            : string.Empty;

                    HttpListenContext context = null;
                    if (token.DataLength > 0) context = new HttpListenContext(recContent); //把收到的文件夹解析成thhp包头，内容竺信息
                    string data;
                    if (context != null && context.IsValidContext &&
                        (context.HttpMethod == "GET" || context.HttpMethod == "POST"))
                    {
                        try
                        {
                            data = SuccessResponse(ProcessContext(context));  //ProcessContext得到要发送至设备的数据,并且发送给设备
                        }
                        catch (Exception exception)
                        {
                            data = SuccessResponse(ToJsonMessage(exception.Message));
                            Logger.Error(exception, "HttpListen Error");
                        }
                    }
                    else if (context == null)
                    {
                        data = SuccessResponse(ToJsonMessage("无效数据!"));
                        Logger.Error("HttpListenContext不能为空");
                    }
                    else
                    {
                        Logger.Error("HttpListenContext无效");
                        data = SuccessResponse(ToJsonMessage("无效数据!"));
                    }
                    var buf = _encoding.GetBytes(data);
                    token.IsReset = true;
                    _mBufferManager.FreeBuffer(e);
                    e.SetBuffer(buf, 0, buf.Length);
                    // read the next block of data send from the client
                    bool willRaiseEvent = token.ClientSocket.SendAsync(e);
                    if (!willRaiseEvent)
                    {
                        ProcessSend(e);
                    }

                    #endregion
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.StackTrace.ToString(), ex);
                CloseClientSocket(e);
            }
            finally
            {
                if (e.SocketError == SocketError.SocketError) CloseClientSocket(e);
            }
        }

        /// <summary>
        /// 处理信息，并返回响应
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract string ProcessContext(HttpListenContext context);

        /// <summary>
        /// This method is invoked when an asynchronous send operation completes.  
        /// The method issues another receive on the socket to read any additional 
        /// data sent from the client 
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            var token = ((HttpListenUserToken)e.UserToken);
            try
            {
                if (e.SocketError == SocketError.SocketError)
                {
                    //TODO ReceviceCompleted需要重新实现
                    if (ReceviceCompleted != null) ReceviceCompleted(this,e);
                }
                else
                {
                    Logger.Debug("发送结束或异常SocketError:" + e.SocketError);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.StackTrace, ex);
            }
            finally
            {
                //数据发送完毕
                CloseClientSocket(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected void CloseClientSocket(SocketAsyncEventArgs e)
        {
            if (e.UserToken == null) return;

            var token = ((HttpListenUserToken)e.UserToken);
            var client = token.ClientSocket;
            // close the socket associated with the client
            try
            {
                if (token.IsReset) _mBufferManager.SetBuffer(e);
                token.ClientSocket = null;
                token.DataLength = 0;
                token.Data = null;
                client.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            catch (Exception ex)
            {
                Logger.Error(ex, "CloseClientSocket");
            }
            finally
            {
                client.Close();
            }
            // decrement the counter keeping track of the total number of clients connected to the server
            Interlocked.Decrement(ref _mNumConnectedSockets);
            _mMaxNumberAcceptedClients.Release();
            Logger.Log(LogLevel.Information, string.Format("关闭连接. 目前仍有{0}客户连接", _mNumConnectedSockets));
            // Free the SocketAsyncEventArg so they can be reused by another client   
            _mReadPool.Push(e);
        }

        /// <summary>
        ///  
        /// </summary> 
        public event EventHandler<SocketAsyncEventArgs> ReceviceCompleted;

        #region 响应信息

        private string NotImplemented()
        {
            return SendResponse(
                           "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body><h2>通信服务器内置WEB服务器</h2><div>501 - Method Not Implemented</div></body></html>",
                           "501 Not Implemented", "text/html");
        }

        private string NotFound()
        {
            return SendResponse(
                         "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body><h2>通信服务器内置WEB服务器</h2><div>404 - Not Found</div></body></html>",
                         "404 Not Found", "text/html");
        }
        private string Error(Exception exception)
        {
            return SendResponse(
                         string.Format("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body><h2>通信服务器内置WEB服务器 ErrorMessage:{0}</h2><div>{1}</div></body></html>",
                         exception.Message, exception.StackTrace.ToString()),
                         "404 Not Found", "text/html");
        }

        /// <summary>
        /// 返回JSON格式消息
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns></returns>
        public string ToJsonMessage(string strContent)
        {
            return "{result:'" + strContent + "'}";
        }

        private string SendOkResponse(byte[] bContent, string contentType)
        {
            return SendResponse(bContent, "200 OK", contentType);
        }

        // For strings
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns></returns>
        public string SuccessResponse(string strContent)
        {
            int length = _encoding.GetBytes(strContent).Length;
            try
            {
                return
                    "HTTP/1.1 200 OK\r\n"
                    + "Server: Fosow.HttpListenService \r\n"
                    + "Content-Length: " + length + "\r\n"
                    + "Connection: close\r\n"
                    + "Content-Type: application/json;charset=utf-8\r\n\r\n" + strContent;
            }
            catch (Exception e)
            {
                Logger.Error("HTTP SendResponse ErrorMsg" + e.Message);
            }
            return string.Empty;
        }

        // For strings
        private string SendResponse(string strContent, string responseCode, string contentType)
        {
            byte[] bContent = _encoding.GetBytes(strContent);
            return SendResponse(bContent, responseCode, contentType);
        }

        // For byte arrays
        private string SendResponse(byte[] bContent, string responseCode, string contentType)
        {
            try
            {
                return
                    "HTTP/1.1 " + responseCode + "\r\n"
                    + "Server: Atasoy Simple Web Server\r\n"
                    + "Content-Length: " + bContent.Length.ToString() + "\r\n"
                    + "Connection: close\r\n"
                    + "Content-Type: " + contentType + "\r\n\r\n" + _encoding.GetString(bContent);
            }
            catch (Exception e)
            {
                Logger.Error("HTTP SendResponse ErrorMsg" + e.Message);
            }
            return string.Empty;
        }

        #endregion

        private class HttpListenUserToken
        {
            public Socket ClientSocket { get; set; }

            public byte[] Data { get; set; }
            public int DataLength { get; set; }
            //已经发送的数据
            public bool IsReset { get; set; }
        }
    }



}