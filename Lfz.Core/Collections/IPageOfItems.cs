//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : CacheHelper
//        DESCRIPTION : 缓存辅助类
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System.Collections.Generic;

namespace Lfz.Collections
{
    /// <summary>
    /// 分页数据列表
    /// </summary>
    /// <typeparam name="T"></typeparam> 
    public interface IPageOfItems<T> : IList<T>, IPagerInfo
    {
        /// <summary>
        /// 查询页面状态数据
        /// TODO 需要从但前页面移除
        /// </summary>
        string SearchHitViewState { get; set; }
    }
}
