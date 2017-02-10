// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :BindingMoblieStatus.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2015-05-18 16:30
// *        https://git.oschina.net/lfz/tools
// *
// *======================================================================*/
namespace Lfz.Enums
{
    /// <summary>
    /// 绑定手机号码状态 0 未绑定 1 绑定申请 2 绑定成功
    /// </summary>
    public enum BindingMoblieStatus
    {
        /// <summary>
        /// 为绑定
        /// </summary>
        None=0,

        /// <summary>
        /// 等待绑定中
        /// </summary>
        WaitBinding=1,

        /// <summary>
        /// 已经绑定
        /// </summary>
        HasBinding=2
    }
}