// ======================================================================
// 	
// 	Copyright (C)  1996-2012  lfz    
// 	All rights reserved
// 	
// 	Filename :DemoController.cs
// 	DESCRIPTION :
// 	
// 	Created By Administrator  at 2014-04-30 14:40
// 	https://git.oschina.net/lfz/tools
// 
// ======================================================================*/
namespace Lfz.Models
{
    /// <summary>
    /// 属性修改跟踪信息
    /// </summary>
    public class PropertyTrackInfo
    { 

        /// <summary>
        /// 属性跟踪状态
        /// </summary>
        public PropertyTrackStatus Status { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 修改前值
        /// </summary>
        public string OldValue { get; set; }

        /// <summary>
        /// 修改后值
        /// </summary>
        public string NewValue { get; set; }
    }
}