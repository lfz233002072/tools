// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :IDatabaseInitialise.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-11 18:33
// *        https://git.oschina.net/lfz/tools
// *
// *======================================================================*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Lfz.Collections;
using Lfz.Utitlies;

namespace Lfz.Data.RawSql
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDatabaseInitialise
    {
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="tableList"></param>
        /// <returns></returns>
        bool Initialise(IEnumerable<Type> tableList);

        /// <summary>
        /// 创建表字段
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="colName"></param>
        /// <param name="newName"></param>
        /// <param name="newDataType"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool AddOrAlterColumn(string tablename, string colName, string newName, TemplateColumnDataType newDataType,
            int length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        bool CreateTable(string tablename, params string[] defaultColumns);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="colname"></param>
        /// <returns></returns>
        bool DropColumn(string tablename, string colname);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        bool DropTable(string tablename);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        bool CheckTableExists(string tablename);

        /// <summary>
        /// 获取当前表存在的字段
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        DataColumnCollection GetColumnList(string tablename);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="colName"></param>
        /// <param name="newDataType"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool AddColumn(string tablename, string colName, TemplateColumnDataType newDataType, int length);
    }

    /// <summary>
    /// 
    /// </summary>
    public class DatabaseInitialise : IDatabaseInitialise
    {
        private readonly IDbProviderConfig _providerConfig;
        private readonly IRawSqlSearchService _searchService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerConfig"></param>
        public DatabaseInitialise(IDbProviderConfig providerConfig)
        {
            _providerConfig = providerConfig;
            _searchService = new RawSqlSearchService(_providerConfig);
        }

        public bool Initialise(IEnumerable<Type> tableList)
        {
            foreach (var type in tableList)
            {
                CreateTable(type);
            }
            return true;
        }

        private bool CreateTable(Type type)
        {
            if (Utils.HasAttributes(typeof(NotMappedAttribute), type)) return true;
            var tableName = Utils.GetTableName(type);
            var propertyList = type.GetProperties().Where(x => x.CanWrite && x.CanRead);
            if (CheckTableExists(tableName))
            {
                var columnList = GetColumnList(tableName);
                if (columnList == null) return false;
                foreach (var propertyInfo in propertyList)
                {
                    if (Utils.HasAttributes(typeof(NotMappedAttribute), propertyInfo)) continue;
                    var columnInfoAttribute = Utils.GetAttribute<ColumnAttribute>(propertyInfo);
                    var columnLengthAttr = Utils.GetAttribute<StringLengthAttribute>(propertyInfo);
                    var columnName = GetColumnName(propertyInfo, columnInfoAttribute);
                    if (!columnList.Contains(columnName))
                    {
                        //如果旧字段不存，那么创建
                        AddColumn(tableName, columnName, GetDataType(propertyInfo, columnInfoAttribute), GetColumnLength(columnLengthAttr));
                    }
                }
            }
            else
            {
                CreateTable(tableName);
                var columnList = GetColumnList(tableName);
                foreach (var propertyInfo in propertyList)
                {
                    if (propertyInfo.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)
                       || Utils.HasAttributes(typeof(NotMappedAttribute), propertyInfo)) continue;
                    var columnInfoAttribute = Utils.GetAttribute<ColumnAttribute>(propertyInfo);
                    var columnLengthAttr = Utils.GetAttribute<StringLengthAttribute>(propertyInfo);
                    var columnName = GetColumnName(propertyInfo, columnInfoAttribute);
                    if (columnList==null ||  columnList.Contains(columnName))continue;
                    AddColumn(tableName, columnName, GetDataType(propertyInfo, columnInfoAttribute), GetColumnLength(columnLengthAttr));
                }
            }
            return true;
        }

        /// <summary>
        /// 创建一个只有一个Id主键的空表
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="defaultColumns"></param>
        /// <returns></returns>
        public bool CreateTable(string tablename, params string[] defaultColumns)
        {
            if (_providerConfig.DbProvider == DbProvider.MySql)
            {
                var sqlClause = string.Format(@"  CREATE TABLE IF NOT EXISTS `{0}` (`Id` int(11) NOT NULL AUTO_INCREMENT,PRIMARY KEY (`Id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;", tablename);
                DbHelperMySql.ExecuteSql(_providerConfig.DbConnectionString, sqlClause);
            }
            else
            {
                var sqlClause = string.Format(@"IF NOT EXISTS(SELECT * FROM sysobjects where id=OBJECT_ID('{0}') and xtype='U')
BEGIN CREATE TABLE {0} (
    [Id]          INT           NOT NULL IDENTITY, 
    PRIMARY KEY CLUSTERED ([Id] ASC));
END", tablename);
                DbHelperSql.ExecuteSql(_providerConfig.DbConnectionString, sqlClause);
            }
            foreach (var column in defaultColumns)
            {
                if (!string.IsNullOrEmpty(column) && !ColumHasExists(tablename, column))
                    AddColumn(tablename, column, TemplateColumnDataType.TemplateString, 255);
            }
            return true;
        }

        /// <summary>
        /// 创建一个只有一个Id主键的空表
        /// </summary> 
        /// <returns></returns>
        public bool AddOrAlterColumn(string tablename, string colName, string newName, TemplateColumnDataType newDataType, int length)
        {
            if (!ColumHasExists(tablename, colName))
            {
                //如果旧字段不存，那么创建
                if (string.IsNullOrEmpty(newName)) newName = colName;
                return AddColumn(tablename, newName, newDataType, length);
            }
            StringBuilder builder = new StringBuilder();
            if (_providerConfig.DbProvider == DbProvider.MySql)
            {
                builder.AppendFormat("ALTER TABLE `{0}`", tablename);
                if (!string.IsNullOrEmpty(newName) &&
                    !string.Equals(colName, newName, StringComparison.OrdinalIgnoreCase))
                {
                    builder.AppendFormat(" CHANGE COLUMN `{0}` `{1}`", colName, newName);
                }
                else
                {
                    builder.AppendFormat(" MODIFY COLUMN `{0}`", colName);
                }
                switch (newDataType)
                {
                    case TemplateColumnDataType.TemplateInt:
                        builder.Append(" int NULL default 0");
                        break;
                    case TemplateColumnDataType.TemplateDatetime:
                        builder.Append(" datetime NULL");
                        break;
                    case TemplateColumnDataType.TemplateDecimal:
                        builder.AppendFormat(" decimal({0},4) NULL", length);
                        break;
                    default:
                        if (length < 1) length = 255;
                        builder.AppendFormat(" varchar({0}) NULL DEFAULT ''", length);
                        break;
                }
                DbHelperMySql.ExecuteSql(_providerConfig.DbConnectionString, builder.ToString());
            }
            else
            {
                if (!string.IsNullOrEmpty(newName) &&
                    string.Equals(colName, newName, StringComparison.OrdinalIgnoreCase))
                {
                    var sqlClause =
                        string.Format(" EXECUTE sp_rename N'{0}.{1}', @newname = N'{2}', @objtype = N'COLUMN'", tablename, colName, newName);
                    DbHelperSql.ExecuteSql(_providerConfig.DbConnectionString, sqlClause);
                    colName = newName;
                }
                builder.AppendFormat("ALTER TABLE {0}", tablename);
                builder.AppendFormat(" ALTER COLUMN {0} ", colName);
                switch (newDataType)
                {
                    case TemplateColumnDataType.TemplateInt:
                        builder.Append(" INT NULL ");
                        break;
                    case TemplateColumnDataType.TemplateDatetime:
                        builder.Append(" datetime NULL ");
                        break;
                    case TemplateColumnDataType.TemplateDecimal:
                        builder.AppendFormat(" decimal({0},4) NULL ", length);
                        break;
                    case TemplateColumnDataType.TemplateText:
                        builder.Append(" ntext ");
                        break;
                    default:
                        if (length < 1) length = 255;
                        builder.AppendFormat(" nvarchar({0}) ", length);
                        break;
                }
                DbHelperSql.ExecuteSql(_providerConfig.DbConnectionString, builder.ToString());
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="colname"></param>
        /// <returns></returns>
        public bool DropColumn(string tablename, string colname)
        {
            if (!ColumHasExists(tablename, colname)) return true;
            var sqlClause = string.Format("alter table {0} drop column {1}", tablename, colname);
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
        /// <returns></returns>
        public bool DropTable(string tablename)
        {
            if (_providerConfig.DbProvider == DbProvider.MySql)
            {
                var sqlClause = string.Format("drop table IF EXISTS {0}", tablename);
                DbHelperMySql.ExecuteSql(_providerConfig.DbConnectionString, sqlClause);
            }
            else
            {
                var sqlClause = string.Format("if object_id('{0}') is not null drop table {0}", tablename);
                DbHelperSql.ExecuteSql(_providerConfig.DbConnectionString, sqlClause);
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public bool CheckTableExists(string tablename)
        {
            tablename = tablename.Replace("'", "''");
            if (_providerConfig.DbProvider == DbProvider.MySql)
            {
                var temp = Utils.ParseConnectionString(_providerConfig.DbConnectionString);
                var dbname = temp.GetValueByParams("database", "initial catalog");
                var sqlClause = string.Format("select count(*) from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA='{1}' and TABLE_NAME='{0}' ", tablename, dbname);
                return DbHelperMySql.Exists(_providerConfig.DbConnectionString, sqlClause);
            }
            else
            {
                var sqlClause = "SELECT  * FROM dbo.SysObjects WHERE ID = object_id(N'" + tablename + "') AND OBJECTPROPERTY(ID, 'IsTable') = 1";
                return TypeParse.StrToInt(DbHelperSql.GetSingle(_providerConfig.DbConnectionString, sqlClause)) != 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename"></param> 
        /// <returns></returns>
        public DataColumnCollection GetColumnList(string tablename)
        {
            string sqlClause = string.Format("select top 0 * from {0}", tablename);

            if (_providerConfig.DbProvider == DbProvider.MySql)
            {
                sqlClause = string.Format("select * from {0} LIMIT 0", tablename);
            }
            using (var ds = _searchService.Query(sqlClause))
            {
                if (ds == null || ds.Tables.Count == 0) return null;
                return ds.Tables[0].Columns;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="colName"></param>
        /// <param name="newDataType"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool AddColumn(string tablename, string colName, TemplateColumnDataType newDataType, int length)
        {
            StringBuilder builder = new StringBuilder();
            if (_providerConfig.DbProvider == DbProvider.MySql)
            {
                builder.AppendFormat("ALTER TABLE `{0}`", tablename);
                builder.AppendFormat(" ADD COLUMN `{0}` ", colName);
                switch (newDataType)
                {
                    case TemplateColumnDataType.TemplateInt:
                        builder.Append(" int NULL default 0");
                        break;
                    case TemplateColumnDataType.TemplateDatetime:
                        builder.Append(" datetime NULL");
                        break;
                    case TemplateColumnDataType.TemplateDecimal:
                        builder.AppendFormat(" decimal({0},4) NULL", 19);
                        break;
                    case TemplateColumnDataType.TemplateText:
                        builder.Append(" text ");
                        break;
                    default:
                        if (length < 1) length = 255;
                        builder.AppendFormat(" varchar({0}) NULL DEFAULT ''", length);
                        break;
                }
                DbHelperMySql.ExecuteSql(_providerConfig.DbConnectionString, builder.ToString());
            }
            else
            {
                builder.AppendFormat("ALTER TABLE {0}", tablename);
                builder.AppendFormat(" ADD {0} ", colName);
                switch (newDataType)
                {
                    case TemplateColumnDataType.TemplateInt:
                        builder.Append(" INT NULL default 0");
                        break;
                    case TemplateColumnDataType.TemplateDatetime:
                        builder.Append(" datetime NULL");
                        break;
                    case TemplateColumnDataType.TemplateDecimal:
                        builder.AppendFormat(" decimal({0},4) NULL", 19);
                        break;
                    case TemplateColumnDataType.TemplateText:
                        builder.Append(" ntext ");
                        break;
                    default:
                        if (length < 1) length = 255;
                        builder.AppendFormat(" nvarchar({0}) NULL DEFAULT ''", length);
                        break;
                }
                DbHelperSql.ExecuteSql(_providerConfig.DbConnectionString, builder.ToString());
            }
            return true;
        }
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        private bool ColumHasExists(string tablename, string colName)
        {
            string sqlClause = string.Format("select top 1 * from {0}", tablename);

            if (_providerConfig.DbProvider == DbProvider.MySql)
            {
                sqlClause = string.Format("select * from {0} LIMIT 1", tablename);
            }
            using (var ds = _searchService.Query(sqlClause))
            {
                if (ds == null || ds.Tables.Count == 0) return false;
                return ds.Tables[0].Columns.Contains(colName);
            }
        }



        private string GetColumnName(PropertyInfo propertyInfo, ColumnAttribute columnAttribute)
        {
            if (columnAttribute != null && !string.IsNullOrEmpty(columnAttribute.Name))
                return columnAttribute.Name;
            else return propertyInfo.Name;
        }

        private TemplateColumnDataType GetDataType(PropertyInfo propertyInfo, ColumnAttribute columnAttribute)
        {
            if (columnAttribute != null && !string.IsNullOrEmpty(columnAttribute.TypeName))
            {
                string typeName = "";
                typeName = columnAttribute.TypeName.ToLower();
                switch (typeName)
                {
                    case "nvarchar":
                    case "varchar":
                    case "char":
                        return TemplateColumnDataType.TemplateString;
                        break;
                }
            }
            if (propertyInfo.PropertyType == typeof(Int64) || propertyInfo.PropertyType == typeof(Int64?)
                || propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?)
                || propertyInfo.PropertyType == typeof(uint) || propertyInfo.PropertyType == typeof(uint?)
                || propertyInfo.PropertyType == typeof(short) || propertyInfo.PropertyType == typeof(short?)
                || propertyInfo.PropertyType == typeof(bool) || propertyInfo.PropertyType == typeof(bool?)
                || propertyInfo.PropertyType.IsEnum
                )
                return TemplateColumnDataType.TemplateInt;
            if (propertyInfo.PropertyType == typeof(decimal)
                || propertyInfo.PropertyType == typeof(decimal?)
                || propertyInfo.PropertyType == typeof(double)
                || propertyInfo.PropertyType == typeof(double?)
                || propertyInfo.PropertyType == typeof(float)
                || propertyInfo.PropertyType == typeof(float?)
                )
                return TemplateColumnDataType.TemplateDecimal;
            if (propertyInfo.PropertyType == typeof(DateTime)
                || propertyInfo.PropertyType == typeof(DateTime?)
                )
                return TemplateColumnDataType.TemplateDatetime;
            if (columnAttribute != null &&(
                 string.Equals(columnAttribute.TypeName, "text", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(columnAttribute.TypeName, "ntext", StringComparison.OrdinalIgnoreCase)))
                return TemplateColumnDataType.TemplateText;
            return TemplateColumnDataType.TemplateString;
        }

        private int GetColumnLength(StringLengthAttribute columnAttribute)
        {
            if (columnAttribute != null && (columnAttribute.MaximumLength > 0))
            {
                return columnAttribute.MaximumLength;
            }
            return 255;
        }

        #endregion

    }
}