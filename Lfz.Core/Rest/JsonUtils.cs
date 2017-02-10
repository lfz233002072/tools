using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lfz.Rest
{
    /// <summary>
    /// 使用Newtonsoft.Json序列化/反序列化JSON数据工具类
    /// </summary>
    public static class JsonUtils
    {
        /// <summary>
        /// 反序列化json字符串为一个对象
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T DeserializeObject<T>(string jsonContent)
        {
            return DeserializeObject<T>(jsonContent, new Dictionary<JsonPropertyFilterEnum, Type>());
        }

        /// <summary>
        /// 反序列化json字符串为一个对象
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DeserializeObject(string jsonContent, Type type)
        {
            return DeserializeObject(type, jsonContent, new Dictionary<JsonPropertyFilterEnum, Type>());
        }

        /// <summary>
        /// REST响应 JSON反序列化
        /// <example>
        /// 对如下类对象的反序列化:
        ///  public sealed class JsonContent 
        ///  {  
        ///      public RestStatus StatusCode { get; set; } 
        ///      [RestResolverFilter(filterEnum)]
        ///      public IJsonContent Content { get; set; }
        ///  }
        /// 这里Content是接口，在反序列化时需要具体化类型。而其反序列化类型的查找，通过filterEnum获取
        /// </example>
        /// </summary> 
        /// <typeparam name="TSubContentImp">JSON模型子内容，其以接口类型实现,需要根据类型具体化</typeparam>
        /// <typeparam name="T">内容主体格式</typeparam>
        /// <param name="jsonContent"></param>
        /// <param name="filterEnum"></param>
        /// <returns></returns>
        public static T DeserializeObject<T, TSubContentImp>(string jsonContent, JsonPropertyFilterEnum filterEnum)
        {
            var dic = new Dictionary<JsonPropertyFilterEnum, Type> { { filterEnum, typeof(TSubContentImp) } };
            Dictionary<JsonPropertyFilterEnum, Type> dicPropertyNameType = dic;
            return DeserializeObject<T, TSubContentImp>(jsonContent, dicPropertyNameType);
        }

        /// <summary>
        /// REST响应 JSON反序列化
        /// <example>
        /// 对如下类对象主体内容形如:
        ///  public sealed class JsonContent 
        ///  {  
        ///      public RestStatus StatusCode { get; set; } 
        ///      [RestResolverFilter(filterEnum)]
        ///      public IJsonContent Content { get; set; }
        ///  }
        /// 这里Content是接口，在反序列化时需要具体化类型。而其反序列化类型的查找，通过filterEnum获取。
        /// 在构造反序列化解析器时，传入一个反序列化实现类型字典以供查找实现类型。
        /// </example>
        /// </summary>   
        /// <param name="type"></param>
        /// <param name="jsonContent"></param>
        /// <param name="dicPropertyNameType">传入一个反序列化实现类型字典以供查找实现类型</param> 
        /// <returns>返回反序列化后的对象实体</returns>
        public static object DeserializeObject(Type type, string jsonContent, IDictionary<JsonPropertyFilterEnum, Type> dicPropertyNameType)
        {
            var settings = new JsonSerializerSettings
                {
                    ContractResolver = new RestContentContractResolver(dicPropertyNameType)
                };
            var dataConvert = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            settings.Converters.Add(dataConvert);
            return (string.IsNullOrEmpty(jsonContent) ? null : JsonConvert.DeserializeObject(jsonContent, type, settings));
        }

        /// <summary>
        /// REST响应 JSON反序列化
        /// <example>
        /// 对如下类对象主体内容形如:
        ///  public sealed class JsonContent 
        ///  {  
        ///      public RestStatus StatusCode { get; set; } 
        ///      [RestResolverFilter(filterEnum)]
        ///      public IJsonContent Content { get; set; }
        ///  }
        /// 这里Content是接口，在反序列化时需要具体化类型。而其反序列化类型的查找，通过filterEnum获取。
        /// 在构造反序列化解析器时，传入一个反序列化实现类型字典以供查找实现类型。
        /// </example>
        /// </summary>  
        /// <typeparam name="T">内容主体格式</typeparam>
        /// <param name="jsonContent"></param>
        /// <param name="dicPropertyNameType">传入一个反序列化实现类型字典以供查找实现类型</param> 
        /// <returns>返回反序列化后的对象实体</returns>
        public static T DeserializeObject<T>(string jsonContent, IDictionary<JsonPropertyFilterEnum, Type> dicPropertyNameType)
        {
            var settings = new JsonSerializerSettings
                {
                    ContractResolver = new RestContentContractResolver(dicPropertyNameType)
                };
            var dataConvert = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            settings.Converters.Add(dataConvert);
            return (string.IsNullOrEmpty(jsonContent) ? default(T) : JsonConvert.DeserializeObject<T>(jsonContent, settings));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSubContentImp"></typeparam>
        /// <param name="jsonContent"></param>
        /// <param name="dicPropertyNameType"></param>
        /// <returns></returns>
        public static T DeserializeObject<T, TSubContentImp>(string jsonContent, IDictionary<JsonPropertyFilterEnum, Type> dicPropertyNameType)
        {
            if (!dicPropertyNameType.ContainsKey(JsonPropertyFilterEnum.RestResponseResult))
            {
                dicPropertyNameType.Add(new KeyValuePair<JsonPropertyFilterEnum, Type>(JsonPropertyFilterEnum.RestResponseResult, typeof(TSubContentImp)));
            }
            return DeserializeObject<T>(jsonContent, dicPropertyNameType);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static string SerializeObject(object obj, params JsonConverter[] converters)
        {
            return SerializeObject(obj, Formatting.None, converters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="formatting"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static string SerializeObject(object obj, Formatting formatting, params JsonConverter[] converters)
        {
            if (obj == null) return string.Empty;
            if ((converters == null) || (converters.Length == 0))
            {
                var itemlist = new JsonConverter[1];
                var dataConvert = new IsoDateTimeConverter
                {
                    DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
                };
                itemlist[0] = dataConvert;
                converters = itemlist;
            }
            else if (converters.All(x => !(x.GetType() == typeof(IsoDateTimeConverter))))
            {
                var list = converters.ToList();
                var dataConvert = new IsoDateTimeConverter
                {
                    DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
                };
                list.Add(dataConvert);
                converters = list.ToArray();
            }
            return JsonConvert.SerializeObject(obj, formatting, converters);
        }


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
        /// <param name="jsonContent"></param> 
        /// <returns></returns>
        public static ResponseContent DeserializeResponse<TSubContentImp>(string jsonContent)
        {
            return DeserializeObject<ResponseContent, TSubContentImp>(jsonContent, JsonPropertyFilterEnum.RestResponseResult);
        }

        /// <summary>
        /// REST响应 JSON反序列化 
        /// </summary>  
        /// <param name="jsonContent"></param> 
        /// <returns></returns>
        public static ResponseContent DeserializeResponse(string jsonContent)
        {
            return DeserializeObject<ResponseContent>(jsonContent);
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
        /// <param name="jsonContent"></param> 
        /// <returns></returns>
        public static ResponseContent DeserializeResponse<TDataContentImp, TSubContentImp>(string jsonContent)
        {
            var dic = new Dictionary<JsonPropertyFilterEnum, Type>
                {
                    {JsonPropertyFilterEnum.RestResponseResult, typeof (TDataContentImp)},
                    {JsonPropertyFilterEnum.SettingModelData, typeof (TSubContentImp)}
                };
            return DeserializeObject<ResponseContent, TSubContentImp>(jsonContent, dic);
        }

        #endregion

        /// <summary>
        /// JSON反序列化 
        /// </summary>
        /// <typeparam name="T"></typeparam>  
        /// <param name="jsonContent"></param> 
        /// <returns></returns>
        public static T Deserialize<T>(string jsonContent)
        {
            return DeserializeObject<T>(jsonContent);
        }

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
  
    }
}