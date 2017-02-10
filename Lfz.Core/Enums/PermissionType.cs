namespace PMSoft.Enums
{
    /// <summary>
    /// 权限类型 0公用权限，1是可以被分配的，99 其它
    /// </summary>
    public enum PermissionType
    {
        /// <summary>
        /// 公用权限,共享权限=0
        /// </summary>
        Shared = 0,

        /// <summary>
        /// 可分配权限=1
        /// </summary>
        Assignassignable = 1,
        /// <summary>
        /// 其它权限
        /// </summary>
        Other = 99,
        /// <summary>
        /// 默认权限=0
        /// </summary>
        Default = Shared
    }
}