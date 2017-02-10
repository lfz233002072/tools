using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Lfz.Collections;
using Lfz.Logging;
using Lfz.Utitlies;

namespace Lfz.Data.RawSql
{
    /// <summary>
    /// 
    /// </summary>
    public class RawSqlSearchService : IRawSqlSearchService
    {
        private readonly IDbProviderConfig _providerConfig;
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> PropertyDictionary = new ConcurrentDictionary<Type, List<PropertyInfo>>();
        protected ILogger Logger;

        /// <summary>
        /// 
        /// </summary>
        public IDbProviderConfig DbProviderConfig { get { return _providerConfig; } }

        /// <summary>
        /// 
        /// </summary>
        public RawSqlSearchService(IDbProviderConfig providerConfig)
        {
            _providerConfig = providerConfig;
            Logger = LoggerFactory.GetLog();
        }

        public PageOfDatatable GetPaged(string tableName, string whereClause, string fields, string orderBy,
            int pageIndex, int pageSize, string primaryKey = "Id")
        {
            if (_providerConfig.DbProvider == DbProvider.MySql)
            {
                if (!string.IsNullOrEmpty(whereClause) && !whereClause.TrimStart().ToLower().StartsWith("where ")) whereClause = " where " + whereClause;
                var totalCount = Count(tableName, whereClause);
                PageOfDatatable pageOfDatatable = new PageOfDatatable()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalItemCount = totalCount
                };
                if (!string.IsNullOrEmpty(orderBy)) orderBy = " order by " + orderBy;
                if (string.IsNullOrEmpty(fields)) fields = "*";
                var sqlClause = string.Format("SELECT {2} FROM {0} {1} {3} limit {4},{5}",
                    tableName, whereClause, fields, orderBy, pageOfDatatable.StartIndex, pageOfDatatable.PageSize);
                using (var ds = DbHelperMySql.Query(_providerConfig.DbConnectionString, sqlClause))
                {
                    pageOfDatatable.Data = ds.Tables[0];
                }
                return pageOfDatatable;
            }
            else
            {
                var parameters = new[]
                {
                    new SqlParameter("@TableName", SqlDbType.NVarChar, 255),
                    new SqlParameter("@Fields", SqlDbType.NVarChar, 255),
                    new SqlParameter("@WhereClause", SqlDbType.NVarChar, 2000),
                    new SqlParameter("@OrderBy", SqlDbType.NVarChar, 255),
                    new SqlParameter("@PrimaryKey", SqlDbType.NVarChar, 255),
                    new SqlParameter("@PageIndex", SqlDbType.Int, 4),
                    new SqlParameter("@PageSize", SqlDbType.Int, 4),
                };
                parameters[0].Value = tableName;
                parameters[1].Value = string.IsNullOrEmpty(fields) ? "*" : fields;
                parameters[2].Value = whereClause ?? string.Empty;
                parameters[3].Value = orderBy ?? string.Empty;
                parameters[4].Value = primaryKey;
                parameters[5].Value = pageIndex;
                parameters[6].Value = pageSize;
                using (
                    var reader = DbHelperSql.RunProcedure(_providerConfig.DbConnectionString, "up_GetPaged", parameters)
                    )
                {
                    PageOfDatatable pageOfDatatable = new PageOfDatatable()
                    {
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                    };
                    if (reader.Tables.Count >= 2 && reader.Tables[1].Rows.Count > 0)
                    {
                        var row = reader.Tables[1].Rows[0];
                        if (row["TotalRowCount"] == DBNull.Value) pageOfDatatable.TotalItemCount = 0;
                        else pageOfDatatable.TotalItemCount = (Int32)row["TotalRowCount"];
                    }
                    pageOfDatatable.Data = reader.Tables[0];
                    return pageOfDatatable;
                }
            }
        }

        public DataRow GetModel(string tableName, string whereClause)
        {
            var table = GetList(tableName, whereClause, "*", string.Empty, 1);
            if (table != null && table.Rows.Count > 0)
                return table.Rows[0];
            else return null;
        }

