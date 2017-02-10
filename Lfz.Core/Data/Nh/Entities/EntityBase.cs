/*======================================================================
 *
 *        Copyright (C)  1996-2012  杭州品茗物联网平台事业部    
 *        All rights reserved
 *
 *        Filename :EntityBase.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at  2013-03-30 19:11
 *        http://www.carvingsky.com/
 *
 *======================================================================*/

namespace Lfz.Data.Nh.Entities
{
    /// <summary>
    /// 数据库表基类
    /// </summary>
    public abstract class EntityBase
    {
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected bool Equals(EntityBase other)
        {
            if (other == null)
                return false;
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EntityBase)obj);
        }


        public static bool operator ==(EntityBase x, EntityBase y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(EntityBase x, EntityBase y)
        {
            return !(x == y);
        }
    }

    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract partial class EntityBase<T> : EntityBase
    {
        /// <summary>
        /// 实体对象唯一标识
        /// </summary>
        public virtual T Id { get; set; }

        /// <summary>
        /// 如果Id相同且同other实例继承关系，那么返回true
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as EntityBase<T>);
        }

        /// <summary>
        /// 非空且Id不等于0，那么返回true
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsTransient(EntityBase<T> obj)
        {
            return obj != null && Equals(obj.Id, default(int));
        }

        /// <summary>
        /// 如果Id相同且同other实例继承关系，那么返回true
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(EntityBase<T> other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(Id, other.Id))
            {
                var otherType = other.GetType();
                var thisType = GetType();
                return thisType.IsAssignableFrom(otherType) ||
                        otherType.IsAssignableFrom(thisType);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Equals(Id, default(int)) ? base.GetHashCode() : Id.GetHashCode();
        }


    }
}