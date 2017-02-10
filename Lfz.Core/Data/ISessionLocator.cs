using System;
using NHibernate;

namespace PMSoft.Data {
    /// <summary>
    /// 
    /// </summary>
    public interface ISessionLocator :IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        ISession For(Type entityType);
    }
}