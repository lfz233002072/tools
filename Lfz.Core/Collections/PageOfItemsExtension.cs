using System.Linq;

namespace Lfz.Collections
{
    public static class PageOfItemsExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageOfItems"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IPageOfItems<object> ToObjectsPageOfItems<T>(this IPageOfItems<T> pageOfItems)
        {
            var result = new PageOfItems<object>(pageOfItems.Select(x => x as object))
            {
                PageIndex = pageOfItems.PageIndex,
                PageSize = pageOfItems.PageSize,
                TotalItemCount = pageOfItems.TotalItemCount,
                SearchHitViewState = pageOfItems.SearchHitViewState
            };
            return result;
        }
    }
}