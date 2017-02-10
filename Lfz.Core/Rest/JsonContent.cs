using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lfz.Rest
{

    /// <summary>
    /// 支持支持Json序列化的列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [JsonArray]
    public class JsonList<T> : List<T>, IJsonContent
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public JsonList()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="collection">数据列表</param>
        public JsonList(IEnumerable<T> collection)
            : base(collection)
        {
        }
    }

    /// <summary>
    /// 多功能内容（一个REST URL资源可根据内容作多种功能实现）
    /// </summary> 
    public class MultifunctionalContent : IJsonContent
    {
        /// <summary>
        /// 
        /// </summary> 
        public string Method { get; set; }

        /// <summary>
        /// 参数内容
        /// </summary> 
        [RestResolverFilter(JsonPropertyFilterEnum.SettingModelData)]
        public IJsonContent Data { get; set; }
    }

    /// <summary>
    ///  多功能内容（一个REST URL资源可根据内容作多种功能实现）,其携带一个主键值
    /// </summary>
    /// <typeparam name="T"></typeparam> 
    public class MultifunctionalContent<T> : MultifunctionalContent
    {
        /// <summary>
        /// 
        /// </summary> 
        public T Id { get; set; }
    }

    /// <summary>
    /// 单一值域的Rest主体内容（可序列化JSON字符串）
    /// </summary>
    /// <typeparam name="T"></typeparam> 
    public class SingleContent<T> : IJsonContent
    {
        /// <summary>
        /// 
        /// </summary> 
        public T Value { get; set; }
    }

    /// <summary>
    /// 支持Rest服务的消息内容（可JSON序列化）
    /// </summary> 
    public class MessageContent : IJsonContent
    {

        /// <summary>
        /// 消息类型
        /// </summary> 
        public string Message { get; set; }
    }

    /// <summary>
    /// 布尔类型Rest内容主体
    /// </summary> 
    public class BoolContent : IJsonContent
    {
        /// <summary>
        /// 标题当前内容是True或False
        /// </summary> 
        public bool Flag { get; set; }
    }


    /// <summary>
    /// 布尔类型Rest内容主体
    /// </summary> 
    public class EmptyContent : IJsonContent
    {
    }

    /// <summary>
    /// 支持支持Json序列化的列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IJsonContent
    {
    }

}