        public DataRow GetModel(string sqlClause)
        {
            using (var ds = Query(sqlClause))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    return ds.Tables[0].Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="whereClause"></param>
        /// <param name="fields"></param>
        /// <param name="orderBy">不包含order by 关键字</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DataTable GetList(string tableName, string whereClause, string fields, string orderBy, int count)
        {
            if (count < 0) count = 1;
            if (!string.IsNullOrEmpty(whereClause) && !whereClause.TrimStart().ToLower().StartsWith("where ")) whereClause = " where " + whereClause;
            if (!string.IsNullOrEmpty(orderBy)) orderBy = " order by " + orderBy;
            if (string.IsNullOrEmpty(fields)) fields = "*";
            string sqlClause = "";
            if (_providerConfig.DbProvider == DbProvider.MySql)
                sqlClause = string.Format("SELECT {0} FROM {1} {2} {3} LIMIT {4}", fields, tableName, whereClause,
                    orderBy, count);
            else
                sqlClause = string.Format("SELECT top {4} {0} FROM {1} {2} {3}", fields, tableName, whereClause,
                    orderBy, count);
            using (var ds =
                _providerConfig.DbProvider == DbProvider.MySql
                    ? DbHelperMySql.Query(_providerConfig.DbConnectionString, sqlClause)
                    : DbHelperSql.Query(_providerConfig.DbConnectionString, sqlClause))
            {
                if (ds != null) return ds.Tables[0];
            }
            return null;
        }

        public List<object> GetListWithSingleValue(string tableName, string whereClause, string fields, string orderBy, int count)
        {
            var dt = GetList(tableName, whereClause, fields, orderBy, count);
            if (dt == null || dt.Rows.Count == 0 || dt.Columns.Count == 0) return new List<object>();
            var list = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(row[0]);
            }
            return list;
        }

        public int Count(string tableName, string whereClause)
        {
            tableName = tableName.Replace("'", "''");
            if (!string.IsNullOrEmpty(whereClause) && !whereClause.TrimStart().ToLower().StartsWith("where ")) whereClause = " where " + whereClause;
            string sqlClause = string.Format("SELECT Count(*) FROM {0} {1}", tableName, whereClause);
            var result =
                _providerConfig.DbProvider == DbProvider.MySql
                    ? DbHelperMySql.GetSingle(_providerConfig.DbConnectionString, sqlClause)
                    : DbHelperSql.GetSingle(_providerConfig.DbConnectionString, sqlClause);
            return TypeParse.StrToInt(result);
        }

