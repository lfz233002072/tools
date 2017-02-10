// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :WavPlayer.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2015-09-11 10:23
// *        https://git.oschina.net/lfz/tools
// *
// *======================================================================*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Lfz.Media
{
    /// <summary>
    /// 
    /// </summary>
    public static class WavPlayer
    {
        /// <summary>
        /// 文件缓存数据
        /// </summary>
        static ConcurrentDictionary<string, byte[]> _fileByteses = new ConcurrentDictionary<string, byte[]>();
        static ConcurrentQueue<string> _fileNames = new ConcurrentQueue<string>();

        static readonly CodeAccessPermission UnmanagedCode = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);

        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        internal static extern bool PlaySound([MarshalAs(UnmanagedType.LPWStr)] string soundName, IntPtr hmod, int soundFlags);

        [DllImport("winmm.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool PlaySound(byte[] soundName, IntPtr hmod, int soundFlags);

        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        private static extern long sndPlaySound(string lpszSoundName, long uFlags);
        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum SoundFlags : int
        {
            /// <summary>play synchronously (default)</summary>
            SND_SYNC = 0x0000,
            /// <summary>play asynchronously</summary>
            SND_ASYNC = 0x0001,
            /// <summary>silence (!default) if sound not found</summary>
            SND_NODEFAULT = 0x0002,
            /// <summary>pszSound points to a memory file</summary>
            SND_MEMORY = 0x0004,
            /// <summary>loop the sound until next sndPlaySound</summary>
            SND_LOOP = 0x0008,
            /// <summary>don’t stop any currently playing sound</summary>
            SND_NOSTOP = 0x0010,
            /// <summary>Stop Playing Wave</summary>
            SND_PURGE = 0x40,
            /// <summary>don’t wait if the driver is busy</summary>
            SND_NOWAIT = 0x00002000,
            /// <summary>name is a registry alias</summary>
            SND_ALIAS = 0x00010000,
            /// <summary>alias is a predefined id</summary>
            SND_ALIAS_ID = 0x00110000,
            /// <summary>name is file name</summary>
            SND_FILENAME = 0x00020000,
            /// <summary>name is resource name or atom</summary>
            SND_RESOURCE = 0x00040004
        }


        /// <summary>
        /// 
        /// </summary> 
        public static void PlayAsync(byte[] soundName)
        {
            PlaySound(soundName, IntPtr.Zero, (int)(SoundFlags.SND_MEMORY | SoundFlags.SND_NODEFAULT | SoundFlags.SND_ASYNC));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filenameList"></param>
        public static void PlayAsync(IEnumerable<string> filenameList)
        {
            var path = Application.StartupPath + "\\Config\\Wave\\";
            UnmanagedCode.Assert();
            try
            {
                foreach (var fileName in filenameList)
                {
                    var filefullname = path + fileName;
                    if (!_fileNames.Contains(filefullname))
                    {
                        if (!File.Exists(filefullname)) continue;
                        _fileNames.Enqueue(filefullname);
                    }
                    sndPlaySound(filefullname,
                        (int)(SoundFlags.SND_FILENAME | SoundFlags.SND_NODEFAULT | SoundFlags.SND_ASYNC));
                    // PlaySound(path + fileName, IntPtr.Zero, (int)(SoundFlags.SND_FILENAME | SoundFlags.SND_NODEFAULT | SoundFlags.SND_ASYNC));
                }
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filenameList"></param>
        public static void Play(IEnumerable<string> filenameList)
        {
            var path = Application.StartupPath + "\\Config\\Wave\\";
            UnmanagedCode.Assert();
            try
            {
                foreach (var fileName in filenameList)
                {
                    var filefullname = path + fileName;
                    if (!_fileNames.Contains(filefullname))
                    {
                        if (!File.Exists(filefullname)) continue;
                        _fileNames.Enqueue(filefullname);
                    }
                    PlaySound(filefullname, IntPtr.Zero, (int)(SoundFlags.SND_FILENAME | SoundFlags.SND_NODEFAULT | SoundFlags.SND_SYNC));
                }
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filenameList"></param>
        public static void PlayAsyncByMemory(IEnumerable<string> filenameList)
        {
            var path = Application.StartupPath + "\\Config\\Wave\\";
            UnmanagedCode.Assert();
            try
            {
                byte[] data = null;
                foreach (var fileName in filenameList)
                {
                    var filefullname = path + fileName;
                    if (!_fileNames.Contains(filefullname))
                    {
                        if (!File.Exists(filefullname)) continue;
                        _fileNames.Enqueue(filefullname);
                    }
                    using (var file = File.Open(filefullname, FileMode.Open))
                    {
                        var bytes = new byte[file.Length];
                        file.Read(bytes, 0, bytes.Length);
                        if (data == null)
                            data = bytes;
                        var temp = new byte[data.Length + bytes.Length];
                        Buffer.BlockCopy(data, 0, temp, 0, data.Length);
                        Buffer.BlockCopy(bytes, 0, temp, data.Length, bytes.Length);
                        data = temp;
                    }
                }
                PlayAsync(data);
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
        }

        /// <summary>
        /// 
        /// </summary> 
        public static void PlayAsync(IEnumerable<byte[]> soundDatas)
        {
            UnmanagedCode.Assert();
            try
            {
                foreach (var soundData in soundDatas)
                {
                    PlayAsync(soundData);
                }
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
        }
    }


}