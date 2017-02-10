using System;
using System.Collections.Generic;
using System.Data;
using Lfz.Collections;

namespace Lfz.Data.RawSql
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBusinessService<TElement> where TElement : class ,new()
    {
        string PrimaryKey { get; }

        IRawSqlSearchService SearchService { get; }

        string TableName { get; }

        string BuildCreateSql(IDictionary<string, object> dic);

        string BuildUpdateSql(IDictionary<string, object> dic, params string[] keys);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        TElement GetByKey(object key);

        bool Delete(int key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TElement Get(int key);

        bool Delete(Guid key);

        TElement Get(Guid key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        TElement Get(string  whereClause);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool DeleteByKey(object key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        bool Exists(string whereClause);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlClause"></param>
        void ExcuteSql(string sqlClause);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <returns></returns>
        bool CreateOrUpdate(TElement model, params string[] keys);
        bool Create (TElement model, params string[] keys);


        bool Update(TElement model, params string[] keys);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="fields"></param>
        /// <param name="orderBy"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<TElement> GetList(string whereClause, string fields, string orderBy, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        IDictionary<string, object> ToDictionary(DataRow row);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="fields"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param> 
        /// <returns></returns>
        IPageOfItems<TElement> GetPaged(string whereClause, string fields, string orderBy, int pageIndex, int pageSize);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        TElement Format(DataRow row, DataColumnCollection columns = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        IDictionary<string, object> GetDictionary(TElement model);
    }
}