namespace Lfz.Enums
{
    /// <summary>
    /// 系统用户状态
    /// </summary>
    public enum UserDataStatus
    {

        /// <summary>
        /// 正常
        /// </summary>

        [CustomDescription("正常")]
        Normarl=0,

        /// <summary>
        /// 已经锁住
        /// </summary>
        [CustomDescription("已经锁住")]
        Locked =-1,

        /// <summary>
        /// 已经提交单未验证
        /// </summary>
        [CustomDescription("未验证")]
        UnVerify=-2,

        /// <summary>
        /// 未设置账户基本信息
        /// </summary> 
        [CustomDescription("未设置账户基本信息")]
        UnInitAccount = -3,

        /// <summary>
        /// 微信账号解除绑定
        /// </summary>
        [CustomDescription("微信账号解除绑定")]
        WxAccountUnBinding = -4,

        /// <summary>
        /// 已经删除
        /// </summary>
        [CustomDescription("已经删除")]
        Deleted= -9
    }
}