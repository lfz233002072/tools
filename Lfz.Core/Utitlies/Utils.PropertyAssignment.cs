using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lfz.Models;

namespace Lfz.Utitlies
{
    public static partial class Utils
    {

        #region 获取相同类型属性值不一致的【属性名称-属性新旧值列表】

        /// <summary>
        /// 获取相同类型属性值不一致的【属性名称-属性新旧值列表】 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="oldObj"></param>
        /// <param name="newObj"></param>
        /// <param name="datetimeFields">时间控件列表</param>
        /// <param name="searchFields">需要查询的属性字段</param>
        /// <param name="exceptSearchFields">需要排除的属性字段</param>
        /// <param name="shouldAddFields">必须包含的属性字段</param>
        /// <returns></returns>
        public static IEnumerable<PropertyTrackInfo> GetDiffValue<TElement>(
            TElement oldObj, TElement newObj,
            out List<string> datetimeFields,
            IEnumerable<string> searchFields = null,
            IEnumerable<string> exceptSearchFields = null,
            IEnumerable<string> shouldAddFields = null)
            where TElement : class
        {
            datetimeFields = new List<string>();
            if (oldObj == null && newObj == null) return Enumerable.Empty<PropertyTrackInfo>();
            if (shouldAddFields == null) shouldAddFields = new List<string>();
            var propertyList = typeof(TElement).GetProperties();
            var resultFields = new List<PropertyInfo>();
            var shouldAddFieldList = shouldAddFields.ToList();
            if (oldObj != null && newObj != null)
            {
                #region 如果两种都不为空，那么为修改数据。修改数据时只对比需要修改的字段

                //如果该列表不为空，那么只限定这些数据
                var enumerable = searchFields == null ? new List<string>() : searchFields.ToList();
                if (searchFields != null && enumerable.Any())
                {
                    resultFields.AddRange(
                         propertyList.Where(
                             x => enumerable.Any(searchProperty => string.Equals(searchProperty, x.Name))));
                }
                else
                {
                    resultFields.AddRange(propertyList);
                }
                //待排除字段(从前面结果中提取)
                if (exceptSearchFields != null)
                {
                    resultFields =
                        resultFields.Where(
                            x => exceptSearchFields.All(
                                searchProperty => !string.Equals(searchProperty, x.Name, StringComparison.OrdinalIgnoreCase)))
                                    .ToList();
                }
                //从属性列表中提取
                if (shouldAddFieldList.Any())
                {
                    var tempList =
                         propertyList.Where(
                             x => shouldAddFieldList.Any(searchProperty => string.Equals(searchProperty, x.Name))).ToList();
                    if (tempList.Any())
                    {
                        resultFields = resultFields.Concat(tempList).ToList();
                    }
                }

                #endregion
            }
            else
            {
                //添加或删除时，将所有字段都需要显示   
            }

            var list = new List<PropertyTrackInfo>();
            foreach (var property in resultFields)
            {
                var propertyType = property.PropertyType;
                if ((propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                    && !property.Name.Equals("LastUpdateTime", StringComparison.OrdinalIgnoreCase))
                {
                    datetimeFields.Add(property.Name);
                }
                var shoudContain = shouldAddFieldList.Any(searchProperty => string.Equals(searchProperty, property.Name));
                var oldValue = oldObj == null ? "" : property.GetValue(oldObj, null);
                var newValue = newObj == null ? "" : property.GetValue(newObj, null);
                var oldValueStr = oldValue != null ? oldValue.ToString() : string.Empty;
                var newValueStr = newValue != null ? newValue.ToString() : string.Empty;
                //如果是变更或添加/删除，那么该属性添加到列表
                if (!string.Equals(oldValueStr, newValueStr, StringComparison.OrdinalIgnoreCase)
                    || shoudContain || (oldObj == null || newObj == null))
                    list.Add(new PropertyTrackInfo()
                    {
                        PropertyName = property.Name,
                        NewValue = newValueStr,
                        OldValue = oldValueStr,
                        Status = !string.Equals(oldValueStr, newValueStr, StringComparison.OrdinalIgnoreCase) ? PropertyTrackStatus.Modified :
                            (oldObj == null && newObj != null ? PropertyTrackStatus.Added :
                            (newObj == null && oldObj != null ? PropertyTrackStatus.Deleted : PropertyTrackStatus.Modified))
                    });
            }
            return list;
        }

        /// <summary>
        /// 获取相同类型属性值不一致的【属性名称-属性新旧值列表】 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="oldObj"></param>
        /// <param name="newObj"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyTrackInfo> GetDiffValue<TElement>(
            TElement oldObj, TElement newObj)
            where TElement : class
        {
            var propertyList = typeof(TElement).GetProperties();
            var list = new List<PropertyTrackInfo>();
            foreach (var property in propertyList)
            {
                var oldValue = oldObj == null ? "" : property.GetValue(oldObj, null);
                var newValue = newObj == null ? "" : property.GetValue(newObj, null);
                var oldValueStr = oldValue != null ? oldValue.ToString() : string.Empty;
                var newValueStr = newValue != null ? newValue.ToString() : string.Empty;
                //如果是变更或添加/删除，那么该属性添加到列表 
                list.Add(new PropertyTrackInfo()
                {
                    PropertyName = property.Name,
                    NewValue = newValueStr,
                    OldValue = oldValueStr,
                    Status = !string.Equals(oldValueStr, newValueStr, StringComparison.OrdinalIgnoreCase) ? PropertyTrackStatus.Modified :
                        (oldObj == null && newObj != null ? PropertyTrackStatus.Added :
                        (newObj == null && oldObj != null ? PropertyTrackStatus.Deleted : PropertyTrackStatus.Unchanged))
                });
            }
            return list;
        }

