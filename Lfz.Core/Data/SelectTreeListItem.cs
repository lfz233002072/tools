namespace Lfz.Data
{
    public class SelectTreeListItem
    {
        public string Id { get; set; }
        public string VisitPath { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
        public object OptionsHtmlAttributes { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class ComboxListItem : System.Object
    {
        private string _Value = string.Empty;
        private string _Text = string.Empty;

        ///
        /// 值
        ///
        public string Value
        {
            get { return this._Value; }
            set { this._Value = value; }
        }
        ///
        /// 显示的文本
        ///
        public string Text
        {
            get { return this._Text; }
            set { this._Text = value; }
        }

        public ComboxListItem(string value, string text)
        {
            this._Value = value;
            this._Text = text;
        }
        public override string ToString()
        {
            return this._Text;
        }

    }
}