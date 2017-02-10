using System;
using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace PMSoft.Data.Conventions {
    /// <summary>
    /// This attribute is used to mark relationships which need to be eagerly fetched with the parent object,
    /// thus defining an aggregate in terms of DDD
    /// </summary>
    public class AggregateAttribute : Attribute {
    }

    /// <summary>
    /// 
    /// </summary>
    public class ReferenceConvention : IReferenceConvention, IReferenceConventionAcceptance, IHasManyConvention, IHasManyConventionAcceptance {
        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public void Apply(IManyToOneInstance instance) {
            instance.Fetch.Join();
        }

        /// <summary>
        /// Whether this convention will be applied to the target.
        /// </summary>
        /// <param name="criteria">Instace that could be supplied</param>
        /// <returns>
        /// Apply on this target?
        /// </returns>
        public void Accept(IAcceptanceCriteria<IManyToOneInspector> criteria) {
            criteria.Expect(x => x.Property != null && x.Property.MemberInfo.GetCustomAttributes(typeof(AggregateAttribute), false).Any());
        }

        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public void Apply(IOneToManyCollectionInstance instance) {
            instance.Fetch.Select();
            instance.Cache.ReadWrite();
        }

        /// <summary>
        /// Whether this convention will be applied to the target.
        /// </summary>
        /// <param name="criteria">Instace that could be supplied</param>
        /// <returns>
        /// Apply on this target?
        /// </returns>
        public void Accept(IAcceptanceCriteria<IOneToManyCollectionInspector> criteria) {
            criteria.Expect(x => x.Member != null && x.Member.IsDefined(typeof(AggregateAttribute), false));
        }
    }
}

