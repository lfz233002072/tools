/*======================================================================
 *
 *        Copyright (C)  1996-2012  杭州品茗信息有限公司    
 *        All rights reserved
 *
 *        Filename :InstallService.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-05-08 14:41
 *        http://www.pinming.cn/
 *
 *======================================================================*/

using System;
using System.Runtime.CompilerServices;
using PMSoft.Config;

namespace PMSoft.Services
{
    public class InstallService : IInstallService
    { 

        public InstallService( )
        { 
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Install()
        {
            bool flag = false;
            if (DataSettingsHelper.DatabaseIsInstalled()) return flag; 
            try
            {
                //init data provider
            /*    var dataProviderInstance = _providerManager.LoadDataProvider();
                dataProviderInstance.InitDatabase();*/
                //now resolve installation service
                //var installationService = ContainerContext.Resolve<IInstallationService>();
                //installationService.InstallData(model.AdminEmail, model.AdminPassword, model.InstallSampleData); 
                //TODO 初始化数据
                //reset cache
                DataSettingsHelper.Installed();
                flag = true;
            }
            catch (Exception exception)
            {
                //reset cache
                DataSettingsHelper.ResetCache(); 
            }
            return flag;
        }
    }
}