using System;

namespace Lfz.Network
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 事件携带的数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object Obj { get; set; }
    }
}