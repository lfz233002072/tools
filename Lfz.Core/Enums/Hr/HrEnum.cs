namespace Lfz.Enums.Hr
{
    public class HrEnum
    {
     


        #region  国籍类型
        /// <summary> 
        //HR_Employeedetail_Country_Type 户口类型 (0 本地城镇,1 本地农村,2 外地城镇)
        /// </summary>
        public enum HrEmployeedetailCountryType
        {
            /// <summary>
            /// 
            /// </summary>
            中国,
            /// <summary>
            /// 
            /// </summary>
            美国,
            /// <summary>
            /// 
            /// </summary>
            加拿大,
            /// <summary>
            /// 
            /// </summary>
            日本,
            /// <summary>
            /// 
            /// </summary>
            韩国,
            /// <summary>
            /// 
            /// </summary>
            新加坡,
            /// <summary>
            /// 
            /// </summary>
            英国,
            /// <summary>
            /// 
            /// </summary>
            法国,
            /// <summary>
            /// 
            /// </summary>
            德国,
            /// <summary>
            /// 
            /// </summary>
            意大利,
            /// <summary>
            /// 
            /// </summary>
            葡萄牙,
            /// <summary>
            /// 
            /// </summary>
            西班牙,
            /// <summary>
            /// 
            /// </summary>
            澳大利亚
        }
        #endregion

        #region  户口类型 (0 本地城镇,1 本地农村,2 外地城镇)
        /// <summary> 
        //HR_Employeedetail_Registered_Type 户口类型 (0 本地城镇,1 本地农村,2 外地城镇)
        /// </summary>
        public enum HrEmployeedetailRegisteredType
        {
            /// <summary>
            /// 
            /// </summary>
            本地城镇,
            /// <summary>
            /// 
            /// </summary>
            本地农村,
            /// <summary>
            /// 
            /// </summary>
            外地城镇,
            /// <summary>
            /// 
            /// </summary>
            外地农村,
            /// <summary>
            /// 
            /// </summary>
            国外户口
        }
        #endregion

        #region  婚姻状况 (0 未婚,1 已婚,2 离婚 ，3 丧偶)
        /// <summary> 
        //HR_Employeedetail_Marriage 婚姻状况 (0 未婚,1 已婚,2 离婚 ，3 丧偶)
        /// </summary>
        public enum HrEmployeedetailMarriage
        {
            /// <summary>
            /// 
            /// </summary>
            未婚,
            /// <summary>
            /// 
            /// </summary>
            已婚,
            /// <summary>
            /// 
            /// </summary>
            离婚,
            /// <summary>
            /// 
            /// </summary>
            丧偶
        }
        #endregion

        #region  政治条件 (0 群众,1 团员,2 党员,3民主党成员)
        /// <summary> 
        //HR_Employeedetail_Policy 政治条件 (0 群众,1 团员,2 党员,3民主党成员)
        /// </summary>
        public enum HrEmployeedetailPolicy
        {
            ///<summary>
            ///</summary>
            群众,
            ///<summary> 
            ///</summary>
            团员,
            ///<summary>
            ///</summary>
            党员,
            /// <summary>
            /// 
            /// </summary>
            民主党成员
        }
        #endregion

        #region  血型 (0 未婚,1 已婚,2 离婚 ，3 丧偶)
        /// <summary> 
        //HR_Employeedetail_BloodType 血型 (0 A型,1 B型,2 O型 ，3 AB型)
        /// </summary>
        public enum HrEmployeedetailBloodType
        {

            ///<summary>
            ///</summary>
            A型,
            ///<summary>
            ///</summary>
            B型,
            ///<summary>
            ///</summary>
            O型,
            ///<summary>
            ///</summary>
            AB型
        }
        #endregion

        #region  学历 (0 初中,1 中专,2 高中 ，3 大专 ，4 本科 ，5 学士 ，6 硕士 ，7 博士 ，8 博士后 , 9 小学)
        /// <summary> 
        //HR_Employeedetail_Education (0 初中,1 中专,2 高中 ，3 大专 ，4 本科 ，5 学士 ，6 硕士 ，7 博士 ，8 博士后, 9 小学)
        /// </summary>
        public enum HrEmployeedetailEducation
        {
            ///<summary>
            ///</summary>
            初中,
            ///<summary>
            ///</summary>
            中专,
            ///<summary>
            ///</summary>
            高中,
            ///<summary>
            ///</summary>
            大专,
            ///<summary>
            ///</summary>
            本科,
            ///<summary>
            ///</summary>
            研究生,
            ///<summary>
            ///</summary>
            博士,
            ///<summary>
            ///</summary>
            博士后,
            /// <summary>
            /// 
            /// </summary>
            小学
        }
        #endregion

        #region  学位 (0 学士,1 硕士,2 博士)
        /// <summary> 
        ///HR_Employeedetail_Degree 学位 (0 学士,1 硕士,2 博士,3 其它)
        /// </summary>
        public enum HrEmployeedetailDegree
        {
            ///<summary>
            ///</summary>
            学士,
            ///<summary>
            ///</summary>
            硕士,
            ///<summary>
            ///</summary>
            博士,
            /// <summary>
            /// 
            /// </summary>
            其它
        }
        #endregion

        #region 员工状态 (0聘用,1实习,2见习,3外派,4停薪留职,0聘用,-1离职中,-9已离职,-10=已辞退,-11=已流失)
        /// <summary>
        /// HR_EmployeeState 员工状态 (0聘用,1实习,2见习,3外派,4停薪留职,5试用,,6隐藏,-1离职中,-9已离职,-10=已辞退,-11=已流失)
        /// </summary>
        public enum HrEmployeeState
        {
            ///<summary>
            ///</summary>
            聘用 = 0,
            ///<summary>
            ///</summary>
            实习 = 1,
            ///<summary>
            ///</summary>
            见习 = 2,
            ///<summary>
            ///</summary>
            外派 = 3,
            ///<summary>
            ///</summary>
            停薪留职 = 4,
            ///<summary>
            ///</summary>
            试用 = 5,
            /// <summary>
            /// </summary>
            隐藏 = 6,
            ///<summary>
            ///</summary>
            离职中 = -1,
            ///<summary>
            ///</summary>
            已离职 = -9,
            /// <summary>
            /// 
            /// </summary>
            已辞退 = -10,
            /// <summary>
            /// 
            /// </summary>
            已流失 = -11
        }
        #endregion

        #region 员工国籍
        /// <summary>
        /// 员工国籍
        /// </summary>
        public enum HrEmployeedetailNativePlace
        {
            中国,
            美国,
            加拿大,
            新加坡
        }
        #endregion

        #region 员工民族
        /// <summary>
        /// 员工民族
        /// </summary>
        public enum HrEmployeedetailNation
        {
            汉族,
            壮族,
            满族,
            回族,
            苗族,
            维吾尔族,
            土家族,
            彝族,
            蒙古族,
            藏族,
            布依族,
            侗族,
            瑶族,
            朝鲜族,
            白族,
            哈尼族,
            哈萨克族,
            黎族,
            傣族,
            畲族,
            傈僳族,
            仡佬族,
            东乡族,
            拉祜族,
            水族,
            佤族,
            纳西族,
            羌族,
            土族,
            仫佬族,
            锡伯族,
            柯尔克孜族,
            达斡尔族,
            景颇族,
            毛南族,
            撒拉族,
            布朗族,
            塔吉克族,
            阿昌族,
            普米族,
            鄂温克族,
            怒族,
            京族,
            基诺族,
            德昂族,
            保安族,
            俄罗斯族,
            裕固族,
            乌孜别克族,
            门巴族,
            鄂伦春族,
            独龙族,
            塔塔尔族,
            赫哲族,
            高山族,
            珞巴族
        }
        #endregion
    }
}
