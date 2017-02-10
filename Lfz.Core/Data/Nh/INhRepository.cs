using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lfz.Collections;
using NHibernate;

namespace Lfz.Data.Nh
{
    public interface INhRepository<T>
    {
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="entity"></param>
        void Create(T entity);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity); 

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="id"></param>
        void Delete(int id);

        /// <summary>
        /// 复制克隆数据
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        void Copy(T source, T target);

        /// <summary>
        /// 更新缓存
        /// </summary>
        void Flush();

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Get(int id);

        /// <summary>
        /// 根据条件获取数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// IQueryable 数据表
        /// </summary>
        IQueryable<T> Table { get; }
       
        /// <summary> 
        /// 调用储存过程入库 
        /// </summary>
        /// <param name="queryName">The query can be either in HQL or SQL format.</param>
        /// <returns></returns>
        IQuery GetNamedQuery(string queryName);

        /// <summary>
        /// 数据数量
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 返回一个的实体记录列表
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> Fetch(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 返回一个的实体记录列表
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        IEnumerable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order);

        /// <summary>
        /// 返回一个的实体记录列表
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

        bool Exists(Expression<Func<T, bool>> func);
    }
}