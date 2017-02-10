/*======================================================================
 *
 *        Copyright (C)  1996-2012  杭州品茗信息有限公司
 *        All rights reserved
 *
 *        Filename :EntityBase.cs
 *        DESCRIPTION :Base class for entities
 *
 *        Created By 林芳崽 at  2013-05-08 18:47
 *        http://www.pinming.cn/
 *
 *======================================================================*/

using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace PMSoft.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class EntityMapping<T> : EntityTypeConfiguration<T>
        where T : EntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected EntityMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }

}
