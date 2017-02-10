using System;

namespace Lfz.Data.Nh.Entities
{
    public abstract class GuidMap<T> :
        EntityMap<T, Guid> where T : EntityBase<Guid>
    {
    }
}