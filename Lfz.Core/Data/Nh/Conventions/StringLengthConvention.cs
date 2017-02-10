using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Lfz.Data.Nh.Conventions {
    public class StringLengthConvention : AttributePropertyConvention<StringLengthAttribute> {
        protected override void Apply(StringLengthAttribute attribute, IPropertyInstance instance) {
            instance.Length(attribute.MaximumLength);
        }
    }
}
