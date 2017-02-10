using System;
using System.Data;

namespace Lfz.Collections
{
    /// <summary>
    /// 分页数据列表
    /// </summary> 
    public class PageOfDatatable : IPageOfDatatable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public PageOfDatatable(DataTable items)
        {
            Data = items;
        }
        public PageOfDatatable()
        {
        }


        /// <summary>
        /// 查询页面状态数据
        /// TODO 需要从但前页面移除
        /// </summary>
        public string SearchHitViewState { get; set; }
        #region IPageOfItems<T> Members

        private int _pageNumber;

        /// <summary> 
        /// 索引从0开始
        /// </summary> 
        public int PageIndex
        {
            get
            {
                return _pageNumber >= 0 ? _pageNumber : 0;
            }
            set { _pageNumber = value; }
        }
        private int _pageSize;

        /// <summary>
        /// 页面大小
        /// </summary> 
        public int PageSize
        {
            get
            {
                return _pageSize > 0 ? _pageSize : 10;
            }
            set { _pageSize = value; }
        }

        private int _totalItemCount;

        /// <summary>
        /// 大于0的数字
        /// </summary> 
        public int TotalItemCount
        {
            get
            {
                return _totalItemCount >= 0 ? _totalItemCount : 0;
            }
            set { _totalItemCount = value; }
        }

        /// <summary>
        /// 最少有一页，哪怕是没有记录，也会显示一页
        /// </summary> 
        public int TotalPageCount
        {
            get { return Math.Max(1, (int)Math.Ceiling((double)TotalItemCount / PageSize)); }
        }

        /// <summary>
        /// 从0开始计数
        /// </summary> 
        public int StartIndex
        {
            get { return PageIndex * PageSize; }
        }

        /// <summary>
        /// 取值范围从0到TotalItemCount-1
        /// </summary> 
        public int EndIndex
        {
            get
            {
                var endNumber = PageSize * (PageIndex + 1);
                if (endNumber > TotalItemCount) endNumber = TotalItemCount;
                return

                    Math.Max(0, endNumber - 1);
            }
        }

        #endregion

        public DataTable Data
        {
            get;
            set;
        }
    }
}