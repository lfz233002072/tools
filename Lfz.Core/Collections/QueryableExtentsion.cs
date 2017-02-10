using System;
using System.Collections.Generic;
using System.Linq;

namespace Lfz.Collections
{
    /// <summary>
    /// IQueryable列表扩展方法
    /// </summary>
    public static class QueryableExtentsion
    {
        /// <summary>
        /// 获取分页列表，queryable从0开始计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IPageOfItems<T> GetPagedFromQueryable<T>(this IQueryable<T> queryable, int totalCount, int pageIndex, int pageSize)
        {
            //初始化页面信息
            var pageOfItems = new PageOfItems<T>
                                  {
                                      PageIndex = pageIndex,
                                      PageSize = pageSize,
                                      TotalItemCount = totalCount
                                  };
            pageOfItems.AddRange(queryable.Skip(pageOfItems.StartIndex).Take(pageOfItems.PageSize).ToReadOnlyCollection().ToList());
            return pageOfItems;
        }

        /// <summary>
        /// 获取分页列表，IEnumerable从0开始计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IPageOfItems<T> GetPaged<T>(this IEnumerable<T> queryable, int totalCount, int pageIndex, int pageSize)
        {
            //初始化页面信息
            var pageOfItems = new PageOfItems<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemCount = totalCount
            };
            pageOfItems.AddRange((queryable.Skip(pageOfItems.StartIndex).Take(pageOfItems.PageSize).ToReadOnlyCollection()).ToList());
            return pageOfItems;
        }

        /// <summary>
        /// 将类型T的分页数据列表转换为类型TView的分页数据列表
        /// </summary>
        /// <typeparam name="T">原始数据类型</typeparam>
        /// <typeparam name="TView">返回的数据类型</typeparam>
        /// <param name="items">数据列表</param>
        /// <param name="selector">数据转换选择器</param>
        /// <returns>返回TView的分页数据列表</returns>
        public static IPageOfItems<TView> ToView<T, TView>(this IPageOfItems<T> items, Func<T, TView> selector)
        {
            //初始化页面信息
            var pageOfItems = new PageOfItems<TView>
            {
                PageIndex = items.PageIndex,
                PageSize = items.PageSize,
                TotalItemCount = items.TotalItemCount,
            };
            var list = items.Select(selector);
            pageOfItems.AddRange(list.ToList());
            return pageOfItems;
        }
         
    }
}