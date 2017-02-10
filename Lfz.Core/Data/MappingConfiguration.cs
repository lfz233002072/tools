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

using System.Data.Entity.ModelConfiguration;

namespace PMSoft.Data
{
    public abstract class MappingConfiguration<T> : EntityTypeConfiguration<T>
        where T : EntityBase
    {
        protected string TablePrefix = "Wf_";

        protected MappingConfiguration()
        { 
            ToTable(TablePrefix + typeof(T).Name);
            HasKey(a => a.Id);
        }
    }

    public abstract class WfMappingConfiguration<T> : MappingConfiguration<T>
    where T : WfEntityBase
    { 
        protected WfMappingConfiguration()
        { 
            Property(x => x.DisplayName).HasMaxLength(255); 
        }
    }
}