/*======================================================================
 *
 *        Copyright (C)  1996-2012  lfz    
 *        All rights reserved
 *
 *        Filename :RestServiceBaseExtensions.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-07-15 10:57
 *        https://git.oschina.net/lfz/tools
 *
 *======================================================================*/

using System;
using System.Collections.Generic;

namespace Lfz.Rest
{
    /// <summary>
    /// RestService服务扩展方法
    /// </summary>
    public static class RestServiceBaseExtensions
    {

        #region RestResponse辅助方法(常见响应辅助方法) 2013-01-05日重构，从原来RestServiceBase类中移除

        /// <summary>
        /// 操作成功，单不携带额外的数据
        /// </summary>
        /// <returns></returns>
        public static string Response(this RestServiceBase restService, IJsonContent content)
        {
            var response = new ResponseContent(content) { StatusCode = RestStatus.Success, };

            return response.ToJsonString();
        }

        /// <summary>
        /// 操作成功，单不携带额外的数据
        /// </summary>
        /// <returns></returns>
        public static string Response(this RestServiceBase restService)
        {
            return new ResponseContent() { StatusCode = RestStatus.Success, }.ToJsonString();
        }

        /// <summary>
        /// 无权限
        /// </summary>
        /// <returns></returns>
        public static string NopowerResponse(this RestServiceBase restService)
        {
            return Response(restService, RestStatus.NoPower);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        public static string Response(this RestServiceBase restService, RestStatus restStatus)
        {
            return new ResponseContent { StatusCode = restStatus, }.ToJsonString();
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        public static string ErrorResponse(this RestServiceBase restService)
        {
            return Response(restService, RestStatus.Error);
        }

        /// <summary>
        /// 未实现
        /// </summary>
        /// <returns></returns>
        public static string NoImpResponse(this RestServiceBase restService)
        {
            return Response(restService, RestStatus.NoImp);
        }

        /// <summary>
        /// 操作成功，单不携带额外的数据
        /// </summary>
        /// <returns></returns>
        public static string ErrorResponse(this RestServiceBase restService, string message)
        {
            return new ResponseContent { StatusCode = RestStatus.Error, Content = new MessageContent { Message = message } }.ToJsonString();
        }

        /// <summary>
        /// 操作成功，单不携带额外的数据
        /// </summary>
        /// <returns></returns>
        public static string DefaultErrorResponse(this RestServiceBase restService, string messageTitle)
        {
            return new ResponseContent
                {
                    StatusCode = RestStatus.Error,
                    Content = new MessageContent { Message = string.Format("{0}失败，请联系维护人员！", messageTitle) }
                }.ToJsonString();
        }


        #endregion

        #region JSON反序列化相关 2013-01-05日添加

        #region REST响应(REST API调用处理完毕后给客户端的响应信息)

        /// <summary>
        /// REST响应 JSON反序列化
        /// <example>
        /// 对如下类对象的反序列化:
        ///  public sealed class RestResponse 
        ///  {  
        ///      public RestStatus StatusCode { get; set; } 
        ///      [JsonProperty("Result")]   
        ///      public IJsonContent Content { get; set; }
        ///  }
        /// 这里Content是接口，在反序列化时需要具体化类型.
        /// 主要是在RestClient中处理响应时需要用到
        /// </example>
        /// </summary> 
        /// <typeparam name="TSubContentImp">JSON模型子内容，其以接口类型实现,需要根据类型具体化</typeparam>
        /// <param name="restService"></param>
        /// <param name="jsonContent"></param> 
        /// <returns></returns>
        public static ResponseContent DeserializeResponse<TSubContentImp>(this RestServiceBase restService, string jsonContent)
        {
            return JsonUtils.DeserializeObject<ResponseContent, TSubContentImp>(jsonContent, JsonPropertyFilterEnum.RestResponseResult);
        }

        /// <summary>
        /// REST响应 JSON反序列化 
        /// </summary> 
        /// <param name="restService"></param>
        /// <param name="jsonContent"></param> 
        /// <returns></returns>
        public static ResponseContent DeserializeResponse(this RestServiceBase restService, string jsonContent)
        {
            return JsonUtils.DeserializeObject<ResponseContent>(jsonContent);
        }

        /// <summary>
        /// REST响应 JSON反序列化
        /// <example>
        ///  public sealed class RestResponse 
        ///  {  
        ///      public RestStatus StatusCode { get; set; } 
        ///      [JsonProperty("Result")]  
        ///      public IJsonContent Content { get; set; }
        ///  }
        /// 这里Content是接口，在反序列化时需要具体化类型.
        /// 主要是在Rest Client中处理响应时需要用到
        /// </example>
        /// </summary> 
        /// <typeparam name="TDataContentImp"></typeparam>
        /// <typeparam name="TSubContentImp">JSON模型子内容，其以接口类型实现,需要根据类型具体化</typeparam>
        /// <param name="restService"></param>
        /// <param name="jsonContent"></param> 
        /// <returns></returns>
        public static ResponseContent DeserializeResponse<TDataContentImp, TSubContentImp>(this RestServiceBase restService,
                                                                                        string jsonContent)
        {
            var dic = new Dictionary<JsonPropertyFilterEnum, Type>
                {
                    {JsonPropertyFilterEnum.RestResponseResult, typeof (TDataContentImp)},
                    {JsonPropertyFilterEnum.SettingModelData, typeof (TSubContentImp)}
                };
            return JsonUtils.DeserializeObject<ResponseContent, TSubContentImp>(jsonContent, dic);
        }


        #endregion

        #region REST接收数据（收到客户端请求数据（JSON格式））的JSON反序列化

        /// <summary>
        /// JSON反序列化(反序列化包含属性Data的JSON字符串，其中Data数据内容派生自ParamsDataBase)
        /// <example>
        /// 对如下类对象进行反序列化
        ///  public sealed class SettingModel : SettingModelBase 
        ///  {  
        ///      public IJsonContent Data { get; set; }
        ///  }
        /// 这里Content是接口，在反序列化时需要具体化类型.
        /// 主要是在Rest Client中处理响应时需要用到
        /// </example>
        /// </summary> 
        /// <param name="restService"></param>
        /// <typeparam name="TElement">JSON模型子内容，其以接口类型实现,需要根据类型具体化</typeparam>
        /// <param name="jsonContent"></param> 
        /// <returns></returns>
        public static TElement DeserializeData<TElement>(this RestServiceBase restService, string jsonContent) where TElement : class,IJsonContent
        {
            var model = JsonUtils.DeserializeObject<MultifunctionalContent, TElement>(jsonContent, JsonPropertyFilterEnum.SettingModelData);
            if (model != null) return model.Data as TElement;
            return null;
        }

        /// <summary>
        /// JSON反序列化(反序列化包含属性Data的JSON字符串，其中Data数据内容派生自ParamsDataBase)
        /// <example>
        /// 对如下类对象进行反序列化
        ///  public sealed class SettingModel : SettingModelBase 
        ///  {  
        ///      public IJsonContent Data { get; set; }
        ///  }
        /// 这里Content是接口，在反序列化时需要具体化类型.
        /// 主要是在Rest Client中处理响应时需要用到
        /// </example>
        /// </summary> 
        /// <param name="restService"></param>
        /// <typeparam name="TSubElement">JSON模型子内容，其以接口类型实现,需要根据类型具体化</typeparam>
        /// <param name="jsonContent"></param> 
        /// <returns></returns>
        public static MultifunctionalContent DeserializeContent<TSubElement>(this RestServiceBase restService, string jsonContent) where TSubElement : class,IJsonContent
        {
            var model = JsonUtils.DeserializeObject<MultifunctionalContent, TSubElement>(jsonContent, JsonPropertyFilterEnum.SettingModelData);
            if (model != null) return model;
            return null;
        }
        #endregion

        /// <summary>
        /// JSON反序列化 
        /// </summary>
        /// <typeparam name="T"></typeparam> 
        /// <param name="restService"></param>
        /// <param name="jsonContent"></param> 
        /// <returns></returns>
        public static T Deserialize<T>(this RestServiceBase restService, string jsonContent)
        {
            return JsonUtils.DeserializeObject<T>(jsonContent);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static JsonList<T> ToJsonList<T>(this IEnumerable<T> collection)
        {
            return new JsonList<T>(collection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="restService"></param>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ToJsonListResponse<T>(this RestServiceBase restService, IEnumerable<T> collection)
        {
            var list = collection == null ? new JsonList<T>() : new JsonList<T>(collection);
            return restService.Response(list);
        }


    }
}