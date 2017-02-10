/*======================================================================
 *
 *        Copyright (C)  1996-2012  lfz    
 *        All rights reserved
 *
 *        Filename :AppSettingsHelper.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-09-26 16:06
 *        https://git.oschina.net/lfz/tools
 *
 *======================================================================*/

using System.Configuration;
using Lfz.Enums;

namespace Lfz.Config
{
    /// <summary>
    ///  AppSettings值获取
    /// </summary>
    public partial class AppSettingsHelper
    {
        public static string CacheItemPrefix
        {
            get { return ConfigurationManager.AppSettings["CacheItemPrefix"]; }
        }


        /// <summary>
        /// 主域名
        /// </summary>
        public static string DomainAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["DomainAddress"] ?? string.Empty;
            }
        } 
    }


    /// <summary>
    /// 附件信息
    /// </summary>
    public class AttachmentInfo
    {
        private int _maxFileSize;

        /// <summary>
        ///  上传附件最大限制（MB）
        /// </summary>
        public int MaxFileSize
        {
            get
            {
                if (_maxFileSize < 0) _maxFileSize = 2;
                return _maxFileSize;
            }
            set { _maxFileSize = value; }
        }

        /// <summary>
        /// 附件允许类型，例如：*.jpg;*.gif;*.png;*.jpeg;
        /// </summary>
        public string AttachmentExtension { get; set; }

        /// <summary>
        /// 附件类型
        /// </summary>
        public AttachmentType AttachmentType { get; set; }
    }
}