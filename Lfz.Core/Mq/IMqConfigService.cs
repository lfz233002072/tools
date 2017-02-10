// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :IConfigCenterService.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2015-11-16 15:58
// *        https://git.oschina.net/lfz/tools
// *
// *======================================================================*/

using System;
using System.Collections.Generic;

namespace Lfz.Mq
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMqConfigService : IPerHttpRequestDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        MqClientConfigInfo GetMqConfig(Guid storeId,bool useLanIpAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<MqInstanceInfo> GetAll();

        /// <summary>
        /// 
        /// </summary>
        void Reset();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idlist"></param>
        /// <returns></returns>
        bool Delete(IEnumerable<int> idlist);

    }
}