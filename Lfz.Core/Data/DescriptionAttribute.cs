/*======================================================================
 *
 *        Copyright (C)  1996-2012  杭州品茗信息有限公司    
 *        All rights reserved
 *
 *        Filename :DescriptionAttribute.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-05-16 14:11
 *        http://www.pinming.cn/
 *
 *======================================================================*/

using System;

namespace PMSoft.Data
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string description)
        {
            Description = description;
        } 
        public string Description { get; set; }
    }

    
}