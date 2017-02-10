using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lfz.Collections;
using Lfz.Data.RawSql;
using Lfz.Logging;
using NHibernate;
using NHibernate.Linq;

namespace Lfz.Data.Nh
{
    public partial class NhRepository<T> : INhRepository<T> where T : class
    {
        private readonly IDbProviderConfig _config;
        private readonly ISessionFactoryHolder _sessionFactory;
        private ISession _session;
 

        ~NhRepository()
        {
            if (_session != null)
                _session.Dispose();
        }

        public ILogger Logger { get; set; }

        protected virtual ISession Session
        {
            get
            {
                if (_session == null)
                {
                    var sessionFactory = _sessionFactory.GetSessionFactory(_config);
                    Logger.Information("Openning database session");
                    _session = sessionFactory.OpenSession(new SessionInterceptor());
                }
                return _session;
            }
        }

        public virtual IQueryable<T> Table
        {
            get { return Session.Query<T>().Cacheable(); }
        }

        #region IRepository<T> Members

        void INhRepository<T>.Create(T entity)
        {
            Create(entity);
        }

        void INhRepository<T>.Update(T entity)
        {
            Update(entity);
        }

        void INhRepository<T>.Delete(T entity)
        {
            Delete(entity);
        }

        public void Delete(int id)
        {
            Delete(Get(id));
        }

        void INhRepository<T>.Copy(T source, T target)
        {
            Copy(source, target);
        }

        void INhRepository<T>.Flush()
        {
            Flush();
        }

        T INhRepository<T>.Get(int id)
        {
            return Get(id);
        }

        T INhRepository<T>.Get(Expression<Func<T, bool>> predicate)
        {
            return Get(predicate);
        }

        IQueryable<T> INhRepository<T>.Table
        {
            get { return Table; }
        }

        public IQuery GetNamedQuery(string queryName)
        {
            return Session.GetNamedQuery(queryName);
        }

        int INhRepository<T>.Count(Expression<Func<T, bool>> predicate)
        {
            return Count(predicate);
        }

        IEnumerable<T> INhRepository<T>.Fetch(Expression<Func<T, bool>> predicate)
        {
            return Fetch(predicate).ToReadOnlyCollection();
        }

        IEnumerable<T> INhRepository<T>.Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
        {
            return Fetch(predicate, order).ToReadOnlyCollection();
        }

        IEnumerable<T> INhRepository<T>.Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip,
                                            int count)
        {
            return Fetch(predicate, order, skip, count).ToReadOnlyCollection();
        }

        #endregion

        public virtual T Get(int id)
        {
            return Session.Get<T>(id);
        }

        public virtual T Get(Expression<Func<T, bool>> predicate)
        {
            return Fetch(predicate).FirstOrDefault();
        }

        public virtual void Create(T entity)
        {

            Logger.Debug("Create {0}", entity);
            Session.Save(entity);
            Flush();
        }

        public virtual void Update(T entity)
        {
            Logger.Debug("Update {0}", entity);
            Session.Evict(entity);
            Session.Merge(entity);
            Flush();
        }

        public virtual void Delete(T entity)
        {
            Logger.Debug("Delete {0}", entity);
            Session.Delete(entity);
            Flush();
        }

        public virtual void Copy(T source, T target)
        {
            Logger.Debug("Copy {0} {1}", source, target);
            var metadata = Session.SessionFactory.GetClassMetadata(typeof(T));
            var values = metadata.GetPropertyValues(source, EntityMode.Poco);
            metadata.SetPropertyValues(target, values, EntityMode.Poco);
        }

        public virtual void Flush()
        {
            Session.Flush();
        }

        public virtual int Count(Expression<Func<T, bool>> predicate)
        {
            return Fetch(predicate).Count();
        }

        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate)
        {
            return predicate == null ? Table : Table.Where(predicate);
        }

        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
        {
            var result = Fetch(predicate);
            if (order == null) return result;
            var orderable = new Orderable<T>(result);
            order(orderable);
            return orderable.Queryable;
        }

        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip,
                                           int count)
        {
            return skip > 0 ? Fetch(predicate, order).Skip(skip).Take(count) : Fetch(predicate, order).Take(count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IPageOfItems<T> GetPaged(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int pageIndex, int pageSize)
        {
            var totalCount = Count(predicate);
            return Fetch(predicate, order).GetPaged(totalCount, pageIndex, pageSize);
        }

        public bool Exists(Expression<Func<T, bool>> func)
        {
            return Count(func) > 0;
        }
    }
}