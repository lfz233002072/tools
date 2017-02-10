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

using System;
using FluentNHibernate.Mapping;

namespace Lfz.Data.Nh.Entities
{

    public abstract class EntityMap<T, TKey> :
        ClassMap<T> where T : EntityBase<TKey>
    {
        protected EntityMap()
        {
            if (typeof (TKey) == typeof (Guid))
                Id(x => x.Id).GeneratedBy.Assigned();
            else Id(x => x.Id);
        }
    }
}