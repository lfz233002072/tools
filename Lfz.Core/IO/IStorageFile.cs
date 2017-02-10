using System;
using System.IO;

namespace Lfz.IO
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public interface IStorageFile
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        /// <returns></returns>
        string GetPath();
        /// <summary>
        /// 文件名称
        /// </summary>
        /// <returns></returns>
        string GetName();
        /// <summary>
        /// 文件大小
        /// </summary>
        /// <returns></returns>
        long GetSize();
        /// <summary>
        /// 最近修改时间
        /// </summary>
        /// <returns></returns>
        DateTime GetLastUpdated();
        /// <summary>
        /// 文件类型
        /// </summary>
        /// <returns></returns>
        string GetFileType();

        /// <summary>
        /// Creates a stream for reading from the file.
        /// </summary>
        Stream OpenRead();

        /// <summary>
        /// Creates a stream for writing to the file.
        /// </summary>
        Stream OpenWrite();
    }
}