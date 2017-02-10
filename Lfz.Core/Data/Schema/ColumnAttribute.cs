// ReSharper disable once CheckNamespace
namespace System.ComponentModel.DataAnnotations.Schema
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        private readonly string _name;
        private string _typeName;
        private int _order = -1;
        public string Name
        {
            get
            {
                return this._name;
            }
        }
        public int Order
        {
            get
            {
                return this._order;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._order = value;
            }
        }
        public string TypeName
        {
            get
            {
                return this._typeName;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("TypeName不能为空");
                }
                this._typeName = value;
            }
        }
        public ColumnAttribute()
        {
        }
        public ColumnAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("字段名称不能为空");
            }
            this._name = name;
        }
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        private readonly string _name;
        private string _schema;

        public string Name
        {

            get
            {
                return this._name;
            }
        }

        public string Schema
        {

            get
            {
                return this._schema;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("表名称不能为空");
                }
                this._schema = value;
            }
        }

        public TableAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("表名称不能为空");
            }
            this._name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NotMappedAttribute : Attribute
    {

        public NotMappedAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DatabaseGeneratedAttribute : Attribute
    {

        public DatabaseGeneratedOption DatabaseGeneratedOption
        {

            get;
            private set;
        }

        public DatabaseGeneratedAttribute(DatabaseGeneratedOption databaseGeneratedOption)
        {
            if (!Enum.IsDefined(typeof(DatabaseGeneratedOption), databaseGeneratedOption))
            {
                throw new ArgumentOutOfRangeException("databaseGeneratedOption");
            }
            this.DatabaseGeneratedOption = databaseGeneratedOption;
        }
    }

    public enum DatabaseGeneratedOption
    {
        None,
        Identity,
        Computed
    }
}
