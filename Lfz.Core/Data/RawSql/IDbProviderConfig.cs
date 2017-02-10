// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :DbConfig.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-04 15:59
// *        https://git.oschina.net/lfz/tools
// *
// *======================================================================*/

using System;

namespace Lfz.Data.RawSql
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDbProviderConfig
    {
        /// <summary>
        /// 
        /// </summary>
        Guid CustomerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string DataFolder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        DbProvider DbProvider { get; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
          string DbConnectionString { get;   }
    }
}