// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :ApiHttpClient.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-02-01 16:24
// *        https://git.oschina.net/lfz/tools
// *
// *======================================================================*/

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Lfz.Config;
using Lfz.Logging;
using Lfz.Rest;

namespace Lfz.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public static class ApiHttpClient
    {
        static readonly ILogger Logger;

        static ApiHttpClient()
        {
            Logger = LoggerFactory.GetLog(RunConfig.Current.LoggerType);
        }

        #region Methods
        /// <summary>
        /// 获取WebApi操作对象(API调用2分钟有效时间，过期无反映表示超时)
        /// </summary> 
        /// <returns>WebApi操作对象</returns>
        static HttpClient GetHttpClient()
        {

            var handler = new HttpClientHandler { AllowAutoRedirect = false };
            var httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };
            // 为JSON格式添加一个Accept报头
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.MaxResponseContentBufferSize = 10240000;//64K
            // Add a user-agent header
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

            return httpClient;
        }

        public static void Post<T>(string requestUri, T data)
        {
            HttpClient client = null;
            HttpResponseMessage response = null;
            try
            {
                client = GetHttpClient();
                response = client.PostAsJsonAsync(requestUri, data).Result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Post:" + requestUri + " Msg:" + e.Message);
            }
            finally
            {
                if (response != null)
                    response.Dispose();
                if (client != null)
                    client.Dispose();
            }
        }


        public static void Put<T>(string requestUri, T data)
        {
            HttpClient client = null;
            HttpResponseMessage response = null;
            try
            {
                client = GetHttpClient();
                response = client.PutAsJsonAsync(requestUri, data).Result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Post:" + requestUri + " Msg:" + e.Message);
            }
            finally
            {
                if (response != null)
                    response.Dispose();
                if (client != null)
                    client.Dispose();
            }
        }

        public static void Delete(string requestUri)
        {
            HttpClient client = null;
            HttpResponseMessage response = null;
            try
            {
                client = GetHttpClient();
                response = client.DeleteAsync(requestUri).Result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Post:" + requestUri + " Msg:" + e.Message);
            }
            finally
            {
                if (response != null)
                    response.Dispose();
                if (client != null)
                    client.Dispose();
            }
        }
        public static TResult Post<T, TResult>(string requestUri, T data)
            where TResult : IJsonContent
        {
            HttpClient client = null;
            HttpResponseMessage response = null;
            ResponseContent<TResult> result = new ResponseContent<TResult>();
            try
            {
                client = GetHttpClient();
                response = client.PostAsJsonAsync(requestUri, data).Result;
                string json = response.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrEmpty(json))
                {
                    Logger.Error("Post:" + requestUri + " Msg:内容为空");
                    return result.Content;
                }
                result = JsonUtils.DeserializeObject<ResponseContent<TResult>>(json) ??
                           new ResponseContent<TResult> { StatusCode = RestStatus.NoImp, Content = default(TResult) };
                if (result.StatusCode != RestStatus.Success)
                    Logger.Error(string.Format("Post:{0} StatusCode{1} Json:{2}", requestUri, result.StatusCode, json));
            }
            catch (Exception e)
            {
                result.StatusCode = RestStatus.Error;
                Logger.Error(e, "Post:" + requestUri + " Msg:" + e.Message);
            }
            finally
            {
                if (response != null)
                    response.Dispose();
                if (client != null)
                    client.Dispose();
            }
            return result.Content;
        }

        public static TResult PostWithApiResult<T, TResult>(string requestUri, T data)
            where TResult : ApiResult, new()
        {
            HttpClient client = null;
            HttpResponseMessage response = null;
            TResult result = new TResult();
            try
            {
                client = GetHttpClient();
                response = client.PostAsJsonAsync(requestUri, data).Result;
                string json = response.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrEmpty(json))
                {
                    Logger.Error("Post:" + requestUri + " Msg:内容为空");
                    result.Status = -99;
                    return result;
                }
                result = JsonUtils.DeserializeObject<TResult>(json) ?? new TResult() { Status = -99 };
                if (result.Status != 0)
                    Logger.Error(string.Format("Post:{0} StatusCode{1} Json:{2}", requestUri, result.Status, json));
            }
            catch (Exception e)
            {
                result.Status = 0;
                Logger.Error(e, "Post:" + requestUri + " Msg:" + e.Message);
            }
            finally
            {
                if (response != null)
                    response.Dispose();
                if (client != null)
                    client.Dispose();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static TResult Get<TResult>(string requestUri)
        {
            HttpClient client = null;
            HttpResponseMessage response = null;
            TResult result = default(TResult);
            try
            {
                client = GetHttpClient();
                response = client.GetAsync(requestUri).Result;
                string json = response.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrEmpty(json)) return default(TResult);
                result = JsonUtils.DeserializeObject<TResult>(json);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Get:" + requestUri + " Msg:" + e.Message);
            }
            finally
            {
                if (response != null)
                    response.Dispose();
                if (client != null)
                    client.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 获取数据(响应ResponseContent)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static TResult GetResponse<TResult>(string requestUri)
            where TResult : IJsonContent, new()
        {
            HttpClient client = null;
            HttpResponseMessage response = null;
            ResponseContent<TResult> result = new ResponseContent<TResult>();
            try
            {
                client = GetHttpClient();
                response = client.GetAsync(requestUri).Result;
                string json = response.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrEmpty(json))
                {
                    Logger.Error("GetResponse:" + requestUri + " Msg:内容为空");
                    return default(TResult);
                }
                result = JsonUtils.DeserializeObject<ResponseContent<TResult>>(json) ??
                           new ResponseContent<TResult> { StatusCode = RestStatus.NoImp, Content = default(TResult) };
                if (result.StatusCode != RestStatus.Success)
                    Logger.Error(string.Format("GetResponse:{0} StatusCode{1} Json:{2}", requestUri, result.StatusCode, json));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Get:" + requestUri + " Msg:" + e.Message);
            }
            finally
            {
                if (response != null)
                    response.Dispose();
                if (client != null)
                    client.Dispose();
            }
            return result.Content;
        }


        /// <summary>
        /// 获取数据
        /// </summary> 
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static ResponseContent GetResponse(string requestUri)
        {
            HttpClient client = null;
            HttpResponseMessage response = null;
            ResponseContent result = null;
            try
            {
                client = GetHttpClient();
                response = client.GetAsync(requestUri).Result;
                string json = response.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrEmpty(json)) return new ResponseContent() { StatusCode = RestStatus.InvalidChars };
                result = JsonUtils.DeserializeObject<ResponseContent>(json);
                if (result == null)
                {
                    result = new ResponseContent { StatusCode = RestStatus.NoImp };
                }
                if (result.StatusCode != RestStatus.Success)
                    Logger.Error(string.Format("GetResponse:{0} StatusCode{1} Json:{2}", requestUri, result.StatusCode, json));
            }
            catch (Exception e)
            {
                Logger.Error(e, "GetResponse:" + requestUri + " Msg:" + e.Message);
                result = new ResponseContent() { StatusCode = RestStatus.Error, Content = new MessageContent() { Message = e.Message } };
            }
            finally
            {
                if (response != null)
                    response.Dispose();
                if (client != null)
                    client.Dispose();
            }
            return result;
        }



        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static TResult GetByApiResult<TResult>(string requestUri)
            where TResult : ApiResult
        {
            return ApiHttpClient.Get<TResult>(requestUri);
        }
    }



}