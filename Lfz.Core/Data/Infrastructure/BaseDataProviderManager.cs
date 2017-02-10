using System;
using PMSoft.Config;

namespace PMSoft.Data.Infrastructure
{
    public abstract class BaseDataProviderManager
    {
        protected BaseDataProviderManager(DataSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            this.Settings = settings;
        }
        protected DataSettings Settings { get; private set; }
        public abstract IDataProvider LoadDataProvider();
    }
}