        /// <summary>
        /// 获取数据对比列表
        /// </summary> 
        /// <param name="oldObj">旧数据对象，要求实体类型完成一样</param>
        /// <param name="newObj">新数据对象，要求实体类型完成一样</param>
        /// <returns></returns>
        public static IEnumerable<PropertyTrackInfo> GetCompareValue(
            object oldObj, object newObj)
        {
            if (oldObj == null && newObj == null) return Enumerable.Empty<PropertyTrackInfo>();
            var objType = oldObj == null ? newObj.GetType() : oldObj.GetType();
            //要求实体类型完成一样
            if (oldObj != null && newObj != null && oldObj.GetType() != newObj.GetType()) return Enumerable.Empty<PropertyTrackInfo>();
            var propertyList = objType.GetProperties();
            var list = new List<PropertyTrackInfo>();
            foreach (var property in propertyList)
            {
                var oldValue = oldObj == null ? "" : property.GetValue(oldObj, null);
                var newValue = newObj == null ? "" : property.GetValue(newObj, null);
                var oldValueStr = oldValue != null ? oldValue.ToString() : string.Empty;
                var newValueStr = newValue != null ? newValue.ToString() : string.Empty;
                //如果是变更或添加/删除，那么该属性添加到列表 
                list.Add(new PropertyTrackInfo()
                {
                    PropertyName = property.Name,
                    NewValue = newValueStr,
                    OldValue = oldValueStr,
                    Status = !string.Equals(oldValueStr, newValueStr, StringComparison.OrdinalIgnoreCase) ? PropertyTrackStatus.Modified :
                        (oldObj == null && newObj != null ? PropertyTrackStatus.Added :
                        (newObj == null && oldObj != null ? PropertyTrackStatus.Deleted : PropertyTrackStatus.Unchanged))
                });
            }
            return list;
        }

        #endregion

        #region 根据属性名称-值列表设置对象值

        /// <summary> 
        /// 根据属性名称-值列表设置对象值
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="obj"></param> 
        /// <param name="propertyNameValue"></param>
        /// <returns></returns>
        public static void SetValue<TElement>(TElement obj, IEnumerable<KeyValuePair<string, string>> propertyNameValue)
            where TElement : class
        {
            if (obj == null) return;
            var elementType = typeof(TElement);
            foreach (var item in propertyNameValue)
            {
                try
                {
                    if (item.Value == null) continue;
                    var property = elementType.GetProperty(item.Key);
                    if (property == null) continue;
                    var propertyType = property.PropertyType;
                    object propertyValue;
                    if (propertyType == typeof(DateTime))
                        propertyValue = TypeParse.StrToDateTime(item.Value, DateTime.Now);
                    else if (propertyType == typeof(DateTime?))
                    {
                        DateTime temp;
                        if (!string.IsNullOrEmpty(item.Value) && DateTime.TryParse(item.Value, out temp))
                        {
                            propertyValue = (DateTime?)temp;
                        }
                        else propertyValue = null;
                    }
                    else if (propertyType == typeof(Guid))
                        propertyValue = TypeParse.StrToGuid(item.Value);
                    else if (propertyType == typeof(Guid?))
                    {
                        Guid temp;
                        if (!string.IsNullOrEmpty(item.Value) && Guid.TryParse(item.Value, out temp))
                        {
                            propertyValue = (Guid?)temp;
                        }
                        else propertyValue = null;
                    }
                    else if (propertyType == typeof(Double?))
                    {
                        Double temp;
                        if (!string.IsNullOrEmpty(item.Value) && Double.TryParse(item.Value, out temp))
                        {
                            propertyValue = (Double?)temp;
                        }
                        else propertyValue = null;
                    }
                    else if (propertyType == typeof(int?))
                    {
                        int temp;
                        if (!string.IsNullOrEmpty(item.Value) && int.TryParse(item.Value, out temp))
                        {
                            propertyValue = (int?)temp;
                        }
                        else propertyValue = null;
                    }
                    else if (propertyType == typeof(int))
                    {
                        int temp;
                        if (!string.IsNullOrEmpty(item.Value) && int.TryParse(item.Value, out temp))
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
                            propertyValue = Convert.ChangeType(item.Value, nullableType);
                        }
                        else
                        {
                            propertyValue = Convert.ChangeType(item.Value, propertyType);
                        }
                    }
                    property.SetValue(obj, propertyValue, new object[] { });
                }
                catch (Exception e)
                {

                }
            }
        }


        #endregion
    }

}
