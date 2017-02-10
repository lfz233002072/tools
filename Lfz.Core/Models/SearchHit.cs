namespace Lfz.Models
{
    public class SearchHit : SearchHitBase
    {
        private string _searchKey;
        public string SearchKey
        {
            get
            {
                _searchKey = (_searchKey ?? string.Empty).Trim();
                if (_searchKey == "请输入关键字") _searchKey = string.Empty;
                return _searchKey;
            }
            set { _searchKey = value; }
        }
    }
}
