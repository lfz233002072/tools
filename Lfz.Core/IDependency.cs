// ======================================================================
// 	
// 	Copyright (C)  1996-2012  lfz    
// 	All rights reserved
// 	
// 	Filename :DemoController.cs
// 	DESCRIPTION :
// 	
// 	Created By Administrator  at 2014-04-30 14:40
// 	https://git.oschina.net/lfz/tools
// 
// ======================================================================*/

namespace Lfz
{
    /// <summary>
    /// Base interface for services that are instantiated per unit of work (i.e. web request).
    /// </summary>
    public interface IDependency
    {
    }

    /// <summary>
    /// Base interface for services that are instantiated per shell/tenant.
    /// </summary>
    public interface ISingletonDependency : IDependency
    {
    }

    /// <summary>
    /// Base interface for services that are instantiated per shell/tenant.
    /// </summary>
    public interface IPerHttpRequestDependency : IDependency
    {
    }

    /// <summary>
    /// Base interface for services that may *only* be instantiated in a unit of work.
    /// This interface is used to guarantee they are not accidentally referenced by a singleton dependency.
    /// </summary>
    public interface IUnitOfWorkDependency : IDependency
    {
    }

    /// <summary>
    ///  不参与依赖注入
    /// </summary>
    public interface INonDependency
    {
    }
}