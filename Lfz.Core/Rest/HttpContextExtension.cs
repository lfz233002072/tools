using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using Lfz.Logging;
using Lfz.Utitlies;

namespace Lfz.Rest
{
    /// <summary>
    /// HttpContext 扩展信息 
    /// </summary>
    public static class HttpContextExtension
    {
        /// <summary>
        /// 返回context.Request输入流转的UTF8字符串
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string ReadAsString(this HttpContext context)
        {
            string s = string.Empty;
            using (var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
            {
                s = reader.ReadToEnd();
                LoggerFactory.GetLog().Log(LogLevel.Information, "Recevice：" + s);
            }
            return s;
        }


        /// <summary>
        ///  获取 HTTP 查询字符串变量集合
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static NameValueCollection QueryString(this HttpContext context)
        {
            return context.Request.QueryString;
        }

        /// <summary>
        /// 根据键值从HTTP查询字符串变量集合中获取Int值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetIntFromQueryString(this HttpContext context, string key)
        {
            return TypeParse.StrToInt(context.Request.QueryString[key]);
        }

        /// <summary>
        /// 根据键值从HTTP查询字符串变量集合中获取字符串值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueFromQueryString(this HttpContext context, string key)
        {
            return context.Request.QueryString[key];
        }
    }
}