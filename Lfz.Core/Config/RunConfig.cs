// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :RunConfig.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-29 11:00
// *        https://git.oschina.net/lfz/tools
// *
// *======================================================================*/

using Lfz.Logging;

namespace Lfz.Config
{
    public partial class RunConfig : JsonConfigBase
    {
        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static RunConfig Current
        {
            get
            {
                return Load<RunConfig>() ?? new RunConfig();
            }
        }

        public LoggerType LoggerType { get; set; }

        public string ApiDomainUrl { get; set; } 

        /// <summary>
        /// 配置项是否加密启动
        /// </summary>
        public bool EnabledEncrypt { get; set; }

        public override string GetConfigFile()
        {
            return "~/Config/RunConfig.json";
        }
    }
}