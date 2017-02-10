using System;
using System.Linq;
using System.Linq.Expressions;

namespace Lfz.Data {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Orderable<T> {
        private IQueryable<T> _queryable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerable"></param>
        public Orderable(IQueryable<T> enumerable) {
            _queryable = enumerable;
        }

        /// <summary>
        /// 
        /// </summary>
        public IQueryable<T> Queryable {
            get { return _queryable; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keySelector"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public Orderable<T> Asc<TKey>(Expression<Func<T, TKey>> keySelector) {
            _queryable = _queryable
                .OrderBy(keySelector);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keySelector1"></param>
        /// <param name="keySelector2"></param>
        /// <typeparam name="TKey1"></typeparam>
        /// <typeparam name="TKey2"></typeparam>
        /// <returns></returns>
        public Orderable<T> Asc<TKey1, TKey2>(Expression<Func<T, TKey1>> keySelector1,
                                              Expression<Func<T, TKey2>> keySelector2) {
            _queryable = _queryable
                .OrderBy(keySelector1)
                .OrderBy(keySelector2);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keySelector1"></param>
        /// <param name="keySelector2"></param>
        /// <param name="keySelector3"></param>
        /// <typeparam name="TKey1"></typeparam>
        /// <typeparam name="TKey2"></typeparam>
        /// <typeparam name="TKey3"></typeparam>
        /// <returns></returns>
        public Orderable<T> Asc<TKey1, TKey2, TKey3>(Expression<Func<T, TKey1>> keySelector1,
                                                     Expression<Func<T, TKey2>> keySelector2,
                                                     Expression<Func<T, TKey3>> keySelector3) {
            _queryable = _queryable
                .OrderBy(keySelector1)
                .OrderBy(keySelector2)
                .OrderBy(keySelector3);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keySelector"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public Orderable<T> Desc<TKey>(Expression<Func<T, TKey>> keySelector) {
            _queryable = _queryable
                .OrderByDescending(keySelector);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keySelector1"></param>
        /// <param name="keySelector2"></param>
        /// <typeparam name="TKey1"></typeparam>
        /// <typeparam name="TKey2"></typeparam>
        /// <returns></returns>
        public Orderable<T> Desc<TKey1, TKey2>(Expression<Func<T, TKey1>> keySelector1,
                                               Expression<Func<T, TKey2>> keySelector2) {
            _queryable = _queryable
                .OrderByDescending(keySelector1)
                .OrderByDescending(keySelector2);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keySelector1"></param>
        /// <param name="keySelector2"></param>
        /// <param name="keySelector3"></param>
        /// <typeparam name="TKey1"></typeparam>
        /// <typeparam name="TKey2"></typeparam>
        /// <typeparam name="TKey3"></typeparam>
        /// <returns></returns>
        public Orderable<T> Desc<TKey1, TKey2, TKey3>(Expression<Func<T, TKey1>> keySelector1,
                                                      Expression<Func<T, TKey2>> keySelector2,
                                                      Expression<Func<T, TKey3>> keySelector3) {
            _queryable = _queryable
                .OrderByDescending(keySelector1)
                .OrderByDescending(keySelector2)
                .OrderByDescending(keySelector3);
            return this;
        }
    }
}