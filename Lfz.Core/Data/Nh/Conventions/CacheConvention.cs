using System;
using System.Collections.Generic;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Lfz.Data.Nh.Conventions {
    public class CacheConvention : IClassConvention, IConventionAcceptance<IClassInspector> {

        public CacheConvention(IEnumerable<Type> types)
        {
            
        }

        public void Apply(IClassInstance instance) {
            instance.Cache.ReadWrite();
        }

        public void Accept(IAcceptanceCriteria<IClassInspector> criteria) { 
        }
    }
}