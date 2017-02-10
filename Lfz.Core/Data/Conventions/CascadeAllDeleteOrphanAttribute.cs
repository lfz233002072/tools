using System;
using FluentNHibernate.Conventions.Instances;

namespace PMSoft.Data.Conventions {

    /// <summary>
    /// 
    /// </summary>
    public class CascadeAllDeleteOrphanAttribute : Attribute {
    }

    /// <summary>
    /// 
    /// </summary>
    public class CascadeAllDeleteOrphanConvention : 
        AttributeCollectionConvention<CascadeAllDeleteOrphanAttribute> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="instance"></param>
        protected override void Apply(CascadeAllDeleteOrphanAttribute attribute, ICollectionInstance instance) {
            instance.Cascade.AllDeleteOrphan();
        }
    }
}
