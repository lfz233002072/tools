using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using FluentNHibernate.Cfg.Db;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.SqlTypes;

namespace PMSoft.Data.Providers {
    /// <summary>
    /// 
    /// </summary>
    public class SqlCeDataServicesProvider : AbstractDataServicesProvider {
        private readonly string _fileName;
        private readonly string _dataFolder;
        private readonly string _connectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFolder"></param>
        /// <param name="connectionString"></param>
        public SqlCeDataServicesProvider(string dataFolder, string connectionString) {
            _dataFolder = dataFolder;
            _connectionString = connectionString;
            _fileName = Path.Combine(_dataFolder, "PmWorkFlowDb.sdf");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public SqlCeDataServicesProvider(string fileName) {
            _dataFolder = Path.GetDirectoryName(fileName);
            _fileName = fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string ProviderName {
            get { return "SqlCe"; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createDatabase"></param>
        /// <returns></returns>
        public override IPersistenceConfigurer GetPersistenceConfigurer(bool createDatabase) {
            var persistence = MsSqlCeConfiguration.MsSqlCe40;

            if (createDatabase) {
                File.Delete(_fileName);
            }

            string localConnectionString = string.Format("Data Source={0}", _fileName);
            if (!File.Exists(_fileName)) {
                CreateSqlCeDatabaseFile(localConnectionString);
            }

            persistence = persistence.ConnectionString(localConnectionString);
            persistence = persistence.Driver(typeof(PmSqlServerCeDriver).AssemblyQualifiedName);
            return persistence;
        }

        private void CreateSqlCeDatabaseFile(string connectionString) {
            if (!string.IsNullOrEmpty(_dataFolder))
                Directory.CreateDirectory(_dataFolder);

            // We want to execute this code using Reflection, to avoid having a binary
            // dependency on SqlCe assembly

            //engine engine = new SqlCeEngine();
            //const string assemblyName = "System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
            const string assemblyName = "System.Data.SqlServerCe";
            const string typeName = "System.Data.SqlServerCe.SqlCeEngine";

            var sqlceEngineHandle = Activator.CreateInstance(assemblyName, typeName);
            var engine = sqlceEngineHandle.Unwrap();

            //engine.LocalConnectionString = connectionString;
            engine.GetType().GetProperty("LocalConnectionString").SetValue(engine, connectionString, null/*index*/);

            //engine.CreateDatabase();
            engine.GetType().GetMethod("CreateDatabase").Invoke(engine, null);

            //engine.Dispose();
            engine.GetType().GetMethod("Dispose").Invoke(engine, null);
        }

        /// <summary>
        /// 
        /// </summary>
        public class PmSqlServerCeDriver : SqlServerCeDriver {
            private PropertyInfo _dbParamSqlDbTypeProperty;

            /// <summary>
            /// Configure the driver using <paramref name="settings"/>.
            /// </summary>
            public override void Configure(IDictionary<string, string> settings) {
                base.Configure(settings);
                using ( var cmd = CreateCommand() ) {
                    var dbParam = cmd.CreateParameter();
                    _dbParamSqlDbTypeProperty = dbParam.GetType().GetProperty("SqlDbType");
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dbParam"></param>
            /// <param name="name"></param>
            /// <param name="sqlType"></param>
            protected override void InitializeParameter(IDbDataParameter dbParam, string name, SqlType sqlType) {
                base.InitializeParameter(dbParam, name, sqlType);

                if(sqlType.DbType == DbType.Binary) {
                    _dbParamSqlDbTypeProperty.SetValue(dbParam, SqlDbType.Image, null);
                    return;
                }

                if ( sqlType.Length <= 4000 ) {
                    return;
                }

                switch(sqlType.DbType) {
                    case DbType.String:
                        _dbParamSqlDbTypeProperty.SetValue(dbParam, SqlDbType.NText, null);
                        break;
                    case DbType.AnsiString:
                        _dbParamSqlDbTypeProperty.SetValue(dbParam, SqlDbType.Text, null);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MsSqlCeConfiguration : PersistenceConfiguration<MsSqlCeConfiguration> {
        /// <summary>
        /// 
        /// </summary>
        protected MsSqlCeConfiguration() {
            Driver<CustomSqlServerCeDriver>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static MsSqlCeConfiguration MsSqlCe40 {
            get { return new MsSqlCeConfiguration().Dialect<CustomMsSqlCe40Dialect>(); }

        }

        /// <summary>
        /// Custom driver so that Text/NText fields are not truncated at 4000 characters
        /// </summary>
        public class CustomSqlServerCeDriver : SqlServerCeDriver {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="dbParam"></param>
            /// <param name="name"></param>
            /// <param name="sqlType"></param>
            protected override void InitializeParameter(IDbDataParameter dbParam, string name, SqlType sqlType) {
                base.InitializeParameter(dbParam, name, sqlType);

                PropertyInfo dbParamSqlDbTypeProperty = dbParam.GetType().GetProperty("SqlDbType");

                if (sqlType.Length <= 4000) {
                    return;
                }

                switch (sqlType.DbType) {
                    case DbType.String:
                        dbParamSqlDbTypeProperty.SetValue(dbParam, SqlDbType.NText, null);
                        break;
                    case DbType.AnsiString:
                        dbParamSqlDbTypeProperty.SetValue(dbParam, SqlDbType.Text, null);
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class CustomMsSqlCe40Dialect : MsSqlCe40Dialect {
            /// <summary>
            /// Can parameters be used for a statement containing a LIMIT?
            /// </summary>
            public override bool SupportsVariableLimit {
                get { return true; }
            }
        }
    }
}
