// ======================================================================
// 	
// 	Copyright (C)  1996-2012  lfz    
// 	All rights reserved
// 	
// 	Filename :InternalUserRoleType.cs
// 	DESCRIPTION :内置角色类型
// 	
// 	Created By 林芳崽 at 2014-04-30 14:40
// 	https://git.oschina.net/lfz/tools
// 
// ======================================================================*/

using System;

namespace Lfz.Enums.Hr
{
    /// <summary>
    /// 内置角色类型
    /// </summary>
    [Flags]
    public enum InternalUserRoleType
    {
        /// <summary>
        /// 系统超级管理员
        /// </summary>
        [CustomDescription("系统管理员")]
        InternalSupperAdmin=2,

        /// <summary>
        /// 商家管理员
        /// </summary>
        [CustomDescription("商家管理员")]
        InternalCustomerAdmin=4,

        /// <summary>
        /// 商家员工
        /// </summary>
        [CustomDescription("商家员工")]
        InternalCustomerEmployee=8,
         
        /// <summary>
        /// 注册用户
        /// </summary>
        [CustomDescription("注册用户")]
        RegisterUser =16,

        /// <summary>
        ///  
        /// </summary> 
        None =0,
    }
}