using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using Lfz.Collections;
using Lfz.Enums; 
using Lfz.Logging;
using Lfz.Utitlies;

namespace Lfz.Data
{



    /// <summary>
    /// 数据仓库
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        public   DbContext CurrentDbContext { get; private set; }
         

        internal DbSet<T> DbSet;

        /// <summary>
        /// 
        /// </summary> 
        /// <param name="dbContext"></param> 
        protected Repository(DbContext dbContext )
        {
            CurrentDbContext = dbContext; 
            CurrentDbContext.Configuration.ValidateOnSaveEnabled = false; 
            this.DbSet = CurrentDbContext.Set<T>();
            Logger = LoggerFactory.GetLog();
        }

        /// <summary>
        /// 整体提交
        /// </summary>
        public int Commit()
        {
            try
            {
                return CurrentDbContext.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                string message = "";
                foreach (var eve in dbEx.EntityValidationErrors)
                {
                    message += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                             eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        message += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                 ve.PropertyName, ve.ErrorMessage);
                    }
                }
                Logger.Error(GetType().FullName + ".Create Message" + message);
                throw; 
            }
            catch (Exception e)
            {
                Logger.Error(e, GetType().FullName + ".Create");
                throw;
            }
            return 0;
        }

        /// <summary>
        /// 日志提供接口
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get { return DbSet; }
        }

        #region IRepository<T> Members

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="entity"></param>
        void IRepository<T>.Create(T entity)
        {
            Create(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void IRepository<T>.Update(T entity)
        {
            Update(entity);
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity"></param>
        public void CreateOrUpdate(Guid id, T entity)
        {
            if (entity == null) return;
            var flag = Get(id) == null;
            if (flag)
            {
                Create(entity);
            }
            else
            {
                Update(entity);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void IRepository<T>.Delete(T entity)
        {
            Delete(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        void IRepository<T>.Delete(Guid id)
        {
            Delete(id);
        }


        /// <summary>
        /// 返回序列中的唯一元素；如果该序列为空，则返回默认值；如果该序列包含多个元素，那么返回第一个元素
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T IRepository<T>.Get(Expression<Func<T, bool>> predicate)
        {
            return Get(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        IQueryable<T> IRepository<T>.Table
        {
            get { return Table; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int IRepository<T>.Count(Expression<Func<T, bool>> predicate)
        {
            return Count(predicate);
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return Count(predicate) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> IRepository<T>.Fetch(Expression<Func<T, bool>> predicate)
        {
            return Fetch(predicate).ToReadOnlyCollection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        IEnumerable<T> IRepository<T>.Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
        {
            return Fetch(predicate, order).ToReadOnlyCollection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <param name="skip"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<T> IRepository<T>.Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip,
                                            int count)
        {
            return Fetch(predicate, order, skip, count).ToReadOnlyCollection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> IRepository<T>.GetAll()
        {
            return Table.ToReadOnlyCollection();
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T Get(object id)
        {
            return DbSet.Find(id);
        }

        /// <summary>
        /// 返回序列中的唯一元素；如果该序列为空，则返回默认值；如果该序列包含多个元素，那么返回第一个元素
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual T Get(Expression<Func<T, bool>> predicate)
        {
            return Fetch(predicate).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Create(T entity)
        {
            FillEntity(true, entity);
            //Logger.Debug("Create {0}", entity.ToString());
            DbSet.Add(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(T entity)
        {
            FillEntity(false, entity);
           // Logger.Debug("Update {0}", entity.ToString());
            DbSet.Attach(entity);
            CurrentDbContext.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Delete(T entity)
        {
          //  Logger.Debug("Delete {0}", entity);
            if (CurrentDbContext.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
        }
        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(object id)
        {
            T entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<T, bool>> predicate)
        {
            return Fetch(predicate).Count();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate)
        {
            return predicate == null ? Table : Table.Where(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
        {
            if (order == null) return Fetch(predicate);
            var orderable = new Orderable<T>(Fetch(predicate));
            order(orderable);
            return orderable.Queryable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <param name="skip"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip,
                                           int count)
        {
            return Fetch(predicate, order).Skip(skip).Take(count);
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

        #region 辅助方法


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        private void FillTreeContentWhenCreate(T entity)
        {
             
        }

        private void FillEntity(bool isInsert, T entity)
        { 
        }


        #endregion



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public void ExecuteSql(string sql, params object[] parameters)
        {
            this.CurrentDbContext.Database.ExecuteSqlCommand(sql, parameters);
        }

    }
}