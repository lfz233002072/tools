using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Lfz.Utitlies;

namespace Lfz.IO
{
    /// <summary>
    /// 文件存储信息提供者实现
    /// </summary>
    public class FileSystemStorageProvider : IStorageProvider
    {
        private readonly string _storagePath;
        private readonly string _publicPath;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public FileSystemStorageProvider(FileSystemSettings settings)
        {
            var mediaPath = Utils.MapPath("~/");
            _storagePath = Path.Combine(mediaPath, settings.DirectoryName);

            var appPath = "";
            if (HostingEnvironment.IsHosted)
            {
                appPath = HostingEnvironment.ApplicationVirtualPath;
            }
            if (!appPath.EndsWith("/"))
                appPath = appPath + '/';
            if (!appPath.StartsWith("/"))
                appPath = '/' + appPath;

            _publicPath = appPath + settings.DirectoryName + "/";
        }


        string Map(string path)
        {
            return string.IsNullOrEmpty(path) ? _storagePath : Path.Combine(_storagePath, path);
        }

        static string Fix(string path)
        {
            return string.IsNullOrEmpty(path)
                       ? ""
                       : Path.DirectorySeparatorChar != '/'
                             ? path.Replace('/', Path.DirectorySeparatorChar)
                             : path;
        }

        #region Implementation of IStorageProvider

        /// <summary>
        /// 获取发布URL
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetPublicUrl(string path)
        {

            return _publicPath + path.Replace(Path.DirectorySeparatorChar, '/');
        }

        /// <summary>
        /// 获取物理存储路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetStoragePath(string path)
        {
            return Map(path);
        }

        /// <summary>
        /// 获取存储文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IStorageFile GetFile(string path)
        {
            if (!File.Exists(Map(path)))
            {
                throw new ArgumentException("File " + path + " does not exist");
            }
            return new FileSystemStorageFile(Fix(path), new FileInfo(Map(path)));
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Exists(string path)
        {
            return File.Exists(Map(path));
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<IStorageFile> ListFiles(string path)
        {
            if (!Directory.Exists(Map(path)))
            {
                throw new ArgumentException("Directory " + path + " does not exist");
            }

            return new DirectoryInfo(Map(path))
                .GetFiles()
                .Where(fi => !IsHidden(fi))
                .Select<FileInfo, IStorageFile>(fi => new FileSystemStorageFile(Path.Combine(Fix(path), fi.Name), fi))
                .ToList();
        }

        /// <summary>
        /// 获取文件目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<IStorageFolder> ListFolders(string path)
        {
            if (!Directory.Exists(Map(path)))
            {
                try
                {
                    Directory.CreateDirectory(Map(path));
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(string.Format("The folder could not be created at path: {0}. {1}", path, ex));
                }
            }

            return new DirectoryInfo(Map(path))
                .GetDirectories()
                .Where(di => !IsHidden(di))
                .Select<DirectoryInfo, IStorageFolder>(di => new FileSystemStorageFolder(Path.Combine(Fix(path), di.Name), di))
                .ToList();
        }

        private static bool IsHidden(FileSystemInfo di)
        {
            return (di.Attributes & FileAttributes.Hidden) != 0;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path"></param>
        public void CreateFolder(string path)
        {
            if (Directory.Exists(Map(path)))
            {
                throw new ArgumentException("Directory " + path + " already exists");
            }

            Directory.CreateDirectory(Map(path));
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path"></param>
        public void DeleteFolder(string path)
        {
            if (!Directory.Exists(Map(path)))
            {
                throw new ArgumentException("Directory " + path + " does not exist");
            }

            Directory.Delete(Map(path), true);
        }

        /// <summary>
        /// 重命名文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newPath"></param>
        public void RenameFolder(string path, string newPath)
        {
            if (!Directory.Exists(Map(path)))
            {
                throw new ArgumentException("Directory " + path + "does not exist");
            }

            if (Directory.Exists(Map(newPath)))
            {
                throw new ArgumentException("Directory " + newPath + " already exists");
            }

            Directory.Move(Map(path), Map(newPath));
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IStorageFile CreateFile(string path)
        {
            //如果目录不存在，那么先创建目录
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            } 
            if (File.Exists(Map(path)))
            {
                throw new ArgumentException("File " + path + " already exists");
            } 
            var fileInfo = new FileInfo(Map(path));
            File.WriteAllBytes(Map(path), new byte[0]);

            return new FileSystemStorageFile(Fix(path), fileInfo);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        public void DeleteFile(string path)
        {
            if (!File.Exists(Map(path)))
            {
                throw new ArgumentException("File " + path + " does not exist");
            }

            File.Delete(Map(path));
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newPath"></param>
        public void RenameFile(string path, string newPath)
        {
            if (!File.Exists(Map(path)))
            {
                throw new ArgumentException("File " + path + " does not exist");
            }

            if (File.Exists(Map(newPath)))
            {
                throw new ArgumentException("File " + newPath + " already exists");
            }

            File.Move(Map(path), Map(newPath));
        }

        #endregion

        private class FileSystemStorageFile : IStorageFile
        {
            private readonly string _path;
            private readonly FileInfo _fileInfo;

            public FileSystemStorageFile(string path, FileInfo fileInfo)
            {
                _path = path;
                _fileInfo = fileInfo;
            }

            #region Implementation of IStorageFile

            public string GetPath()
            {
                return _path;
            }

            public string GetName()
            {
                return _fileInfo.Name;
            }

            public long GetSize()
            {
                return _fileInfo.Length;
            }

            public DateTime GetLastUpdated()
            {
                return _fileInfo.LastWriteTime;
            }

            public string GetFileType()
            {
                return _fileInfo.Extension;
            }

            public Stream OpenRead()
            {
                return new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read);
            }

            public Stream OpenWrite()
            {
                return new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite);
            }

            #endregion
        }

        private class FileSystemStorageFolder : IStorageFolder
        {
            private readonly string _path;
            private readonly DirectoryInfo _directoryInfo;

            public FileSystemStorageFolder(string path, DirectoryInfo directoryInfo)
            {
                _path = path;
                _directoryInfo = directoryInfo;
            }

            #region Implementation of IStorageFolder

            public string GetPath()
            {
                return _path;
            }

            public string GetName()
            {
                return _directoryInfo.Name;
            }

            public DateTime GetLastUpdated()
            {
                return _directoryInfo.LastWriteTime;
            }

            public long GetSize()
            {
                return GetDirectorySize(_directoryInfo);
            }

            public IStorageFolder GetParent()
            {
                if (_directoryInfo.Parent != null)
                {
                    return new FileSystemStorageFolder(Path.GetDirectoryName(_path), _directoryInfo.Parent);
                }
                throw new ArgumentException("Directory " + _directoryInfo.Name + " does not have a parent directory");
            }

            #endregion

            private static long GetDirectorySize(DirectoryInfo directoryInfo)
            {
                long size = 0;

                FileInfo[] fileInfos = directoryInfo.GetFiles();
                foreach (FileInfo fileInfo in fileInfos)
                {
                    if (!IsHidden(fileInfo))
                    {
                        size += fileInfo.Length;
                    }
                }
                DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                foreach (DirectoryInfo dInfo in directoryInfos)
                {
                    if (!IsHidden(dInfo))
                    {
                        size += GetDirectorySize(dInfo);
                    }
                }

                return size;
            }
        }

    }
}