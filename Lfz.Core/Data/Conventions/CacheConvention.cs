using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace PMSoft.Data.Conventions {
    /// <summary>
    /// 
    /// </summary>
    public class CacheConvention : IClassConvention, IConventionAcceptance<IClassInspector> {
        private readonly IEnumerable<RecordBlueprint> _descriptors;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptors"></param>
        public CacheConvention(IEnumerable<RecordBlueprint> descriptors) {
            _descriptors = descriptors;
        }

        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public void Apply(IClassInstance instance) {
            instance.Cache.ReadWrite();
        }

        /// <summary>
        /// Whether this convention will be applied to the target.
        /// </summary>
        /// <param name="criteria">Instace that could be supplied</param>
        /// <returns>
        /// Apply on this target?
        /// </returns>
        public void Accept(IAcceptanceCriteria<IClassInspector> criteria) {
            criteria.Expect(x => _descriptors.Any(d => d.Type.Name == x.EntityType.Name));
        }
    }

     
}