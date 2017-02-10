using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using PMSoft.Data.Providers;

namespace PMSoft.Data.Conventions
{
    /// <summary>
    /// 数据表名称转换器
    /// </summary>
    public class RecordTableNameConvention : IClassConvention
    {
        private readonly SessionFactoryParameters _parameters;
        private readonly Dictionary<Type, RecordBlueprint> _descriptors;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public RecordTableNameConvention(SessionFactoryParameters parameters)
        {
            _parameters = parameters;
            _descriptors = parameters.RecordDescriptors.ToDictionary(d => d.Type);
        }

        /// <summary>
        /// Apply changes to the target
        /// </summary>
        public void Apply(IClassInstance instance)
        {
            RecordBlueprint desc;
            if (_descriptors.TryGetValue(instance.EntityType, out desc))
            {
                instance.Table(GetTablename(desc.TableName));
            }
        }

        private string GetTablename(string name)
        {
            //移除结尾字符串Entity
            if ((name.EndsWith("Entity", StringComparison.OrdinalIgnoreCase) || name.EndsWith("Record", StringComparison.OrdinalIgnoreCase))
                && name.Length > 8)
                name = name.Substring(0, name.Length - 6);
            var prefix = _parameters.TablePrefix;
            if (!string.IsNullOrEmpty(prefix))
            {
                var fix = prefix.Trim('_');
                if (name.StartsWith(fix)) name = name.Substring(fix.Length);
                return fix + "_" + name;
            }
            return name;
        }
    }
}