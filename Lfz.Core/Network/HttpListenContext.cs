/*
 * HTTP请求保护的基本信息(其中Method可能为GET POST)
 * 
 * Method / HTTP/1.1\r\n
 * Host: atasoyweb.net\r\n
 * User-Agent: Mozilla/5.0 (Windows NT 6.1; rv:14.0) Gecko/20100101 Firefox/14.0.1\r\n
 * Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/
/*;q=0.8\r\n
* Accept-Language: tr-tr,tr;q=0.8,en-us;q=0.5,en;q=0.3\r\n
* Accept-Encoding: gzip, deflate\r\n
* Connection: keep-alive\r\n\r\n
* 
*/

using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using Lfz.Utitlies;

namespace Lfz.Network
{
    /// <summary>
    /// HTTP监听上下文
    /// </summary>
    public class HttpListenContext
    {
        /// <summary>
        /// HTTP监听上下文
        /// </summary>
        /// <param name="recContent"></param>
        public HttpListenContext(string recContent)
        { 
            IsValidContext = false;
            Headers = new NameValueCollection();
            QueryString = new NameValueCollection();
            Forms = new NameValueCollection();
            string[] contentAttr = Regex.Split(recContent, "\r\n\r\n");
            //内容是否包含Http头部.Http头部和Http内容是用"\r\n\r\n"分割
            if (contentAttr.Length > 0)
            {
                string headerContent = contentAttr[0];
                //HTTP 头部每条记录是用"\r\n"分割
                string[] headerArr = Regex.Split(headerContent, "\r\n");

                #region 解析协议头部信息

                if (headerArr.Length > 0)
                {
                    //第一行为协议、方法、地址等信息，其形如Method / HTTP/1.1\r\n
                    var codeList = headerArr[0].Split(' ');
                    var method = codeList[0];
                    if (string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(method, "POST", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(method, "DELETE", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(method, "PUT", StringComparison.OrdinalIgnoreCase))
                    {
                        IsValidContext = true;
                        HttpMethod = codeList[0];
                    }
                    if (codeList.Length > 1) Path = codeList[1];
                    if (codeList.Length > 2) Protocol = codeList[2];
                }

                #endregion

                for (int i = 1; i < headerArr.Length; i++)
                {
                    var codeList = headerArr[i].Split(':');
                    //有效的Http头部信息
                    if (codeList.Length > 0) Headers.Add(codeList[0], codeList[1]);
                }

                ContentLength = TypeParse.StrToInt(Headers["Content-Length"]);
                ContentType = Headers["Content-Type"];
                //具有有效内容
                if (ContentLength > 0 && recContent.Length > headerContent.Length + 4)
                {
                    //剔除Http头部信息外全部为Http内容
                    ContentBody = recContent.Substring(headerContent.Length + 4);
                }
                else ContentLength = 0;//内容无效
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receBuf"></param>
        /// <param name="encoding"></param>
        public HttpListenContext(byte[] receBuf, Encoding encoding)
            : this(encoding.GetString(receBuf))
        {

        }


        /// <summary>
        /// 是否为有效上下文
        /// </summary>
        public bool IsValidContext { get; private set; }

        /// <summary>
        /// 当前请求的虚拟路径
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// 使用协议
        /// </summary>
        public string Protocol { get; private set; }

        /// <summary>
        /// 指定客户端发送的内容长度（以字节计）。
        /// </summary>
        /// <returns>客户端发送的内容的长度（以字节为单位）。</returns>
        public int ContentLength { get; private set; }

        /// <summary>
        /// 主体内容
        /// </summary>
        public string ContentBody { get; private set; }

        /// <summary>
        ///  获取或设置传入请求的 MIME 内容类型。
        /// </summary>
        /// <returns>表示传入请求的 MIME 内容类型的字符串，例如，“text/html”</returns>
        public string ContentType { get; private set; }

        /// <summary>
        ///  获取 HTTP 查询字符串变量集合。
        /// </summary>
        public NameValueCollection QueryString { get; private set; }

        /// <summary>
        /// 获取窗体变量集合。 
        /// </summary>
        public NameValueCollection Forms { get; private set; }

        /// <summary>
        /// 获取 HTTP 头集合
        /// </summary>
        public NameValueCollection Headers { get; private set; }

        /// <summary>
        /// 获取客户端使用的 HTTP 数据传输方法（如 GET、POST 或 HEAD）
        /// </summary>
        public string HttpMethod { get; private set; }

    }
}
