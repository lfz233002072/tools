namespace Lfz.Enums.Hr
{
    /// <summary>
    /// 模块权限类型
    /// </summary>
    public enum ModuleRightsType
    {
        /// <summary>
        /// 谁都可以查看
        /// </summary>
        [CustomDescription("公用权限")]
        Common = 0,

        /// <summary>
        /// 可分配的
        /// </summary>
        [CustomDescription("可分配")]
        Assignabled = 1, 

        /// <summary>
        /// 不可分配的权限，系统角色独有
        /// </summary>
        [CustomDescription("不可分配")]
        NonAssignabled=99
    }
}