        public int GetSingle(string sqlClause)
        {
            var result =
                _providerConfig.DbProvider == DbProvider.MySql
                    ? DbHelperMySql.GetSingle(_providerConfig.DbConnectionString, sqlClause)
                    : DbHelperSql.GetSingle(_providerConfig.DbConnectionString, sqlClause);
            return TypeParse.StrToInt(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlClause"></param>
        /// <returns></returns>
        public object GetSingleObject(string sqlClause)
        {
            var result =
                _providerConfig.DbProvider == DbProvider.MySql
                    ? DbHelperMySql.GetSingle(_providerConfig.DbConnectionString, sqlClause)
                    : DbHelperSql.GetSingle(_providerConfig.DbConnectionString, sqlClause);
            return result;
        }


        public bool Exists(string tableName, string whereClause)
        {
            var count = Count(tableName, whereClause);
            return count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlClause"></param>
        public void ExcuteSql(string sqlClause)
        {
            if (_providerConfig.DbProvider == DbProvider.MySql)
                DbHelperMySql.ExecuteSql(_providerConfig.DbConnectionString, sqlClause);
            else DbHelperSql.ExecuteSql(_providerConfig.DbConnectionString, sqlClause);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlClause"></param>
        public DataSet Query(string sqlClause)
        {
            if (_providerConfig.DbProvider == DbProvider.MySql)
                return DbHelperMySql.Query(_providerConfig.DbConnectionString, sqlClause);
            else
                return DbHelperSql.Query(_providerConfig.DbConnectionString, sqlClause);
        }

        public DataTable QueryTable(string sqlClause)
        {
            using (var ds = Query(sqlClause))
            {
                if (ds != null && ds.Tables.Count > 0)
                    return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteByKey(string tablename, int id)
        {
            var sqlClause = string.Format("delete from {0} where id= {1}", tablename, id);
            if (_providerConfig.DbProvider == DbProvider.MySql)
            {
                DbHelperMySql.ExecuteSql(_providerConfig.DbConnectionString, sqlClause);
            }
            else
                DbHelperSql.ExecuteSql(_providerConfig.DbConnectionString, sqlClause);
            return true;
        }

        public bool Delete(string tablename, string whereClause)
        {
            if (string.IsNullOrEmpty(whereClause)) return false;
            if (!string.IsNullOrEmpty(whereClause) && !whereClause.TrimStart().ToLower().StartsWith("where ")) whereClause = " where " + whereClause;
            var sqlClause = string.Format("delete from {0} {1}", tablename, whereClause);
            if (_providerConfig.DbProvider == DbProvider.MySql)
            {
                DbHelperMySql.ExecuteSql(_providerConfig.DbConnectionString, sqlClause);
            }
            else
                DbHelperSql.ExecuteSql(_providerConfig.DbConnectionString, sqlClause);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataRow GetByKey(string tablename, int id)
        {
            return GetModel(tablename, "id=" + id);
        }

        public List<PropertyInfo> GetPropertyInfos(Type elementType)
        {
            List<PropertyInfo> propertyList;
            PropertyDictionary.TryGetValue(elementType, out propertyList);
            if (propertyList == null || propertyList.Count == 0)
            {
                propertyList = elementType.GetProperties().Where(x => x.CanWrite && x.CanRead).ToList();
                PropertyDictionary.AddOrUpdate(elementType, propertyList, (x, y) => propertyList);
            }
            return propertyList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public virtual TElement Format<TElement>(DataRow row, DataColumnCollection columns = null) where TElement : class ,new()
        {
            if (row == null) return null;
            if (columns == null)
                columns = row.Table.Columns;
            TElement element = new TElement();
            List<PropertyInfo> propertyList = GetPropertyInfos(typeof(TElement));
            foreach (DataColumn column in columns)
            {
                var columnName = column.ColumnName;
                object value = null;
                var property = propertyList.FirstOrDefault(x => string.Equals(x.Name, column.ColumnName, StringComparison.OrdinalIgnoreCase));
                if (property == null) continue;
                try
                {
                    if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                        value = TypeParse.StrToInt(row[columnName]);
                    else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                        value = TypeParse.StrToDecimal(row[columnName], 0);
                    else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
                        value = TypeParse.StrToDecimal(row[columnName], 0);
                    else if (property.PropertyType == typeof(float) || property.PropertyType == typeof(float?))
                        value = TypeParse.StrToDecimal(row[columnName], 0);
                    else if (property.PropertyType == typeof(Int64) || property.PropertyType == typeof(Int64?))
                        value = TypeParse.StrToInt64(row[columnName], 0);
                    else if (property.PropertyType == typeof(short) || property.PropertyType == typeof(short?))
                        value = TypeParse.StrToInt(row[columnName], 0);
                    else if (property.PropertyType == typeof(DateTime))
                        value = TypeParse.StrToDateTime(row[columnName]);
                    else if (property.PropertyType == typeof(DateTime?))
                        value = TypeParse.ConvertToNullableDateTime(row[columnName]);
                    else if (property.PropertyType == typeof(string))
                        value = Convert.ToString(row[columnName]);
                    else if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                        value = TypeParse.StrToGuid(row[columnName]);
                    else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                        value = TypeParse.StrToBool(row[columnName]);
                    else if (property.PropertyType == typeof(byte) || property.PropertyType == typeof(byte?))
                        value = (byte)TypeParse.StrToInt(row[columnName]);
                    else
                    {
                        // Logger.Trace(string.Format("Fromat 错误：{0} 列值类型无效 {1}", property.Name, property.PropertyType));
                        continue;
                    }
                    property.SetValue(element, value, new object[] { });
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Fromat");
                }
            }
            return element;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual IDictionary<string, object> GetDictionary<TElement>(TElement model) where TElement : class
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            if (model == null) return result;
            List<PropertyInfo> propertyList = GetPropertyInfos(typeof(TElement));
            foreach (var property in propertyList)
            {
                var key = property.Name.ToLower();
                if (!result.ContainsKey(key)
                    && !Utils.HasAttributes(typeof(NotMappedAttribute), property))
                {
                    if (property.PropertyType.IsEnum)
                    {
                        var value = property.GetValue(model, new object[] { });
                        if (value != null)
                            result.Add(key, (int)value);
                    }
                    else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                    {
                        var value = property.GetValue(model, new object[] { });
                        if (value == null)
                            result.Add(key, 0);
                        else
                        {
                            result.Add(key, TypeParse.StrToBool(value) ? 1 : 0);
                        }
                    }
                    else
                        result.Add(key, property.GetValue(model, new object[] { }));
                }
            }
            return result;
        }

    }
}