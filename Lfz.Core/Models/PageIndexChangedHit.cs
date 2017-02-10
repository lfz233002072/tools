namespace Lfz.Models
{
    /// <summary>
    /// 页面信息修改
    /// </summary>
    public class PageIndexChangedHit : IViewStateSearchHit
    {
        public int PageIndex { get; set; }
        public string SearchHitViewState { get; set; }
    }
}