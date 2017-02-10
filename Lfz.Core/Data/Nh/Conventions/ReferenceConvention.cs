using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Lfz.Data.Nh.Conventions
{
    public class ReferenceConvention : IReferenceConvention, IReferenceConventionAcceptance, IHasManyConvention, IHasManyConventionAcceptance {
        public void Apply(IManyToOneInstance instance) {
            instance.Fetch.Join();
        }

        public void Accept(IAcceptanceCriteria<IManyToOneInspector> criteria) {
            criteria.Expect(x => x.Property != null && x.Property.MemberInfo.GetCustomAttributes(typeof(AggregateAttribute), false).Any());
        }

        public void Apply(IOneToManyCollectionInstance instance) {
            instance.Fetch.Select();
            instance.Cache.ReadWrite();
        }

        public void Accept(IAcceptanceCriteria<IOneToManyCollectionInspector> criteria) {
            criteria.Expect(x => x.Member != null && x.Member.IsDefined(typeof(AggregateAttribute), false));
        }
    }
}