namespace PMSoft.Data.Providers {
    /// <summary>
    /// 
    /// </summary>
    public interface IDataServicesProviderFactory   {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionFactoryParameters"></param>
        /// <returns></returns>
        IDataServicesProvider CreateProvider(DataServiceParameters sessionFactoryParameters);
    }

}
