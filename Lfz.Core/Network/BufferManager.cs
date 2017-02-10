//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : BufferManager
//        DESCRIPTION : 缓存池管理，用户SocketAsyncEventArgs
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System.Collections.Generic;
using System.Net.Sockets;

namespace Lfz.Network
{
    /// <summary>
    /// 缓存池管理，用户SocketAsyncEventArgs
    /// </summary>
    internal class BufferManager
    {
        /// <summary>
        ///  the total number of bytes controlled by the buffer pool
        /// </summary>
        readonly int _mNumBytes;

        /// <summary>
        /// the underlying byte array maintained by the Buffer Manager
        /// </summary> 
        byte[] _mBuffer;

        readonly Stack<int> _mFreeIndexPool;
        int _mCurrentIndex;
        readonly int _mBufferSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalBytes">缓存总字节</param>
        /// <param name="bufferSize">单个缓存对象大小</param>
        public BufferManager(int totalBytes, int bufferSize)
        {
            _mNumBytes = totalBytes;
            _mCurrentIndex = 0;
            _mBufferSize = bufferSize;
            _mFreeIndexPool = new Stack<int>();
        }

        /// <summary>
        /// 使用BufferManager分配缓存控件
        /// </summary>
        public void InitBuffer()
        {
            // create one big large buffer and divide that 
            // out to each SocketAsyncEventArg object
            _mBuffer = new byte[_mNumBytes];
        }

        /// <summary>
        /// 从缓存池中分配缓存给SocketAsyncEventArgs对象
        /// </summary>
        /// <param name="args"></param>
        /// <returns>true if the buffer was successfully set, else false</returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {

            if (_mFreeIndexPool.Count > 0)
            {
                args.SetBuffer(_mBuffer, _mFreeIndexPool.Pop(), _mBufferSize);
            }
            else
            {
                if ((_mNumBytes - _mBufferSize) < _mCurrentIndex)
                {
                    return false;
                }
                args.SetBuffer(_mBuffer, _mCurrentIndex, _mBufferSize);
                _mCurrentIndex += _mBufferSize;
            }
            return true;
        }

        /// <summary>
        /// 移除SocketAsyncEventArg关联的缓存，并将其压入缓冲池
        /// </summary>
        /// <param name="args"></param>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            _mFreeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

    }
}