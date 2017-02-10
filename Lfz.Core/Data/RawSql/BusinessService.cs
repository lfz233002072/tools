using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Lfz.Collections;

namespace Lfz.Data.RawSql
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BusinessService<TElement> : IBusinessService<TElement> where TElement : class ,new()
    {
        private readonly IRawSqlSearchService _searchService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerConfig"></param>
        protected BusinessService(IDbProviderConfig providerConfig)
        {
            _searchService = new RawSqlSearchService(providerConfig);
            _primatyKeyIsInt = null;
        }

        private bool? _primatyKeyIsInt;
        private bool IsIntPrimatyKey
        {
            get
            {
                if (_primatyKeyIsInt.HasValue) return _primatyKeyIsInt.Value;
                var propertyList = _searchService.GetPropertyInfos(typeof(TElement));
                var property =
                    propertyList.FirstOrDefault(
                        x => string.Equals(x.Name, PrimaryKey, StringComparison.OrdinalIgnoreCase));
                if (property != null &&
                    (property.PropertyType == typeof(int) || property.PropertyType == typeof(long)))
                {
                    _primatyKeyIsInt = true;
                }
                else _primatyKeyIsInt = false;
                return _primatyKeyIsInt.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string PrimaryKey
        {
            get { return "Id"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IRawSqlSearchService SearchService
        {
            get { return _searchService; }
        }


        /// <summary>
        /// 是否自动增长字段
        /// </summary>
        /// <returns></returns>
        public virtual bool IsIdentity()
        {
            return IsIntPrimatyKey;
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract string TableName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string BuildCreateSql(IDictionary<string, object> dic)
        {
            StringBuilder fieldStr = new StringBuilder("");
            StringBuilder valueStr = new StringBuilder("");
            bool isIdentity = IsIdentity();
            bool flag = false;
            foreach (var pair in dic)
            {
                if (isIdentity && string.Equals(pair.Key, PrimaryKey, StringComparison.OrdinalIgnoreCase))
                {
                    //整数类型主键，只能是自动增长
                    continue;
                }
                fieldStr.AppendFormat("{0}{1}", flag ? "," : "", pair.Key);
                valueStr.AppendFormat("{0}'{1}'", flag ? "," : "", pair.Value);
                flag = true;
            }
            var str = fieldStr.ToString();
            if (string.IsNullOrEmpty(str)) return string.Empty;
            if (isIdentity)
                return string.Format("INSERT {0}({1}) VALUES({2}); select @@IDENTITY;", TableName, str, valueStr);
            else
                return string.Format("INSERT {0}({1}) VALUES({2}); ", TableName, str, valueStr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public string BuildUpdateSql(IDictionary<string, object> dic, params string[] keys)
        {
            //如果没有自定义更新主键，那么使用表主键
            var keyList = new List<string>();
            if (keys != null && keys.Any())
                keyList.AddRange(keys);
            else keyList.Add(PrimaryKey);
            List<string> inValidList = new List<string>();
            foreach (var pair in dic)
            {
                if (pair.Value == null)
                    inValidList.Add(pair.Key);
            }
            foreach (var key in inValidList)
            {
                dic.Remove(key);
            }
            StringBuilder builder = new StringBuilder("update ");
            builder.AppendFormat("{0} SET ", TableName);
            bool hasSetClause = false;
            foreach (var pair in dic)
            {
                //除主键外，其它值都是可变的
                if (string.Equals(pair.Key, PrimaryKey, StringComparison.OrdinalIgnoreCase)
                     || pair.Value == null)
                    continue;
                builder.AppendFormat("{2}{0}='{1}'", pair.Key, pair.Value, hasSetClause ? "," : "");
                hasSetClause = true;
            }
            if (!hasSetClause) return string.Empty;
            hasSetClause = false;
            foreach (var key in keyList)
            {
                string tempkey = key;
                object value = null;
                if (dic.ContainsKey(tempkey))
                {
                    value = dic[tempkey];
                }
                else
                {
                    tempkey = dic.Keys.FirstOrDefault(x => string.Equals(x, tempkey, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrEmpty(tempkey)) value = dic[tempkey];
                }
                if (value != null)
                {
                    builder.AppendFormat("{2}{0}='{1}'", tempkey, value, hasSetClause ? " and " : " where ");
                    hasSetClause = true;
                }
            }
            if (!hasSetClause) return string.Empty;
            return builder.ToString();
        }


        public TElement Get(string whereClause)
        {
            var row = _searchService.GetModel(TableName, whereClause);
            return Format(row);
        }
         
        public bool Delete(int key)
        {
            var whereClause = string.Format("{0}={1}", PrimaryKey, key);
            return _searchService.Delete(TableName, whereClause);
        }

        public TElement Get(int key)
        { 
            var whereClause = string.Format("{0}={1}", PrimaryKey, key);
            return Get(whereClause);
        }
        public bool Delete(Guid key)
        {
            var whereClause = string.Format("{0}='{1}'", PrimaryKey, key);
            return _searchService.Delete(TableName, whereClause);
        }

        public TElement Get(Guid key)
        {
            var whereClause = string.Format("{0}='{1}'", PrimaryKey, key);
            return Get(whereClause);
        }


        public bool DeleteByKey(object key)
        {
            string whereClause;
            if (IsIdentity())
            {
                whereClause = string.Format("{0}={1}", PrimaryKey, key);
            }
            else
            {
                whereClause = string.Format("{0}='{1}'", PrimaryKey, key);
            }
            return _searchService.Delete(TableName, whereClause);
        }

        public TElement GetByKey(object key)
        {
            string whereClause;
            if (IsIdentity())
            {
                whereClause = string.Format("{0}={1}", PrimaryKey, key);
            }
            else
            {
                whereClause = string.Format("{0}='{1}'", PrimaryKey, key);
            }
            return Get(whereClause);
        }

        public void ExcuteSql(string sqlClause)
        {
            _searchService.ExcuteSql(sqlClause);
        }
        public bool Exists(string whereClause)
        {
            return _searchService.Exists(TableName, whereClause);
        }

        public virtual bool CreateOrUpdate(TElement model, params string[] keys)
        {
            var dic = ToSqlDictionary(GetDictionary(model));
            List<string> primaryKeyList = new List<string>();
            if (keys != null)
                primaryKeyList.AddRange(keys);
            if (keys == null || !keys.Any())
                primaryKeyList.Add(PrimaryKey);
            var whereClause = "";
            string sqlClause;
            foreach (var key in primaryKeyList)
            {
                var tempKey = key.ToLower();
                if (dic.ContainsKey(tempKey))
                    whereClause = string.Format("{0}{1}{2}='{3}'", whereClause, string.IsNullOrEmpty(whereClause) ? "" : " AND ", tempKey, dic[tempKey]);
            }
            if (string.IsNullOrEmpty(whereClause)) return false;
            if (Exists(whereClause))
            {
                sqlClause = BuildUpdateSql(dic, keys);
                if (string.IsNullOrEmpty(sqlClause)) return false;
                _searchService.ExcuteSql(sqlClause);
            }
            else
            {
                sqlClause = BuildCreateSql(dic);
                if (string.IsNullOrEmpty(sqlClause)) return false;


                if (IsIdentity())
                {
                    var propertyList = _searchService.GetPropertyInfos(typeof(TElement));
                    var property =
                        propertyList.FirstOrDefault(
                            x => string.Equals(x.Name, PrimaryKey, StringComparison.OrdinalIgnoreCase));
                    if (property != null)
                    {
                        var value = _searchService.GetSingle(sqlClause);
                        property.SetValue(model, value, new object[] { });
                    }
                }
                else
                {
                    _searchService.ExcuteSql(sqlClause);
                }
            }
            return true;
        }

        protected virtual IDictionary<string, object> ToSqlDictionary(IDictionary<string, object> dic)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            foreach (var o in dic)
            {
                var key = o.Key.ToLower();
                if (result.ContainsKey(key) || string.IsNullOrEmpty(o.Key) || o.Value == null) continue;
                result.Add(key, o.Value);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual bool Create(TElement model, params string[] keys)
        {
            var dic = ToSqlDictionary(GetDictionary(model));
            List<string> primaryKeyList = new List<string>();
            if (keys != null)
                primaryKeyList.AddRange(keys);
            if (keys == null || !keys.Any())
                primaryKeyList.Add(PrimaryKey);
            var whereClause = "";
            string sqlClause;
            foreach (var key in primaryKeyList)
            {
                var tempKey = key.ToLower();
                if (dic.ContainsKey(tempKey))
                    whereClause = string.Format("{0}{1}{2}='{3}'", whereClause, string.IsNullOrEmpty(whereClause) ? "" : " AND ", tempKey, dic[tempKey]);
            }
            if (string.IsNullOrEmpty(whereClause)) return false;

            sqlClause = BuildCreateSql(dic);
            if (string.IsNullOrEmpty(sqlClause)) return false;

            var propertyList = _searchService.GetPropertyInfos(typeof(TElement));
            var property =
                propertyList.FirstOrDefault(
                    x => string.Equals(x.Name, PrimaryKey, StringComparison.OrdinalIgnoreCase));
            if (property != null && property.PropertyType == typeof(int))
            {
                var value = _searchService.GetSingle(sqlClause);
                property.SetValue(model, value, new object[] { });
            }
            else
            {
                _searchService.ExcuteSql(sqlClause);
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual bool Update(TElement model, params string[] keys)
        {
            var dic = ToSqlDictionary(GetDictionary(model));
            List<string> primaryKeyList = new List<string>();
            if (keys != null)
                primaryKeyList.AddRange(keys);
            if (keys == null || !keys.Any())
                primaryKeyList.Add(PrimaryKey);
            var whereClause = "";
            string sqlClause;
            foreach (var key in primaryKeyList)
            {
                var tempKey = key.ToLower();
                if (dic.ContainsKey(tempKey))
                    whereClause = string.Format("{0}{1}{2}='{3}'", whereClause, string.IsNullOrEmpty(whereClause) ? "" : " AND ", tempKey, dic[tempKey]);
            }
            if (string.IsNullOrEmpty(whereClause)) return false;

            sqlClause = BuildUpdateSql(dic, keys);
            if (string.IsNullOrEmpty(sqlClause)) return false;
            _searchService.ExcuteSql(sqlClause);

            return true;
        }


        public List<TElement> GetList(string whereClause, string fields, string orderBy, int count)
        {
            List<TElement> result = new List<TElement>();
            using (var dt = _searchService.GetList(TableName, whereClause, fields, orderBy, count))
            {
                if (dt == null) return result;
                foreach (DataRow dataRow in dt.Rows)
                {
                    var item = Format(dataRow, dt.Columns);
                    result.Add(item);
                }
            }
            return result;
        }

        public IPageOfItems<TElement> GetPaged(string whereClause, string fields, string orderBy, int pageIndex, int pageSize)
        {
            var dt = _searchService.GetPaged(TableName, whereClause, fields, orderBy, pageIndex, pageSize, PrimaryKey);
            if (dt == null) return new PageOfItems<TElement>() { PageIndex = pageIndex, PageSize = pageSize }; ;
            var result = new PageOfItems<TElement>()
            {
                PageIndex = dt.PageIndex,
                PageSize = dt.PageSize,
                TotalItemCount = dt.TotalItemCount,
            };
            foreach (DataRow dataRow in dt.Data.Rows)
            {
                var item = Format(dataRow, dt.Data.Columns);
                result.Add(item);
            }
            return result;
        }

        public virtual TElement Format(DataRow row, DataColumnCollection columns = null)
        {
            return _searchService.Format<TElement>(row, columns);
        }

        public virtual IDictionary<string, object> GetDictionary(TElement model)
        {
            return _searchService.GetDictionary(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public virtual IDictionary<string, object> ToDictionary(DataRow row)
        {
            if (row == null) return new Dictionary<string, object>();
            var columns = row.Table.Columns;
            return columns.Cast<DataColumn>().ToDictionary(dataColumn => dataColumn.ColumnName, dataColumn => row[dataColumn.ColumnName]);
        }


    }
}