using System;
using FluentNHibernate.Cfg.Db;

namespace PMSoft.Data.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlDataServicesProvider : AbstractDataServicesProvider
    {
        private readonly string _dataFolder;
        private readonly string _connectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFolder"></param>
        /// <param name="connectionString"></param>
        public MySqlDataServicesProvider(string dataFolder, string connectionString)
        {
            _dataFolder = dataFolder;
            _connectionString = connectionString;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string ProviderName
        {
            get { return "MySql"; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createDatabase"></param>
        /// <returns></returns>
        public override IPersistenceConfigurer GetPersistenceConfigurer(bool createDatabase)
        {
            var persistence = MySQLConfiguration.Standard;
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentException("The connection string is empty");
            }
            persistence = persistence.ConnectionString(_connectionString);
            return persistence;
        }
    }
}