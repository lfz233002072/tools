//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : SocketAsyncEventArgsPool
//        DESCRIPTION : Represents a collection of reusable SocketAsyncEventArgs objects. 
//
//        Created By ¡÷∑º·Ã at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Lfz.Network
{
    /// <summary>
    /// Based on example from http://msdn2.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.socketasynceventargs.aspx
    /// Represents a collection of reusable SocketAsyncEventArgs objects.  
    /// </summary>
    internal sealed class SocketAsyncEventArgsPool
    {
        /// <summary>
        /// Pool of SocketAsyncEventArgs.
        /// </summary>
        readonly Stack<SocketAsyncEventArgs> pool;

        /// <summary>
        /// Initializes the object pool to the specified size.
        /// </summary>
        /// <param name="capacity">Maximum number of SocketAsyncEventArgs objects the pool can hold.</param>
        internal SocketAsyncEventArgsPool(Int32 capacity)
        {
            pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        /// <summary>
        /// Removes a SocketAsyncEventArgs instance from the pool.
        /// </summary>
        /// <returns>SocketAsyncEventArgs removed from the pool.</returns>
        internal SocketAsyncEventArgs Pop()
        {
            lock (pool)
            {
                return pool.Count > 0 ? pool.Pop() : null;
            }
        }

        /// <summary>
        /// Add a SocketAsyncEventArg instance to the pool. 
        /// </summary>
        /// <param name="item">SocketAsyncEventArgs instance to add to the pool.</param>
        internal void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
            }
            lock (pool)
            {
                pool.Push(item);
            }
        }

        internal void Clear()
        {
            foreach (var eventArgse in pool)
            {
                eventArgse.Dispose();
            }
            pool.Clear();
        }
    }
}