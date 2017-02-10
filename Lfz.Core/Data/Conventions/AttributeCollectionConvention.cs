using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace PMSoft.Data.Conventions {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AttributeCollectionConvention<T> : ICollectionConvention, ICollectionConventionAcceptance where T : Attribute {
        /// <summary>
        /// Whether this convention will be applied to the target.
        /// </summary>
        /// <param name="criteria">Instace that could be supplied</param>
        /// <returns>
        /// Apply on this target?
        /// </returns>
        public void Accept(IAcceptanceCriteria<ICollectionInspector> criteria) {
            criteria.Expect(inspector => GetAttribute(inspector) != null);
        }

        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public void Apply(ICollectionInstance instance) {
            Apply(GetAttribute(instance), instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="instance"></param>
        protected abstract void Apply(T attribute, ICollectionInstance instance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inspector"></param>
        /// <returns></returns>
        private static T GetAttribute(ICollectionInspector inspector) {
            return Attribute.GetCustomAttribute(inspector.Member, typeof(T)) as T;
        }
    }
}