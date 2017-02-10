/*======================================================================
 *
 *        Copyright (C)  1996-2012  lfz    
 *        All rights reserved
 *
 *        Filename :RestServiceBase.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-07-15 10:57
 *        https://git.oschina.net/lfz/tools
 *
 *======================================================================*/

using System.Web;
using Lfz.Logging;

namespace Lfz.Rest
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class RestServiceBase : IHttpHandler
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Write(this.Process(context));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual string Process(HttpContext context)
        {
            string result = string.Empty;
            context.Response.ContentType = "application/json";
            var hasRight = true;
            if (hasRight)
            {
                string requestBody = context.ReadAsString();
                switch (context.Request.HttpMethod)
                {
                    case "DELETE":
                        result = ProcessDelete(context, requestBody);
                        break;
                    case "GET":
                        result = ProcessGet(context, requestBody);
                        break;
                    case "PUT":
                        result = ProcessPut(context, requestBody);
                        break;
                    case "POST":
                        result = ProcessPost(context, requestBody);
                        break;
                    default:
                        result = ProcessDefault(context, requestBody);
                        break;
                }
            }
            else
            {
                result = new ResponseContent(null) { StatusCode = RestStatus.NoPower }.ToJsonString();
            }
            return result;
        }

        /// <summary>
        /// 日志
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected RestServiceBase()
        {
            Logger = LoggerFactory.GetLog();
        }

        /// <summary>
        /// GET方法处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public virtual string ProcessGet(HttpContext context, string requestBody)
        {
            return new ResponseContent(null) { StatusCode = RestStatus.NoImp }.ToJsonString();
        }

        /// <summary>
        /// Put方法处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public virtual string ProcessPut(HttpContext context, string requestBody)
        {
            return new ResponseContent(null) { StatusCode = RestStatus.NoImp }.ToJsonString();
        }

        /// <summary>
        /// POST方法处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public virtual string ProcessPost(HttpContext context, string requestBody)
        {
            return new ResponseContent(null) { StatusCode = RestStatus.NoImp }.ToJsonString();
        }


        /// <summary>
        /// 删除处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public virtual string ProcessDelete(HttpContext context, string requestBody)
        {
            return new ResponseContent(null) { StatusCode = RestStatus.NoImp }.ToJsonString();
        }


        /// <summary>
        /// 默认处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public virtual string ProcessDefault(HttpContext context, string requestBody)
        {
            return new ResponseContent(null) { StatusCode = RestStatus.NoImp }.ToJsonString();
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}