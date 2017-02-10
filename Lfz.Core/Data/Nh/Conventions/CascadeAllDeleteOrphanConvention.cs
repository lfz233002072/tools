using FluentNHibernate.Conventions.Instances;

namespace Lfz.Data.Nh.Conventions
{
    public class CascadeAllDeleteOrphanConvention : 
        AttributeCollectionConvention<CascadeAllDeleteOrphanAttribute> {

        protected override void Apply(CascadeAllDeleteOrphanAttribute attribute, ICollectionInstance instance) {
            instance.Cascade.AllDeleteOrphan();
        }
        }
}