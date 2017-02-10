using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using Lfz.Data.RawSql;

namespace Lfz.Data.Nh.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class RecordTableNameConvention : IClassConvention
    {
        private readonly List<Type> _types;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        public RecordTableNameConvention(IEnumerable<Type> types)
        {
            _types = new List<Type>(types);
        }

        public void Apply(IClassInstance instance)
        {
            var type = _types.FirstOrDefault(x => x == instance.EntityType);
            if (type != null)
            {
                var attr = type.GetCustomAttributes(typeof(CustomTableNameAttribute), true).FirstOrDefault() as CustomTableNameAttribute;
                if (attr != null) instance.Table(attr.TableName); ;

                var attr2 =
                    type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
                if (attr2 != null)
                    instance.Table(attr2.Name);
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class EnumConvention : IUserTypeConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            instance.CustomType(instance.Property.PropertyType);
        }

        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Property.PropertyType.IsEnum);
        }
        public bool Accept(Type type)
        {
            return type.IsEnum;
        }
    }

    public class ColumnConvention : IColumnConvention
    {
        public void Apply(IColumnInstance instance)
        {
        }
    }

    public class PropertyConvertion : IPropertyConvention
    {
        private readonly IDbProviderConfig _config;

        public PropertyConvertion(IDbProviderConfig config)
        {
            _config = config;
        }

        public void Apply(IPropertyInstance instance)
        {
            var list = instance.Property.MemberInfo.GetCustomAttributes(typeof (ColumnAttribute), true);
            if(!list.Any()) return;
            var attr = (ColumnAttribute)list.FirstOrDefault();
            if (attr != null)
            {
                if (!string.IsNullOrEmpty(attr.Name))
                    instance.Column(attr.Name);
                if (string.IsNullOrEmpty(attr.TypeName)) return;
                if (_config.DbProvider == DbProvider.MySql)
                {
                    var typename = attr.TypeName.ToLower();
                    if (typename == "ntext")
                        instance.CustomSqlType("text");
                    else if (typename == "nvarchar")
                        instance.CustomSqlType("varchar");
                    else if (typename == "nchar")
                        instance.CustomSqlType("char");
                    else
                        instance.CustomSqlType(typename);
                }
                else if (_config.DbProvider == DbProvider.SqlServer)
                {
                    var typename = attr.TypeName.ToLower();
                    if (typename == "text")
                        instance.CustomSqlType("ntext");
                    else if (typename == "varchar")
                        instance.CustomSqlType("nvarchar");
                    else if (typename == "char")
                        instance.CustomSqlType("nchar");
                    else
                        instance.CustomSqlType(typename);
                }else
                    instance.CustomSqlType(attr.TypeName);
            }
        }
    }
}