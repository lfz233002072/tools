
using System;

namespace Lfz.IO
{
    /// <summary>
    /// 文件夹信息
    /// </summary>
    public interface IStorageFolder 
    {
        /// <summary>
        /// 路径
        /// </summary>
        /// <returns></returns>
        string GetPath();
        /// <summary>
        /// 文件夹名称
        /// </summary>
        /// <returns></returns>
        string GetName();
        /// <summary>
        /// 非隐藏文件数量
        /// </summary>
        /// <returns></returns>
        long GetSize();
        /// <summary>
        /// 最近修改时间
        /// </summary>
        /// <returns></returns>
        DateTime GetLastUpdated();
        /// <summary>
        /// 上级文件夹信息
        /// </summary>
        /// <returns></returns>
        IStorageFolder GetParent();
    }
}