using Lfz.Data.RawSql;

namespace Lfz.Data.Nh.Providers {
     

    public class DataServicesProviderFactory : IDataServicesProviderFactory { 
         

        public IDataServicesProvider CreateProvider(IDbProviderConfig parameters)
        {
            var providerName = parameters.DbProvider.ToString();
            if(parameters.DbProvider==DbProvider.MySql)
                return new MySqlDataServicesProvider(parameters.DataFolder, parameters.DbConnectionString);
            else if(parameters.DbProvider==DbProvider.SqlServer)
                return new SqlServerDataServicesProvider(parameters.DataFolder, parameters.DbConnectionString);
            if (providerName == "SqlCe")
            {
                return new SqlCeDataServicesProvider(parameters.DataFolder, parameters.DbConnectionString);
            }
            return null;
        }
         
    }
}
