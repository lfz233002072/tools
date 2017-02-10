using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lfz.Rest
{
    /// <summary>
    /// Rest内容主体解析器
    /// </summary>
    public class RestContentContractResolver : DefaultContractResolver
    {
        private readonly IDictionary<JsonPropertyFilterEnum, Type> _dicPropertyNameType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicPropertyNameType">JSON属性过滤器列表，所有过滤器中的属性均为需要指定类型重构反序列化组件的</param>
        public RestContentContractResolver(IDictionary<JsonPropertyFilterEnum, Type> dicPropertyNameType)
            : base(false)
        {
            this._dicPropertyNameType = dicPropertyNameType ?? new Dictionary<JsonPropertyFilterEnum, Type>();
        }

        /// <summary>
        /// 创建反序列化中需要使用到的JsonProperty属性列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            var filterList = GetFilterAttrbute(type);
            return FilterProperties(properties, filterList);
        }

        private IList<JsonProperty> FilterProperties(IList<JsonProperty> properties, IList<FilterInfo> filterList)
        {
            foreach (var jsonProperty in properties)
            {
                if (jsonProperty.PropertyType == null) continue;
                //获取其属性类型是否实现RestResolverFilterAttribute属性
                var filterInfo = filterList.FirstOrDefault(x => x.PropertyType == jsonProperty.PropertyType && x.Name == jsonProperty.PropertyName);
                if (filterInfo == null || filterInfo.FilterAttribute == null) continue;
                var attr = filterInfo.FilterAttribute;
                if (attr.UniqueKey == JsonPropertyFilterEnum.None) continue;
                //如果包含有效的RestResolverFilterAttribute属性，那么需要进行类型替换。如果替换类型不存在，那么移除Json属性
                if (_dicPropertyNameType.ContainsKey(attr.UniqueKey))
                {
                    var value = _dicPropertyNameType[attr.UniqueKey];
                    jsonProperty.PropertyType = value;
                    jsonProperty.Required = Required.Default;
                }
                else
                {
                    //当前需要过滤处理，但是未传入有效参数,所以移除特性
                    jsonProperty.PropertyType = null;
                }
            }
            //只有PropertyType不为空的才是有效类型
            properties = properties.Where(x => x.PropertyType != null).ToList();
            return properties;
        }

        private IList<FilterInfo> GetFilterAttrbute(Type type)
        {
            var dictionary = new List<FilterInfo>();
            //声明属性的类型获取 
            var propertyList = type.GetProperties();
            foreach (var propertyInfo in propertyList)
            {
                var filterAttribute = GetAttribute<RestResolverFilterAttribute>(propertyInfo);
                if (filterAttribute == null) continue;
                var typeName = new FilterInfo();
                var attribute = GetAttribute<JsonPropertyAttribute>(propertyInfo);
                typeName.Name = attribute != null ? attribute.PropertyName : propertyInfo.Name;
                typeName.PropertyType = propertyInfo.PropertyType;
                typeName.FilterAttribute = filterAttribute;
                dictionary.Add(typeName);
            }
            return dictionary;
        }

        private T GetAttribute<T>(PropertyInfo memberInfo) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
        }

        private class FilterInfo
        {
            public string Name { get; set; }
            public Type PropertyType { get; set; }
            public RestResolverFilterAttribute FilterAttribute { get; set; }
        }
    }
}