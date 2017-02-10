// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :IRawSqlSearchService.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-04 16:14
// *        https://git.oschina.net/lfz/tools
// *
// *======================================================================*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Lfz.Collections;

namespace Lfz.Data.RawSql
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRawSqlSearchService
    {
        List<PropertyInfo> GetPropertyInfos(Type elementType);

        /// <summary>
        /// 
        /// </summary>
        IDbProviderConfig DbProviderConfig { get;  }

        /// <summary>
        /// 分页数据获取
        /// </summary> 
        /// <param name="sqlClause"></param>
        /// <param name="fields"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PageOfDatatable GetPaged(string tableName, string whereClause, string fields, string orderBy, int pageIndex, int pageSize, string primaryKey = "Id");

        DataRow GetByKey(string tablename, int id);
        DataRow GetModel(string tableName, string whereClause);
        DataRow GetModel(string sqlClause);

        bool Delete (string tablename, string whereClause);
        bool DeleteByKey(string tablename, int id);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="fields"></param>
        /// <param name="orderBy"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        DataTable GetList(string tableName, string whereClause, string fields, string orderBy, int count);

        /// <summary>
        /// 获取指定类型数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="whereClause"></param>
        /// <param name="fields"></param>
        /// <param name="orderBy"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<object> GetListWithSingleValue(string tableName, string whereClause, string fields, string orderBy, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlClause"></param>
        /// <returns></returns>
        object GetSingleObject(string sqlClause);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlClause"></param>
        /// <returns></returns>
        int GetSingle(string sqlClause);
        /// <summary>
        /// 记录条数获取
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        int Count(string tableName, string whereClause);

        /// <summary>
        /// 数据是否存在
        /// </summary>
        /// <param name="whereClause"></param> 
        /// <returns></returns>
        bool Exists(string tableName, string whereClause);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlClause"></param>
        void ExcuteSql(string sqlClause);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlClause"></param>
        DataSet Query(string sqlClause);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlClause"></param>
        /// <returns></returns>
        DataTable QueryTable(string sqlClause);

   
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        TElement Format<TElement>(DataRow row, DataColumnCollection columns = null) where TElement : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        IDictionary<string, object> GetDictionary<TElement>(TElement model) where TElement : class;
    }
}