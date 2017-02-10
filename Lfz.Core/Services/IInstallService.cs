
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMSoft.Services
{
    public interface IInstallService
    {
        //安装数据库
        bool Install();
    }
}
