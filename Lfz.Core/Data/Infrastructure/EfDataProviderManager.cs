using System;
using PMSoft.Config;

namespace PMSoft.Data.Infrastructure
{
    public partial class EfDataProviderManager : BaseDataProviderManager
    {
        public EfDataProviderManager(DataSettings settings):base(settings)
        {
        }

        public override IDataProvider LoadDataProvider()
        {

            var providerName = Settings.DataProvider;
            if (String.IsNullOrWhiteSpace(providerName))
                throw new Exception("Data Settings doesn't contain a providerName");

            switch (providerName.ToLowerInvariant())
            {
                case "sqlserver":
                    return new SqlServerDataProvider(); 
                default:
                    throw new Exception(string.Format("Not supported dataprovider name: {0}", providerName));
            }
        }

    }
}
