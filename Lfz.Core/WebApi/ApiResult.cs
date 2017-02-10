using System;
using Newtonsoft.Json;

namespace Lfz.WebApi
{
    /// <summary>
    /// 
    /// </summary> 
    public class ApiResult
    {
        /// <summary>
        /// 是否使用成功 0 成功
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// api操作消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 状态原始值（整数）
        /// </summary>
        [JsonIgnore]
        public ServerApiStatus ApiStatus
        {
            get
            {
                if (Enum.IsDefined(typeof(ServerApiStatus), Status))
                    return (ServerApiStatus)Enum.Parse(typeof(ServerApiStatus), Status.ToString());
                return ServerApiStatus.None;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T> : ApiResult
    {
        /// <summary>
        /// 当前获取的数据集合
        /// </summary>
        public T Data { get; set; }
    }
}