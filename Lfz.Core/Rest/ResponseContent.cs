using Newtonsoft.Json;

namespace Lfz.Rest
{
    /// <summary>
    /// Rest服务响应信息(服务端使用，需要序列化的位置)
    /// </summary>
    public sealed class ResponseContent : ResponseContent<IJsonContent>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ResponseContent()
            : this(new EmptyContent() { })
        {
        }

        /// <summary>
        /// 根据Rest内容构造响应信息
        /// </summary>
        /// <param name="content"></param>
        public ResponseContent(IJsonContent content)
        {
            this.Content = content;
        }

        /// <summary>
        ///  Rest服务响应内容
        /// </summary>
        [JsonProperty("Result")]
        [RestResolverFilter(JsonPropertyFilterEnum.RestResponseResult)]
        public override IJsonContent Content { get; set; } 
    }
    /// <summary>
    /// CQRS模式中Command的返回结果
    /// </summary>
    public interface ICommandResult
    {


    }
    /// <summary>
    /// Rest服务响应信息(客户端使用，需要反序列化的位置)
    /// </summary>
    public class ResponseContent<TContent> : RestContentBase, ICommandResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ResponseContent()
        {
        }

        /// <summary>
        ///  Rest服务响应内容
        /// </summary>
        [JsonProperty("Result")]
        public virtual TContent Content { get; set; }

        /// <summary>
        /// Rest服务响应状态
        /// </summary> 
        [JsonProperty("StatusCode")]
        public RestStatus StatusCode { get; set; }
    }
}