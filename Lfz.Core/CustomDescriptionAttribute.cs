/*======================================================================
 *
 *        Copyright (C)  1996-2012  lfz    
 *        All rights reserved
 *
 *        Filename :DescriptionAttribute.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-05-16 14:11
 *        https://git.oschina.net/lfz/tools
 *
 *======================================================================*/

using System;

namespace Lfz
{ 

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class CustomTableNameAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        public CustomTableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class CustomDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public CustomDescriptionAttribute(string description)
        {
            Description = description;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
    }

     
}