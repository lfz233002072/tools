//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : StringEventArgs
//        DESCRIPTION : 包含字符串内容的事件参数
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;

namespace Lfz.Services
{
    /// <summary>
    /// 包含字符串内容的事件参数
    /// </summary>
    public class StringEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public StringEventArgs(string str)
        {
            Content = str;
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}