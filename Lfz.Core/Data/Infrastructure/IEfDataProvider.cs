using System.Data.Entity.Infrastructure;

namespace PMSoft.Data.Infrastructure
{
    public interface IEfDataProvider: IDataProvider
    {
        /// <summary>
        /// Get connection factory
        /// </summary>
        /// <returns>Connection factory</returns>
        IDbConnectionFactory GetConnectionFactory();

        /// <summary>
        /// Initialize connection factory
        /// </summary>
        void InitConnectionFactory();

        /// <summary>
        /// Set database initializer
        /// </summary>
        void SetDatabaseInitializer();
    }
}
