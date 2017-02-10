using System;
using NHibernate.Cfg;

namespace PMSoft.Data
{
    /// <summary>
    /// Session≈‰÷√ª∫¥Ê
    /// </summary>
    public interface ISessionConfigurationCache 
    {
        /// <summary>
        /// ¥”ª∫¥Ê÷–ªÒ»°≈‰÷√
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        Configuration GetConfiguration(Func<Configuration> builder);
    }
}