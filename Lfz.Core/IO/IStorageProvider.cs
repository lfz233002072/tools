
using System.Collections.Generic;

namespace Lfz.IO
{
    /// <summary>
    /// 存储信息提供者
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// 获取发布URL
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetPublicUrl(string path);
        /// <summary>
        /// 获取物理存储路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetStoragePath(string path);
        /// <summary>
        /// 获取存储文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IStorageFile GetFile(string path);
        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IEnumerable<IStorageFile> ListFiles(string path);
        /// <summary>
        /// 获取文件目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IEnumerable<IStorageFolder> ListFolders(string path);
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path"></param>
        void CreateFolder(string path);
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path"></param>
        void DeleteFolder(string path);
        /// <summary>
        /// 重命名文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newPath"></param>
        void RenameFolder(string path, string newPath);
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        void DeleteFile(string path);
        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newPath"></param>
        void RenameFile(string path, string newPath);
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IStorageFile CreateFile(string path);
        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool Exists(string path);
    }
}