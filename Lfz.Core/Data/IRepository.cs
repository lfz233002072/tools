using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Lfz.Collections;
using Lfz.Data;

namespace Lfz.Data {
    /// <summary>
    /// 数据仓库
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        DbContext CurrentDbContext { get; }

        /// <summary>
        /// 整体提交,返回受影响行数。一般大于0表示操作成功
        /// </summary>
        int Commit();

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="entity"></param>
        void Create(T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity"></param>
        void CreateOrUpdate(Guid id, T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        void Delete(Guid id);
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Get(object id);

        /// <summary>
        /// 返回序列中的唯一元素；如果该序列为空，则返回默认值；如果该序列包含多个元素，那么返回第一个元素
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// 
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Exists(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> Fetch(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        IEnumerable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <param name="skip"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip, int count);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <param name="pageIndex">从0开始计数</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IPageOfItems<T> GetPaged(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int pageIndex,
                                 int pageSize);

        /// <summary>
        /// 执行原始sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        void ExecuteSql(string sql, params object[] parameters);
    }
}