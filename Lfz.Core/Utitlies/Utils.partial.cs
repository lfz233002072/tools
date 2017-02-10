//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : Utils
//        DESCRIPTION : 工具箱类
//
//        Created By 林芳崽 at  2012-08-25 19:32:40
//        https://git.oschina.net/lfz/tools
//
//====================================================================== 

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lfz.Utitlies
{
    /// <summary>
    /// 辅助工具箱，包括IP获取、字符转换、枚举处理等
    /// </summary>
    public static partial class Utils
    {
        #region 构造时间

        /// <summary>
        /// 获取日期值
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="date"></param>
        /// <param name="hour"></param>
        /// <param name="min"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static DateTime Getdate(int year, int month, int date, int hour, int min, int second)
        {
            var d = new DateTime(1980, 1, 1, 0, 0, 0);
            return TypeParse.StrToDateTime(String.Format("{0}-{1}-{2} {3}:{4}:{5}", year, month, date, hour, min, second), d);
        }

        #endregion

        /// <summary>
        ///  输入日期是否大于等于指定时间
        /// </summary>
        /// <param name="compairDate"></param>
        /// <param name="inputDate"></param>
        /// <returns></returns>
        public static bool DateIsLargerOrEqual(DateTime compairDate, DateTime inputDate)
        {
            return compairDate.Date <= inputDate.Date;
        }


        /// <summary>
        /// 输入日期是否为今天或今天之后
        /// </summary> 
        /// <param name="inputDate"></param>
        /// <returns></returns>
        public static bool DateIsLargerOrEqual(DateTime inputDate)
        {
            return DateIsLargerOrEqual(DateTime.Now, inputDate);
        }

        #region 获取当前年份到1900年1月1日相差的天数

        /// <summary>
        /// 获取当前年份到1900年1月1日相差的天数
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetDifDays(DateTime date)
        {
            //1900年1月1日=599266080000000000
            return TimeSpan.FromTicks(date.Ticks - 599266080000000000).Days;
        }

        #endregion


        #region Base64字符串

        /// <summary>
        /// UTF8字符串转化为Base64字符串
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ToBase64String(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
        }

        /// <summary>
        /// 从Base64字符串中获取值UTF8字符串
        /// </summary>
        /// <param name="base64Str"></param>
        /// <returns></returns>
        public static string FromBase64String(string base64Str)
        {
            if (string.IsNullOrEmpty(base64Str)) return string.Empty;
            var bytes = Convert.FromBase64String(base64Str);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        #endregion
        #region 枚举扩展

        /// <summary>
        /// 获取枚举对象的描述备注信息，如果没有那么返回枚举值得名称（通过判断枚举值是否携带属性CustomDescriptionAttribute，获取枚举描述信息）。
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="obj">可以有枚举值、枚举名称、枚举对象</param>
        /// <returns></returns>
        public static string GetEnumDesc<TEnum>(object obj) where TEnum : struct
        {
            if (obj == null) return string.Empty;
            TEnum enumObj;
            if (obj is TEnum) enumObj = (TEnum)obj;
            else
            {
                if (!Enum.TryParse(obj.ToString(), out enumObj)) return String.Empty;
            }
            var enumInfo = enumObj.GetType().GetField(enumObj.ToString());
            if (enumInfo == null) return obj.ToString();
            var enumAttributes = (CustomDescriptionAttribute[])enumInfo.
                GetCustomAttributes(typeof(CustomDescriptionAttribute), false);
            if (enumAttributes.Length > 0) return enumAttributes[0].Description;
            return enumObj.ToString();
        }

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static string GetEnumDesc(object obj, Type enumType)
        {
            if (obj == null) return string.Empty;
            if (!enumType.IsEnum) return obj.ToString();
            object enumObj = Enum.ToObject(enumType, obj);
            var enumInfo = enumType.GetField(enumObj.ToString());
            var enumAttributes = (CustomDescriptionAttribute[])enumInfo.
                GetCustomAttributes(typeof(CustomDescriptionAttribute), false);
            if (enumAttributes.Length > 0) return enumAttributes[0].Description;
            return enumObj.ToString();
        }


        /// <summary>
        /// 获取枚举描述信息列表（通过判断枚举值是否携带属性CustomDescriptionAttribute，获取枚举描述信息）
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static IEnumerable<string> GetEnumDescList<TEnum>() where TEnum : struct
        {
            List<string> list = new List<string>();
            var enumType = typeof(TEnum);
            var namelist = Enum.GetNames(enumType);
            foreach (var name in namelist)
            {
                var enumObj = (TEnum)Enum.Parse(enumType, name, false);
                var enumInfo = enumObj.GetType().GetField(name);
                var enumAttributes = (CustomDescriptionAttribute[])enumInfo.
                    GetCustomAttributes(typeof(CustomDescriptionAttribute), false);
                list.Add(enumAttributes.Length > 0 ? enumAttributes[0].Description : name);
            }
            return list;
        }
 


        /// <summary>
        /// 根据DescriptionAttribute属性或枚举值、枚举字符串获取枚举对象实例（通过判断枚举值是否携带属性CustomDescriptionAttribute，获取枚举描述信息）。
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="descOrNameOrValue"></param>
        /// <returns></returns>
        public static TEnum GetEnum<TEnum>(object descOrNameOrValue) where TEnum : struct
        {
            if (descOrNameOrValue == null) return default(TEnum);
            TEnum result;
            var enumType = typeof(TEnum);
            var namelist = Enum.GetNames(enumType);
            string descOrNameOrValueStr = descOrNameOrValue.ToString();
            if (Enum.TryParse(descOrNameOrValueStr, out result)) return result;
            foreach (var name in namelist)
            {
                //从描述字符串中获取
                var enumObj = (TEnum)Enum.Parse(enumType, name, false);
                var enumInfo = enumObj.GetType().GetField(name);
                var enumAttributes = (CustomDescriptionAttribute[])enumInfo.
                    GetCustomAttributes(typeof(CustomDescriptionAttribute), false);
                if (enumAttributes.Length > 0
                    && !String.IsNullOrEmpty(enumAttributes[0].Description)
                    && enumAttributes[0].Description.Equals(descOrNameOrValueStr, StringComparison.OrdinalIgnoreCase))
                {
                    result = enumObj;
                    break;
                }
            }
            return result;
        }

        #endregion


        #region 获取指定属性的DisplayAttribute名称


        /// <summary>
        /// 获取指定属性的DisplayAttribute名称
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public static string GetDisplayName(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrEmpty(propertyName)) return string.Empty;
            var objType = obj.GetType();
            return GetDisplayName(objType, propertyName);
        }

        /// <summary>
        /// 获取指定属性的DisplayAttribute名称
        /// </summary> 
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public static string GetDisplayName<TElement>(string propertyName) where TElement : class
        {
            return GetDisplayName(typeof(TElement), propertyName);
        }

        /// <summary>
        /// 获取指定属性的DisplayAttribute名称
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public static string GetDisplayName(Type objType, string propertyName)
        {
            if (!objType.IsClass) return propertyName;
            var property =
                objType.GetProperties()
                       .FirstOrDefault(x => x.Name.Equals(propertyName.Trim(), StringComparison.OrdinalIgnoreCase));
            if (property == null) return propertyName;
            var attr =
                property.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
            if (attr != null) return attr.Name;
            return property.Name;
        }

        /// <summary>
        /// 获取表名。如果有<see cref="CustomTableNameAttribute"/>属性那么取该属性标记的表名，否则返回类型名称。
        /// </summary>
        /// <param name="objType"></param> 
        /// <returns>返回表名</returns>
        public static string GetTableName(Type objType)
        {
            var attr =
                objType.GetCustomAttributes(typeof(CustomTableNameAttribute), true).FirstOrDefault() as CustomTableNameAttribute;
            if (attr != null) return attr.TableName;
            return objType.Name;
        }

        #endregion

        #region 属性映射

        /// <summary>
        /// 获取【属性名称-参数键值】列表,所有使用@开头的值都作为一个参数存在
        /// </summary> 
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> GetPropertyArgs(object obj)
        {
            if (obj == null) yield break;
            var type = obj.GetType();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var value = Convert.ToString(propertyInfo.GetValue(obj, new object[] { }));
                if (String.IsNullOrEmpty(value)) continue;
                value = value.Trim();
                if (!value.StartsWith("@")) continue;
                yield return new KeyValuePair<string, string>(propertyInfo.Name, value.Substring(1));
            }
        }

        /// <summary>
        /// 更新对象的属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value"></param>
        public static void UpdatePropertyValue(object obj, string propertyName, object value)
        {
            if (obj == null) return; ;
            var type = obj.GetType();
            var property = type.GetProperties().FirstOrDefault(x => x.Name == propertyName);
            if (property == null || property.GetType() != typeof(string)) return;
            property.SetValue(obj, Convert.ToString(value), new object[] { });
        }

        /// <summary>
        /// 获取【属性名称-参数键值】列表,所有使用@开头的值都作为一个参数存在,并根据属性更新参数
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="args"></param> 
        public static IEnumerable<KeyValuePair<string, string>> UpdatePropertiesValue(object obj, IDictionary<string, object> args)
        {
            if (obj == null) return Enumerable.Empty<KeyValuePair<string, string>>();
            var list = new List<KeyValuePair<string, string>>();
            var type = obj.GetType();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var value = Convert.ToString(propertyInfo.GetValue(obj, new object[] { }));
                if (String.IsNullOrEmpty(value)) continue;
                value = value.Trim();
                if (!value.StartsWith("@")) continue;
                var argKey = value.Substring(1);
                var propertyType = propertyInfo.PropertyType;
                if (propertyType == typeof(string))
                {
                    //如果参数列表包含自定参数,那么使用参数替换
                    if (args.ContainsKey(argKey))
                    {
                        try
                        {
                            object propertyValue;
                            if (!args.TryGetValue(argKey, out propertyValue)) value = String.Empty;
                            propertyValue = (propertyValue ?? String.Empty);
                            propertyInfo.SetValue(obj, propertyValue, new object[] { });
                        }
                        catch (Exception)
                        {
                        }
                    }
                    list.Add(new KeyValuePair<string, string>(propertyInfo.Name, argKey));
                }
            }
            return list;
        }

        #endregion



        #region URL token 生成(生成规则为base64为加密解密)

        /// <summary>
        /// 解析URL中的[名称-值列表】
        /// </summary>
        /// <param name="url"></param> 
        /// <returns></returns>
        public static Dictionary<string, string> GetUrlParamValues(string url)
        {
            var result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(url)) return result;
            var index = url.IndexOf('?');
            if (index > 0 && index != url.Length - 1) url = url.Substring(index + 1);
            if (string.IsNullOrEmpty(url)) return result;
            var list = url.Trim('&', '?').Split('&');
            foreach (var s in list)
            {
                var keyvalues = s.Split('=');
                if (keyvalues.Length < 2 || string.IsNullOrEmpty(keyvalues[0])) continue;
                var value = keyvalues[1] ?? string.Empty;
                var key = keyvalues[0].Trim();
                if (result.ContainsKey(key)) continue;
                result.Add(key, value);
            }
            return result;
        }

        /// <summary>
        /// 获取URL中指定参数名对应的参数值。如果不存在，那么返回string.Empty
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramKey"></param>
        /// <returns>返回参数值。如果不存在，那么返回string.Empty</returns>
        public static string GetUrlParamValue(string url, string paramKey)
        {
            string result;
            var dic = GetUrlParamValues(url);
            if (dic.TryGetValue(paramKey, out result)) return result;
            return string.Empty;
        }

        #endregion


      

        #region 获取指定属性的CustomDescriptionAttribute描述

        /// <summary>
        /// 获取指定属性的CustomDescriptionAttribute描述
        /// </summary>
        /// <param name="objType"></param> 
        /// <returns></returns>
        public static string GetCustomDescription(Type objType)
        {
            var attr =
                objType.GetCustomAttributes(typeof(CustomDescriptionAttribute), true).FirstOrDefault() as CustomDescriptionAttribute;
            if (attr != null) return attr.Description;
            return objType.FullName;
        }

        /// <summary>
        /// 获取指定属性的CustomDescriptionAttribute描述
        /// </summary> 
        /// <returns></returns>
        public static string GetCustomDescription<T>()
        {
            return GetCustomDescription(typeof(T));
        }
        #endregion



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="tb"></param>
        /// <returns></returns>
        public static List<TElement> TableConvertToList<TElement>(DataTable tb) where TElement : new()
        {
            List<TElement> list = new List<TElement>();
            var propertyList = typeof(TElement).GetProperties().Where(x => x.CanWrite);
            if (tb != null && tb.Columns.Count > 0 && tb.Rows.Count > 0)
            {
                for (int i = 0; i < tb.Rows.Count; i++)
                {
                    TElement element = new TElement();
                    var row = tb.Rows[i];
                    for (int j = 0; j < tb.Columns.Count; j++)
                    {
                        try
                        {
                            var columnName = tb.Columns[j].ColumnName;
                            var property =
                                propertyList.FirstOrDefault(
                                    x => string.Equals(columnName, x.Name, StringComparison.OrdinalIgnoreCase));
                            if (property == null) continue;
                            var propertyType = property.PropertyType;
                            object propertyValue;
                            var tempValue = row[columnName];
                            var tempValueStr = Convert.ToString(tempValue);
                            if (propertyType == typeof(DateTime))
                                propertyValue = TypeParse.StrToDateTime(tempValue, DateTime.Now);
                            else if (propertyType == typeof(DateTime?))
                            {
                                DateTime temp;
                                if (!string.IsNullOrEmpty(tempValueStr) && DateTime.TryParse(tempValueStr, out temp))
                                {
                                    propertyValue = (DateTime?)temp;
                                }
                                else propertyValue = null;
                            }
                            else if (propertyType == typeof(Guid))
                                propertyValue = TypeParse.StrToGuid(tempValue);
                            else if (propertyType == typeof(Guid?))
                            {
                                Guid temp;
                                if (!string.IsNullOrEmpty(tempValueStr) && Guid.TryParse(tempValueStr, out temp))
                                {
                                    propertyValue = (Guid?)temp;
                                }
                                else propertyValue = null;
                            }
                            else if (propertyType == typeof(Double?))
                            {
                                Double temp;
                                if (!string.IsNullOrEmpty(tempValueStr) && Double.TryParse(tempValueStr, out temp))
                                {
                                    propertyValue = (Double?)temp;
                                }
                                else propertyValue = null;
                            }
                            else if (propertyType == typeof(int?))
                            {
                                int temp;
                                if (!string.IsNullOrEmpty(tempValueStr) && int.TryParse(tempValueStr, out temp))
                                {
                                    propertyValue = (int?)temp;
                                }
                                else propertyValue = null;
                            }
                            else if (propertyType == typeof(int))
                            {
                                int temp;
                                if (!string.IsNullOrEmpty(tempValueStr) && int.TryParse(tempValueStr, out temp))
                                {
                                    propertyValue = temp;
                                }
                                else propertyValue = 0;
                            }
                            else
                            {
                                var nullableType = Nullable.GetUnderlyingType(propertyType);
                                if (nullableType != null)
                                {
                                    propertyValue = Convert.ChangeType(tempValueStr, nullableType);
                                }
                                else
                                {
                                    propertyValue = Convert.ChangeType(tempValueStr, propertyType);
                                }
                            }
                            property.SetValue(element, propertyValue, new object[] { });
                        }
                        catch (Exception e) { }
                    }
                    list.Add(element);
                }
            }
            return list;
        }

 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static bool HasAttributes(Type attributeType, Type descriptor)
        {
            return descriptor.GetCustomAttributes(attributeType, true).Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizeType"></param>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static bool HasAttributes(Type authorizeType, PropertyInfo descriptor)
        {
            return descriptor.GetCustomAttributes(authorizeType, true).Any();
        }

        /// <summary>
        /// 
        /// </summary> 
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(PropertyInfo descriptor) where TAttribute : class
        {
            if (descriptor.GetCustomAttributes(typeof(TAttribute), true).Any())
                return (TAttribute)descriptor.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault();
            return null;
        }/// <summary>
        /// 
        /// </summary> 
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(Type descriptor) where TAttribute : class
        {
            if (descriptor.GetCustomAttributes(typeof(TAttribute), true).Any())
                return (TAttribute)descriptor.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault();
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ParseConnectionString(string connectionStr)
        {
            Dictionary<string, string> dir = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(connectionStr)) return dir;
            var tempArr = connectionStr.ToLower().Split(';');
            foreach (var s in tempArr)
            {
                var temp2 = s.Split('=');
                if (temp2.Length == 2 && !string.IsNullOrEmpty(temp2[0]) && !string.IsNullOrEmpty(temp2[1]))
                {
                    var key = temp2[0].Trim();
                    if (!dir.ContainsKey(key))
                        dir.Add(key, temp2[1]);
                }
            }
            return dir;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string ToSqlConnectionString(IDictionary<string, string> dic)
        {
            StringBuilder str = new StringBuilder();
            foreach (var item in dic)
            {
                str.AppendFormat("{0}={1};", item.Key, item.Value);
            }
            return str.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddressOrHostName"></param>
        /// <param name="port"></param>
        /// <param name="database"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string FormatSqlServerConnectionString(string ipAddressOrHostName, int port, string database, string uid, string pwd)
        {
            var datasource = ipAddressOrHostName;
            if (port != 1433) datasource += "," + port;
            return string.Format("data source={0};initial catalog={1};user id={2};password={3};", datasource, database, uid, pwd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddressOrHostName"></param>
        /// <param name="port"></param>
        /// <param name="database"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string FormatMysqlEfConnectionString(string ipAddressOrHostName, int port, string database, string uid, string pwd)
        {
            var datasource = ipAddressOrHostName;
            if (port == 0) port = 3306;
            return string.Format("Data Source ={0};port ={1};Initial Catalog={2};user id={3};password={4};",
                datasource, port, database, uid, pwd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddressOrHostName"></param>
        /// <param name="port"></param>
        /// <param name="database"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string FormatMysqlConnectionString(string ipAddressOrHostName, int port, string database, string uid, string pwd)
        {
            return string.Format("server={0};port={1};Database={2};Uid={3};Pwd={4};CharSet=utf8;", ipAddressOrHostName, port, database, uid, pwd);
        }
    }
}
