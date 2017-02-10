using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using PMSoft.Collections;
using PMSoft.Config;
using PMSoft.Data.Infrastructure;
using PMSoft.Logging;

namespace PMSoft.Data
{
    /// <summary>
    /// Entity Framework repository
    /// </summary>
    public partial class EfRepository<T> where T : EntityBase
    {
        private readonly IDbContext _context;
        private IDbSet<T> _entities;
        public ILogger Logger { get; set; }
         
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EfRepository(IDbContext context)
        {
            Logger = LoggerFactory.GetLog();
            this._context = context;
        }

        public T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        public void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Add(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                Logger.Error(msg);
                throw new Exception(msg);
            }
        }


        public void Update(IEnumerable<T> entities)
        {
            try
            {
                foreach (var entity in entities)
                {

                }
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                Logger.Error(msg);
                throw new Exception(msg);
            }
        }

        public void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                Logger.Error(msg);
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Delete(object id)
        {
            var entity = GetById(id);
            Delete(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public bool Exists(Guid id)
        {
            return Table.Count(x => x.Id == id) > 0;
        }

        public void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Remove(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                Logger.Error(msg);
                throw new Exception(msg);
            }
        }

        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="whereClause"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetList<TKey>(Func<T, bool> whereClause, Func<T, TKey> keySelector)
        {
            return Table.Where(whereClause).OrderBy(keySelector).ToReadOnlyCollection();
        }

        /// <summary>
        /// 获取数据列表
        /// </summary> 
        /// <param name="whereClause"></param> 
        /// <returns></returns>
        public virtual IEnumerable<T> GetList(Func<T, bool> whereClause)
        {
            return Table.Where(whereClause).ToReadOnlyCollection();
        }

        private IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }
    }
}