using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace PMSoft.Data.Conventions {
    /// <summary>
    /// 
    /// </summary>
    public class StringLengthMaxAttribute : StringLengthAttribute {
        /// <summary>
        /// 
        /// </summary>
        public StringLengthMaxAttribute() : base(10000) {
            // 10000 is an arbitrary number large enough to be in the nvarchar(max) range 
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StringLengthConvention : AttributePropertyConvention<StringLengthAttribute> {
        /// <summary>
        /// Apply changes to a property with an attribute matching T.
        /// </summary>
        /// <param name="attribute">Instance of attribute found on property.</param><param name="instance">Property with attribute</param>
        protected override void Apply(StringLengthAttribute attribute, IPropertyInstance instance) {
            instance.Length(attribute.MaximumLength);
        }
    }
}
