// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :INhTypeProvider.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-14 10:12
// *        https://git.oschina.net/lfz/tools
// *
// *======================================================================*/

using System;
using System.Collections.Generic;

namespace Lfz.Data.Nh
{
    /// <summary>
    /// 
    /// </summary>
    public interface INhTypeProvider:ISingletonDependency
    {
        IEnumerable<Type> GetTypes();
    }
}