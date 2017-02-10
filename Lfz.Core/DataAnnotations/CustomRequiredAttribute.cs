/*======================================================================
 *
 *        Copyright (C)  1996-2012  lfz    
 *        All rights reserved
 *
 *        Filename :CustomRequiredAttribute.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-09-09 15:06
 *        https://git.oschina.net/lfz/tools
 *
 *======================================================================*/

using System;

namespace Lfz.DataAnnotations
{
    /// <summary>
    /// 需要做隐藏控制的类中添加属性控制 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class DynamicHidenAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="corpTypeProperty">企业类型依赖的属性</param> 
        public DynamicHidenAttribute(string corpTypeProperty = "")
        {
            CorpTypeProperty = corpTypeProperty;
        }

        public string CorpTypeProperty { get; private set; }
    }
}