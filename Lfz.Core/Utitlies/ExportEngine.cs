/************************************************************************ 
 * uoLib library for .Net projects.
 * Copyright (c) 2008-2010 by uonun
 * Homepage: http://udnz.com/Works/uolib
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see http://www.gnu.org/licenses/gpl.html.
 ***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lfz.Utitlies
{
    /// <summary>
    /// 数据导出引擎，支持智能导出，自动绑定数据列，实现一行代码即可导出，支持样式设置。
    /// </summary>
    /// <typeparam name="T">数据实例的类型</typeparam>
    /// <remarks>
    /// <para>此类专用于数据导出。 它可以将数据对象导出到Excel中。提供如下功能支持：</para>
    /// <ul>
    /// <li>广泛支持多种数据源，支持智能导出，自动绑定数据列，实现一行代码即可导出</li>
    /// <li>支持手动指定数据列</li>
    /// <li>支持自定义处理程序，让数据导出更智能</li>
    /// <li>支持文档标题、文档备注</li>
    /// <li>支持自定义设置Excel表格的样式</li>
    /// <li>支持.NET Framework 2.0 及其以上</li>
    /// <li>支持导出为Excel、网页</li>
    /// </ul>
    /// <br />
    /// <para>它广泛支持多种数据源，<strong>您可以绑定任意集合</strong>：</para>
    /// <ul>
    /// <li>DataSet</li>
    /// <li>DataTable</li>
    /// <li>List</li>
    /// <li>List&lt;T&gt;（T可以是任何自定义实例类）</li>
    /// <li>Collection</li>
    /// <li>Collection&lt;T&gt;（T可以是任何自定义实例类）</li>
    /// <li>……任何实现了IListSource、IEnumerable 或 IDataSource中任意一个接口的对象！</li>
    /// <li>简言之，<strong>任何可用绑定到GridView的数据源都可以直接绑定导出</strong>！</li>
    /// </ul>
    /// <para>
    /// 详细使用说明请见Demo程序：<a href="http://udnz.com/Works/Demo/ExportEngine/" target="_blank">http://udnz.com/Works/Demo/ExportEngine/</a>
    /// </para>
    /// </remarks> 
    public class ExportEngine<T>
    {
        private Stopwatch _watch;

        /// <summary>
        /// 导出参数。可设置文件名、标题文字、表格样式、备注信息等
        /// </summary>
        public Options Options { get { return _opt; } set { _opt = value; } }
        private Options _opt = new Options();

        /// <summary>
        /// 要导出的数据列。不指定则自动绑定数据源的所有列
        /// </summary>
        public List<DataColumn<T>> Columns { get { return _columns; } set { _columns = value; } }
        private List<DataColumn<T>> _columns = new List<DataColumn<T>>();

        /// <summary>
        /// 添加要导出的要导出的数据列
        /// </summary>
        /// <param name="dataField">获取或设置要绑定的数据字段的名称。可以是对象属性、字段、DataRow名称等，使用方法同 BoundField.DataField。</param>
        /// <param name="headerText"> 获取或设置显示在数据列标头中的文本，使用方法同 BoundField.HeaderText。</param>
        /// <param name="dataFormatter">获取或设置数据处理程序。用于绑定每行时定制化输出该列的文本。导出自动绑定列时无效。</param>
        public void AddColumns(string dataField, string headerText, DataFormatter<T> dataFormatter)
        {
            this.AddColumns(dataField, headerText, string.Empty, dataFormatter);
        }

        /// <summary>
        /// 添加要导出的要导出的数据列
        /// </summary>
        /// <param name="dataField">获取或设置要绑定的数据字段的名称。可以是对象属性、字段、DataRow名称等，使用方法同 BoundField.DataField。</param>
        /// <param name="headerText"> 获取或设置显示在数据列标头中的文本，使用方法同 BoundField.HeaderText。</param>
        /// <param name="dataFormatString">获取或设置字符串，该字符串指定字段值的显示格式。默认值为空字符串 ("")，表示尚无特殊格式设置应用于该字段值。</param>
        /// <param name="dataFormatter">获取或设置数据处理程序。用于绑定每行时定制化输出该列的文本。导出自动绑定列时无效。</param>
        /// <param name="mHtmlEncode"> 获取或设置一个值，该值指示在显示字段值之前，是否对这些字段值进行 HTML 编码。默认为 true。</param>
        /// <param name="mHtmlEncodeFormatString">获取或设置一个值，该值指示格式化的文本在显示时是否应经过 HTML 编码。默认为 true。</param>
        public void AddColumns(string dataField, string headerText, string dataFormatString = "", DataFormatter<T> dataFormatter = null, bool mHtmlEncode = true, bool mHtmlEncodeFormatString = true)
        {
            this.Columns.Add(new DataColumn<T>()
            {
                DataField = dataField,
                HeaderText = headerText,
                DataFormatString = dataFormatString,
                DataFormater = dataFormatter,
                HtmlEncode = mHtmlEncode,
                HtmlEncodeFormatString = mHtmlEncodeFormatString
            });
        }

        /// <summary>
        /// 获取或设置输出流的 HTTP 字符集。具体说明同 <see cref="System.Web.HttpResponse.Charset"/>。此处默认为 UTF-8。
        /// </summary>
        public string Charset { get { return _charset; } set { _charset = value; } }
        private string _charset = "UTF-8";

        /// <summary>
        /// 获取或设置输出流的 HTTP 字符集。具体说明同 <see cref="System.Web.HttpResponse.ContentEncoding"/>，此处默认为 Encoding.UTF8。
        /// </summary>
        public Encoding ContentEncoding { get { return _contentEncoding; } set { _contentEncoding = value; } }
        private Encoding _contentEncoding = Encoding.UTF8;

        /// <summary>
        /// 获取或设置导出文件的类型。默认为Excel 97-2003
        /// </summary>
        public FileType FileType { get { return _fileType; } set { _fileType = value; } }
        private FileType _fileType = FileType.Excel;

        /// <summary>
        /// 初始化<see cref="/>类的新实例
        /// </summary>
        public ExportEngine() { }

        /// <summary>
        /// 初始化<see cref="/>类的新实例
        /// </summary>
        /// <param name="columns">要导出的数据列。不指定则自动绑定数据源的所有列</param>
        public ExportEngine(List<DataColumn<T>> columns)
        {
            this.Columns = columns;
        }

        /// <summary>
        /// 初始化<see cref="/>类的新实例
        /// </summary>
        /// <param name="columns">要导出的数据列。不指定则自动绑定数据源的所有列</param>
        /// <param name="options">导出参数。可设置文件名、标题文字、表格样式、备注信息等</param>
        public ExportEngine(List<DataColumn<T>> columns, Options options)
        {
            this.Columns = columns;
            this.Options = options;
        }

        /// <summary>
        /// 执行导出任务
        /// </summary>
        /// <param name="data">要导出的数据</param>
        public void ExportToExcel(object data)
        {
            ExportToExcel(GetGdv(data));
        }

        private GridView GetGdv(object data)
        {
            if (data == null) throw new ArgumentNullException("data", "没有输入有效数据");
            if (!(data is IListSource || data is IEnumerable || data is IDataSource))
                throw new ArgumentException("data", "数据源的类型无效。它必须是 IListSource、IEnumerable 或 IDataSource！");

            _watch = new Stopwatch();
            _watch.Start();

            GridView gvw = new GridView();
            gvw.RowCreated += gvw_RowCreated;
            if (Options == null) Options = new Options();

            //设置样式
            SetStype(gvw);

            //已设置字段和列头，则只导出指定的字段
            if (this.Columns != null && this.Columns.Count > 0)
            {
                gvw.AutoGenerateColumns = false;
                int columnCount = this.Columns.Count;
                for (int i = 0; i < columnCount; i++)
                {
                    if (this.Columns[i] == null) continue;

                    BoundField bf = new BoundField();
                    bf.DataField = this.Columns[i].DataField;
                    bf.HeaderText = this.Columns[i].HeaderText;
                    bf.DataFormatString = this.Columns[i].DataFormatString;
                    bf.HtmlEncode = this.Columns[i].HtmlEncode;
                    bf.HtmlEncodeFormatString = this.Columns[i].HtmlEncodeFormatString;
                    gvw.Columns.Add(bf);
                }
            }
            else
            {
                gvw.AutoGenerateColumns = true;
            }

            gvw.DataSource = data;
            // 若手动指定了导出列，则进行数据处理
            if (this.Columns != null)
            {
                gvw.RowDataBound += new GridViewRowEventHandler(gvw_RowDataBound);
            }
            gvw.DataBind();
            return gvw;
        }

        public byte[] ExportToBytes(object data)
        {
            var gdv = GetGdv(data);
            return ExportToBytes(gdv);
        }

        /// <summary>
        /// 导出GridView中的数据到Excel
        /// </summary>
        /// <param name="gvw"></param>
        private byte[] ExportToBytes(GridView gvw)
        {

            var sbHtml = new StringBuilder();
            sbHtml.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n");
            sbHtml.Append(string.Format("<html>\n<head>\n<title>{0}</title>\n", this.Options.TitleText));
            sbHtml.Append(string.Format("<meta http-equiv=\"Content-Type\" content=\"text/html; charset={0}\">\n", this.Charset));
            sbHtml.Append("<style type=\"text/css\" title=\"\">\n<!--\n");
            sbHtml.Append("body {margin:0;padding:0;color:#000;background-color:#FFF;}\n");
            sbHtml.Append("body,p {font-size:12px;line-height:150%;font-family:Arial, Helvetica, sans-serif;}\n");
            sbHtml.Append("//-->\n</style>\n</head>\n");
            sbHtml.Append("<body>\n");

            System.Globalization.CultureInfo m_CI = new System.Globalization.CultureInfo("ZH-CN", true);//区域设置 
            StringWriter tw = new System.IO.StringWriter(m_CI);
            HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
            gvw.RenderControl(hw);

            int spancol = this.Columns.Count;
            if (gvw.HeaderRow != null) spancol = gvw.HeaderRow.Cells.Count;
            if (!string.IsNullOrEmpty(Options.TitleText))
            {
                string css = " style=\"{0}\" ";
                if (Options.TitleIsBold) css = string.Format(css, "font-weight:bolder;{0}");
                if (Options.TitleIsItalic) css = string.Format(css, "font-style:italic;{0}");

                if (!string.IsNullOrEmpty(Options.TitleFontName))
                    css = string.Format(css, "font-family:" + Options.TitleFontName + ";{0}");
                else
                    css = string.Format(css, "font-family:Arial, Helvetica, sans-serif;{0}");

                if (!Options.TitleFontSize.IsEmpty) css = string.Format(css, "font-size:" + Options.TitleFontSize.ToString() + ";{0}");
                if (!Options.TitleBackColor.IsEmpty) css = string.Format(css, "background-color:" + Options.TitleBackColor.Name + ";{0}");
                if (!Options.TitleForeColor.IsEmpty) css = string.Format(css, "color:" + Options.TitleForeColor.Name + ";{0}");
                switch (Options.TitleHorizontalAlign)
                {
                    case HorizontalAlign.Center:
                        css = string.Format(css, "text-align:center;{0}");
                        break;
                    case HorizontalAlign.Justify:
                        css = string.Format(css, "text-align:justify;{0}");
                        break;
                    case HorizontalAlign.Right:
                        css = string.Format(css, "text-align:right;{0}");
                        break;
                    case HorizontalAlign.Left:
                    case HorizontalAlign.NotSet:
                    default:
                        break;
                }
                if (!Options.TitleHeight.IsEmpty) css = string.Format(css, "height:" + Options.TitleHeight.Value.ToString("f0") + "px;{0}");
                css = css.Replace("{0}", "");
                sbHtml.Append(string.Format("<table><tr><td {0} colspan=\"{2}\">{1}</td></tr></table>", css, Utils.EncodeHtml(Options.TitleText), spancol));
            }
            if (!string.IsNullOrEmpty(Options.TitleInfoHtml))
            {
                sbHtml.Append(string.Format("<table><tr><td  colspan=\"{0}\">{1}</td></tr></table>", spancol, Options.TitleInfoHtml));
            }

            sbHtml.Append(tw.ToString());

            if (!string.IsNullOrWhiteSpace(Options.InfoHtml))
                sbHtml.Append(string.Format("<table><tr><td colspan=\"{1}\"></td></tr><tr><td colspan=\"{1}\">{0}</td></tr><tr><td colspan=\"{1}\"></td></tr></table>", Options.InfoHtml, spancol));

            _watch.Stop();

            sbHtml.Append(string.Format("<div align=\"right\" style=\"color:#AAAAAA;font-family:Verdana\">导出时间:{0}, 记录数量(s): {2}, 耗时: {1}ms</div>", DateTime.Now, _watch.ElapsedMilliseconds, gvw.Rows.Count));
            sbHtml.Append("\n</body>\n</html>");
            return this.ContentEncoding.GetBytes(sbHtml.ToString());
        }

        private void gvw_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // 检查该行的每一列，如果设置了该列的数据处理程序，则对该列进行处理
                for (int n = 0; n < this.Columns.Count; n++)
                {
                    if (this.Columns[n].DataFormater != null)
                    {
                        // 调用外部的自定义处理程序的耗时不纳入计算范围
                        _watch.Stop();
                        e.Row.Cells[n].Text = this.Columns[n].DataFormater((T)e.Row.DataItem);
                        _watch.Start();
                    }
                }
            }
        }

        //设计表头
        protected void gvw_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header && Options.TableHeaderCell != null)
            {
                TableCellCollection header = e.Row.Cells;
                header.Clear();
                header.Add(Options.TableHeaderCell);
            }
        }

        /// <summary>
        /// 设置样式
        /// </summary>
        /// <param name="gvw"></param>
        private void SetStype(GridView gvw)
        {
            gvw.BorderStyle = System.Web.UI.WebControls.BorderStyle.Solid;

            gvw.HeaderStyle.Font.Bold = Options.HeaderIsBold;
            gvw.HeaderStyle.Font.Italic = Options.HeaderIsItalic;
            gvw.HeaderStyle.Font.Name = Options.HeaderFontName;
            gvw.HeaderStyle.Font.Size = Options.HeaderFontSize;
            gvw.HeaderStyle.Wrap = Options.HeaderStyle.Wrap;
            gvw.HeaderStyle.ForeColor = Options.HeaderStyle.ForeColor;
            gvw.HeaderStyle.BackColor = Options.HeaderStyle.BackColor;
            gvw.HeaderStyle.HorizontalAlign = Options.HeaderStyle.HorizontalAlign;
            gvw.HeaderStyle.Height = Options.HeaderStyle.Height;
            gvw.ShowHeaderWhenEmpty = true;

            gvw.RowStyle.Font.Name = Options.RowFontName;
            gvw.RowStyle.Font.Size = Options.RowFontSize;
            gvw.RowStyle.Wrap = Options.RowStyle.Wrap;
            gvw.RowStyle.ForeColor = Options.RowStyle.ForeColor;
            gvw.RowStyle.BackColor = Options.RowStyle.BackColor;
            gvw.RowStyle.HorizontalAlign = Options.RowStyle.HorizontalAlign;
            gvw.RowStyle.Height = Options.RowStyle.Height;

            gvw.AlternatingRowStyle.Font.Name = Options.AlternatingRowFontName;
            gvw.AlternatingRowStyle.Font.Size = Options.AlternatingRowFontSize;
            gvw.AlternatingRowStyle.Wrap = Options.AlternatingRowStyle.Wrap;
            gvw.AlternatingRowStyle.ForeColor = Options.AlternatingRowStyle.ForeColor;
            gvw.AlternatingRowStyle.BackColor = Options.AlternatingRowStyle.BackColor;
            gvw.AlternatingRowStyle.HorizontalAlign = Options.AlternatingRowStyle.HorizontalAlign;
            gvw.AlternatingRowStyle.Height = Options.AlternatingRowStyle.Height;

            gvw.EmptyDataText = Options.EmptyDataText;
            gvw.EmptyDataRowStyle.HorizontalAlign = HorizontalAlign.Center;
            gvw.EmptyDataRowStyle.ForeColor = Color.Red;
        }


        /// <summary>
        /// 初始化响应头信息，并返回文件名
        /// </summary>
        /// <returns></returns>
        public string InitFileName()
        {
            string fileName;
            #region 准备文件名
            if (!string.IsNullOrWhiteSpace(Options.FileName))
                fileName = Options.FileName;
            else
                fileName = string.Format("Exported File {0:yyyy-MM-dd_HH_mm}", DateTime.Now);

            if (fileName.Contains(".")) { fileName = fileName.Substring(0, fileName.LastIndexOf(".")); }

            fileName = (string.Equals(HttpContext.Current.Request.Browser.Browser, "IE"))
                           ? HttpUtility.UrlEncode(fileName, this.ContentEncoding)
                           : fileName;

            switch (this._fileType)
            {
                case FileType.Htm:
                    fileName += ".htm";
                    break;
                case FileType.Html:
                    fileName += ".html";
                    break;
                default:
                    fileName += ".xls";
                    break;
            }
            #endregion

            return fileName;
        }

        /// <summary>
        /// 导出GridView中的数据到Excel
        /// </summary>
        /// <param name="gvw"></param>
        private void ExportToExcel(GridView gvw)
        {
            var fileName = InitFileName();

            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.Charset = this.Charset;
            HttpContext.Current.Response.ContentEncoding = this.ContentEncoding;
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.HeaderEncoding = this.ContentEncoding;  
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
            HttpContext.Current.Response.AppendHeader("Content-Type", string.Format("application/vnd.ms-excel;charset={0};", this.Charset));
            HttpContext.Current.Response.AppendHeader("Content-Language", "zh-CN");
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";

            HttpContext.Current.Response.Write("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n");
            HttpContext.Current.Response.Write(string.Format("<html>\n<head>\n<title>{0}</title>\n", this.Options.TitleText));
            HttpContext.Current.Response.Write(string.Format("<meta http-equiv=\"Content-Type\" content=\"text/html; charset={0}\">\n", this.Charset));
            HttpContext.Current.Response.Write("<style type=\"text/css\" title=\"\">\n<!--\n");
            HttpContext.Current.Response.Write("body {margin:0;padding:0;color:#000;background-color:#FFF;}\n");
            HttpContext.Current.Response.Write("body,p {font-size:12px;line-height:150%;font-family:Arial, Helvetica, sans-serif;}\n");
            HttpContext.Current.Response.Write("//-->\n</style>\n</head>\n");
            HttpContext.Current.Response.Write("<body>\n");

            System.Globalization.CultureInfo m_CI = new System.Globalization.CultureInfo("ZH-CN", true);//区域设置 
            StringWriter tw = new System.IO.StringWriter(m_CI);
            HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
            gvw.RenderControl(hw);

            int spancol = this.Columns.Count;
            if (gvw.HeaderRow != null) spancol = gvw.HeaderRow.Cells.Count;
            if (!string.IsNullOrEmpty(Options.TitleText))
            {
                string css = " style=\"{0}\" ";
                if (Options.TitleIsBold) css = string.Format(css, "font-weight:bolder;{0}");
                if (Options.TitleIsItalic) css = string.Format(css, "font-style:italic;{0}");

                if (!string.IsNullOrEmpty(Options.TitleFontName))
                    css = string.Format(css, "font-family:" + Options.TitleFontName + ";{0}");
                else
                    css = string.Format(css, "font-family:Arial, Helvetica, sans-serif;{0}");

                if (!Options.TitleFontSize.IsEmpty) css = string.Format(css, "font-size:" + Options.TitleFontSize.ToString() + ";{0}");
                if (!Options.TitleBackColor.IsEmpty) css = string.Format(css, "background-color:" + Options.TitleBackColor.Name + ";{0}");
                if (!Options.TitleForeColor.IsEmpty) css = string.Format(css, "color:" + Options.TitleForeColor.Name + ";{0}");
                switch (Options.TitleHorizontalAlign)
                {
                    case HorizontalAlign.Center:
                        css = string.Format(css, "text-align:center;{0}");
                        break;
                    case HorizontalAlign.Justify:
                        css = string.Format(css, "text-align:justify;{0}");
                        break;
                    case HorizontalAlign.Right:
                        css = string.Format(css, "text-align:right;{0}");
                        break;
                    case HorizontalAlign.Left:
                    case HorizontalAlign.NotSet:
                    default:
                        break;
                }
                if (!Options.TitleHeight.IsEmpty) css = string.Format(css, "height:" + Options.TitleHeight.Value.ToString("f0") + "px;{0}");
                css = css.Replace("{0}", "");
                HttpContext.Current.Response.Write(string.Format("<table><tr><td {0} colspan=\"{2}\">{1}</td></tr></table>", css, Utils.EncodeHtml(Options.TitleText), spancol));
            }
            if (!string.IsNullOrEmpty(Options.TitleInfoHtml))
            {
                HttpContext.Current.Response.Write(string.Format("<table><tr><td  colspan=\"{0}\">{1}</td></tr></table>", spancol, Options.TitleInfoHtml));
            }

            HttpContext.Current.Response.Write(tw.ToString());

            if (!string.IsNullOrWhiteSpace(Options.InfoHtml))
                HttpContext.Current.Response.Write(string.Format("<table><tr><td colspan=\"{1}\"></td></tr><tr><td colspan=\"{1}\">{0}</td></tr><tr><td colspan=\"{1}\"></td></tr></table>", Options.InfoHtml, spancol));

            _watch.Stop();

            HttpContext.Current.Response.Write(string.Format("<div align=\"right\" style=\"color:#AAAAAA;font-family:Verdana\">导出时间:{0}, 记录数量(s): {2}, 耗时: {1}ms</div>", DateTime.Now, _watch.ElapsedMilliseconds, gvw.Rows.Count));
            HttpContext.Current.Response.Write("\n</body>\n</html>");
            //HttpContext.Current.Response.Flush(); 
            HttpContext.Current.Response.End();

            gvw.Dispose();
            tw.Dispose();
            hw.Dispose();

            gvw = null;
            tw = null;
            hw = null;
        }
    }

    /// <summary>
    /// 数据处理程序。用于绑定每行时定制化输出该列的文本
    /// </summary>
    /// <typeparam name="T">各行的数据类型</typeparam>
    /// <param name="columnData">当前行的数据</param>
    /// <returns></returns>
    public delegate string DataFormatter<T>(T columnData);

    /// <summary>
    /// 要导出的数据列信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataColumn<T>
    {
        private bool mHtmlEncode = true;
        private bool mHtmlEncodeFormatString = true;

        /// <summary>
        /// 获取或设置要绑定的数据字段的名称。可以是对象属性、字段、DataRow名称等，使用方法同 BoundField.DataField。
        /// </summary>
        public string DataField { get; set; }
        /// <summary>
        /// 获取或设置显示在数据列标头中的文本，使用方法同 BoundField.HeaderText。
        /// </summary>
        public string HeaderText { get; set; }
        /// <summary>
        /// 获取或设置数据处理程序。用于绑定每行时定制化输出该列的文本。导出自动绑定列时无效。
        /// </summary>
        public DataFormatter<T> DataFormater { get; set; }
        /// <summary>
        /// 获取或设置字符串，该字符串指定字段值的显示格式。默认值为空字符串 ("")，表示尚无特殊格式设置应用于该字段值。<see cref=""/>为null时有效。
        /// </summary>
        public string DataFormatString { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示在显示字段值之前，是否对这些字段值进行 HTML 编码。默认为 true。
        /// </summary>
        public bool HtmlEncode { get { return this.mHtmlEncode; } set { this.mHtmlEncode = value; } }
        /// <summary>
        /// 获取或设置一个值，该值指示格式化的文本在显示时是否应经过 HTML 编码。默认为 true。
        /// </summary>
        public bool HtmlEncodeFormatString { get { return this.mHtmlEncodeFormatString; } set { this.mHtmlEncodeFormatString = value; } }
    }

    /// <summary>
    /// 导出时使用的参数设置
    /// </summary>
    public class Options
    {
        /// <summary>
        /// 导出文件名，不设置则系统自动生成
        /// </summary>
        public string FileName;


        /// <summary>
        /// 导出文件备注信息，将跟随在导出表格之后，支持Html代码
        /// </summary>
        public string InfoHtml;

        /// <summary>
        /// 导出二级标题Html代码
        /// </summary>
        public string TitleInfoHtml;

        private string mEmptyDataText = "无数据...";

        public string EmptyDataText
        {
            get { if (string.IsNullOrEmpty(mEmptyDataText)) mEmptyDataText = "无数据..."; return mEmptyDataText; }
            set { mEmptyDataText = value; }
        }
        /// <summary>
        /// 导出数据表格的标题文字
        /// </summary>
        public string TitleText;

        /// <summary>
        /// 标题文字是否加粗，默认加粗
        /// </summary>
        public bool TitleIsBold = true;

        /// <summary>
        /// 标题文字是否斜体，默认不使用斜体
        /// </summary>
        public bool TitleIsItalic = false;

        /// <summary>
        /// 标题文字使用的字体，默认为微软雅黑
        /// </summary>
        public string TitleFontName = "微软雅黑";

        /// <summary>
        /// 标题文字大小，默认为 18
        /// </summary>
        public FontUnit TitleFontSize = 18;

        /// <summary>
        /// 标题文字颜色，默认黑色
        /// </summary>
        public Color TitleForeColor = Color.Black;

        /// <summary>
        /// 标题文字背景色，默认不设置
        /// </summary>
        public Color TitleBackColor = Color.Empty;

        /// <summary>
        /// 标题文字横向位置，默认居中
        /// </summary>
        public HorizontalAlign TitleHorizontalAlign = HorizontalAlign.Center;

        /// <summary>
        /// 标题高度，默认 50 像素
        /// </summary>
        public Unit TitleHeight = 50;

        /// <summary>
        /// 列标题是否加粗，默认加粗
        /// </summary>
        public bool HeaderIsBold = true;

        /// <summary>
        /// 列标题是否斜体，默认不使用斜体
        /// </summary>
        public bool HeaderIsItalic = false;

        /// <summary>
        /// 列标题默认字体，默认为 Verdana
        /// </summary>
        public string HeaderFontName = "Verdana";

        /// <summary>
        /// 列标题文字大小，默认为 10
        /// </summary>
        public FontUnit HeaderFontSize = 10;

        /// <summary>
        /// 列标题样式。仅Wrap/BackColor/ForeColor/HorizontalAlign/Height有效。
        /// </summary>
        public TableItemStyle HeaderStyle = new TableItemStyle()
        {
            Wrap = false,
            BackColor = Color.FromArgb(0, 112, 192),
            ForeColor = Color.White,
            HorizontalAlign = HorizontalAlign.Center,
            Height = 22,
        };

        /// <summary>
        /// 数据行字体，默认为 Verdana
        /// </summary>
        public string RowFontName = "Verdana";

        /// <summary>
        /// 数据行文字大小，默认为 9
        /// </summary>
        public FontUnit RowFontSize = 9;

        /// <summary>
        /// 数据行文字样式。仅Wrap/BackColor/ForeColor/HorizontalAlign/Height有效。
        /// </summary>
        public TableItemStyle RowStyle = new TableItemStyle()
        {
            Wrap = true,
            Height = 22,
        };

        public TableHeaderCell TableHeaderCell { get; set; }

        /// <summary>
        /// 交替行字体，默认为 Verdana
        /// </summary>
        public string AlternatingRowFontName = "Verdana";

        /// <summary>
        /// 交替行文字大小，默认为 9
        /// </summary>
        public FontUnit AlternatingRowFontSize = 9;

        /// <summary>
        /// 交替行文字样式。仅Wrap/BackColor/ForeColor/HorizontalAlign/Height有效。
        /// </summary>
        public TableItemStyle AlternatingRowStyle = new TableItemStyle()
        {
            Wrap = true,
            Height = 22,
            BackColor = Color.FromArgb(240, 240, 240)
        };
    }

    /// <summary>
    /// 导出文件类型
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// Excel 97-2003
        /// </summary>
        Excel,
        /// <summary>
        /// Htm 网页
        /// </summary>
        Htm,
        /// <summary>
        /// Html 网页
        /// </summary>
        Html
    }
}
