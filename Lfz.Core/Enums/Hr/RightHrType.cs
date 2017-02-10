using System;

namespace Lfz.Enums.Hr
{
    /// <summary>
    /// 权限的人事类型，0 人事组织不相关 1:员工 2:部门 4:行政级别 8:部门岗位  32768:其他
    /// </summary> 
    [Flags]
    public enum RightHrType
    {
        /// <summary>
        /// 0 人事组织不相关
        /// </summary>
        None = 0,

        /// <summary>
        /// 1  设置指定员工，【行政级别，岗位，角色】四维权限表
        /// </summary>
        Employee = 1,

        /// <summary>
        /// 2  部门
        /// </summary>
        Department = 2,

        /// <summary>
        /// 4 行政级别
        /// </summary>
        Level = 4,

        /// <summary>
        /// 8 部门岗位
        /// </summary>
        Position = 8,
         
        /// <summary>
        /// 32768  其他
        /// </summary>
        Other = 32768
    }
}