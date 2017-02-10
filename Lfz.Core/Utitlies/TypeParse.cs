//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : TypeParse
//        DESCRIPTION : 类型转化工具
//
//        Created By 林芳崽 at  2012-09-11 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ServiceStack.Text;

namespace Lfz.Utitlies
{
    /// <summary>
    /// 类型转化工具
    /// </summary>
    public static class TypeParse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDefaultValue(object obj)
        {
            if (obj == null) return true;
            var type = obj.GetType();
            if (object.Equals(type.GetDefaultValue(), obj)) return true;
            var result = obj.ToString();
            if (string.IsNullOrEmpty(result) || result == "0" || result == Guid.Empty.ToString()) return true;

            return false;
        }
        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object expression)
        {
            return expression != null && IsNumeric(expression.ToString());
        }

        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(string expression)
        {
            int result;
            return Int32.TryParse(expression, out result);
        }

        /// <summary>
        /// 是否为Double类型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object expression)
        {
            double result;
            return double.TryParse(expression.ToString(), out result);
        }


        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="expression">要转换的字符串</param> 
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(object expression)
        {
            return StrToBool(expression, false);
        }

        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(object expression, bool defValue)
        {
            if (expression == null) return defValue;
            else
            {
                return StrToBool(expression.ToString(), defValue);
            }
        }

        /// <summary>
        /// 布尔类型转换为整型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int BoolToInt(object obj)
        {
            return Convert.ToBoolean(obj) ? 1 : 0;
        }

        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(string expression, bool defValue)
        {
            if (string.IsNullOrEmpty(expression)) return defValue;
            if (expression.Trim() == "1") return true;
            bool result;
            return bool.TryParse(expression, out result) ? result : defValue;
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param> 
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(object expression)
        {
            return StrToInt(expression, 0);
        }

        /// <summary>
        /// 将对象转换为Guid类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param> 
        /// <returns>转换后的int类型结果</returns>
        public static Guid StrToGuid(object expression)
        { 
            if (expression == null || string.IsNullOrEmpty(expression.ToString())) return Guid.Empty;
            Guid value;
            return Guid.TryParse(expression.ToString(), out value) ? value : Guid.Empty; 
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(object expression, int defValue)
        {
            return expression != null ? StrToInt(expression.ToString(), defValue) : defValue;
        }

        /// <summary>
        /// 将字符串转换为Int32类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int StrToInt(string str)
        {

            return StrToInt(str, 0);
        }

        /// <summary>
        /// 转换为Int32范围的整数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static uint StrToUInt(byte[] data, int startIndex, uint maxValue)
        {
            var temp = BitConverter.ToUInt32(data, startIndex);
            return temp < maxValue ? temp : 0;
        }

        /// <summary>
        /// 将无符号整数转换为有符号整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int UIntToInt(uint value)
        {
            if (value > int.MaxValue)
                return  Convert.ToInt32(-value + int.MaxValue);
            else return Convert.ToInt32(value);
        }

        /// <summary>
        /// 将有符号整数转换为无符号整数，负数部分表示 Math.Abs((long)value) + int.MaxValue
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint IntToUInt(int value)
        {
            if (value < 0)
                return Convert.ToUInt32(Math.Abs((long)value) + int.MaxValue);
            else return Convert.ToUInt32(value);
        }

        /// <summary>
        /// 转换为UInt32范围的整数
        /// </summary>
        /// <param name="value"></param> 
        /// <returns></returns>
        public static uint StrToUInt(string value)
        {
            uint temp = 0;
            uint.TryParse(value, out temp);
            return temp;
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string str, int defValue)
        {
            if (string.IsNullOrEmpty(str)) return defValue;
            int result;
            return Int32.TryParse(str, out result) ? result : Int32.TryParse(StrToFloat(str, defValue).ToString(), out result) ? result : defValue;
        }


        /// <summary>
        /// 将对象转换为Int64类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param> 
        /// <returns>转换后的int类型结果</returns>
        public static long StrToInt64(object expression)
        {
            return StrToInt64(expression, 0);
        }

        /// <summary>
        /// 将对象转换为Int64类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static long StrToInt64(object expression, long defValue)
        {
            return expression != null ? StrToInt64(expression.ToString(), defValue) : defValue;
        }

        /// <summary>
        /// 将对象转换为Int64类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static long StrToInt64(string str, long defValue)
        {
            if (string.IsNullOrEmpty(str)) return defValue;
            long result;
            return long.TryParse(str, out result) ? result : long.TryParse(StrToFloat(str, defValue).ToString(), out result) ? result : defValue;
        }


        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param> 
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(object strValue)
        {
            return StrToFloat(strValue, 0);
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(object strValue, float defValue)
        {
            return (strValue == null) ? defValue : StrToFloat(strValue.ToString(), defValue);
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(string strValue, float defValue)
        {
            if (string.IsNullOrEmpty(strValue)) return defValue;
            float result;
            return float.TryParse(strValue, out result) ? result : defValue;
        }

        /// <summary>
        /// string型转换为double型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static double StrToDouble(string strValue, double defValue)
        {
            if (string.IsNullOrEmpty(strValue)) return defValue;
            double result;
            return double.TryParse(strValue, out result) ? result : defValue;
        }
        ///<summary>
        /// string型转换为double型
        ///</summary>
        ///<param name="strValue"></param>
        ///<param name="defValue"></param>
        ///<returns></returns>
        public static double StrToDouble(object strValue, double defValue)
        {
            return (strValue == null) ? defValue : StrToDouble(strValue.ToString(), defValue);
        }

        /// <summary>
        /// string型转为decimal
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static decimal StrToDecimal(object strValue )
        {
            return StrToDecimal(strValue, 0);
        }

        /// <summary>
        /// string型转为decimal
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static decimal StrToDecimal(object strValue, decimal defValue)
        {
            if (strValue == null)
                return defValue;
            try
            {
                decimal.TryParse(strValue.ToString(), out defValue);
            }
            catch { }
            return defValue;
        }

        /// <summary>
        /// String型转换为DateTime型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns></returns>
        public static DateTime StrToDateTime(object strValue, DateTime defValue)
        {
            return strValue == null ? defValue : StrToDateTime(strValue.ToString(), defValue);
        }

        /// <summary>
        /// String型转换为DateTime型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param> 
        /// <returns></returns>
        public static DateTime StrToDateTime(object strValue)
        {
            return StrToDateTime(strValue, DateTime.MinValue);
        }
        /// <summary>
        /// String型转换为DateTime型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns></returns>
        public static DateTime StrToDateTime(string strValue, DateTime defValue)
        {
            if (strValue == null) return defValue;

            try
            {
                return Convert.ToDateTime(strValue);
            }
            catch
            {
                return defValue;
            }
        }

        /// <summary>
        /// 将一个字符串转为为时间
        /// </summary>
        /// <param name="strValue">待转换的值</param>
        /// <param name="defValue">转换失败返回的时间</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(object strValue, DateTime defValue)
        {
            string[] formats = {"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt", 
                   "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss", 
                   "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt", 
                   "M/d/yyyy h:mm", "M/d/yyyy h:mm", 
                   "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm","yyyyMMdd","yyyy-MM-dd", "yyyy-MM-dd hh:mm:ss", 
                   "yyyy-MM-dd HH:mm:ss", "yyyyMMddhhmmss", "yyyyMMddHHmmss"};
            DateTime result;
            string value = Convert.ToString(strValue);
            if (!DateTime.TryParse(value, out result))
            {
                if (!DateTime.TryParseExact(value, formats, new CultureInfo("zh-cn"), DateTimeStyles.None, out result))
                    result = defValue;
            }
            return result;
        }

        /// <summary>
        /// 将一个字符串转为为时间
        /// </summary>
        /// <param name="strValue">待转换的值</param> 
        /// <returns>如果转换失败返回0001/1/1</returns>
        public static DateTime ConvertToDateTime(object strValue)
        {
            return ConvertToDateTime(strValue, DateTime.Now);
        }

       /// <summary>
       /// 转换为时间对象，如果无效格式，那么返回空
       /// </summary>
       /// <param name="strValue"></param>
       /// <returns></returns>
        public static DateTime? ConvertToNullableDateTime(object strValue)
        {
            string[] formats = {"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt", 
                   "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss", 
                   "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt", 
                   "M/d/yyyy h:mm", "M/d/yyyy h:mm", 
                   "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm","yyyyMMdd","yyyy-MM-dd", "yyyy-MM-dd hh:mm:ss", 
                   "yyyy-MM-dd HH:mm:ss", "yyyyMMddhhmmss", "yyyyMMddHHmmss"};
            DateTime result;
            string value = Convert.ToString(strValue);
            if (!DateTime.TryParse(value, out result))
            {
                if (!DateTime.TryParseExact(value, formats, new CultureInfo("zh-cn"), DateTimeStyles.None, out result))
                    return null;
            }
            return result;
        }

        /// <summary>
        /// 时间转换为yyyy-MM-dd
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DateToShortStr(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            DateTime temp;
            if (DateTime.TryParse(str, out temp))
                return temp.ToString("yyyy-MM-dd");
            return str;
        }

        /// <summary>
        /// 输出中文星期
        /// </summary>
        /// <param name="dayOfWeekEn"></param>
        /// <param name="bHavingCss"></param> 
        /// <returns></returns>
        public static string DayOfWeekCh(DayOfWeek dayOfWeekEn, bool bHavingCss)
        {
            if (bHavingCss)
            {
                string[] CnWeekDayOfCSS = {
                                              "<span style='color:#FF0000'>星期天</span>", "星期一", "星期二", "星期三", "星期四",
                                              "星期五",
                                              "<span style='color:#CC5500'>星期六</span>"
                                          };
                return CnWeekDayOfCSS[(int)dayOfWeekEn];

            }
            string[] CnWeekDay = { "星期天", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            return CnWeekDay[(int)dayOfWeekEn];
        }


        /// <summary>
        /// 输出中文星期
        /// </summary>
        /// <param name="dayOfWeekEn"></param>
        /// <returns></returns>
        public static string DayOfWeekCh(DayOfWeek dayOfWeekEn)
        {
            return DayOfWeekCh(dayOfWeekEn, false);
        }


        /// <summary>
        /// 时间字符串（长整数）转换为时间格式（X天X小时X分X秒格式）
        /// </summary>
        /// <param name="timeLong"></param>
        /// <returns></returns>
        public static string TimeToStr(object timeLong)
        {
            return TimeToStr(timeLong, 0);
        }

        /// <summary>
        /// 转秒时间为：天时分秒的格式
        /// </summary>
        /// <param name="timeLong"></param> 
        /// <param name="iUnit"> 默认0 ：XX天XX小时XX分XX秒 ；1：XX小时XX分XX秒 </param> 
        /// <returns></returns>
        public static string TimeToStr(object timeLong, int iUnit)
        {

            string strTime = string.Empty;
            int iTimeLong = StrToInt(timeLong.ToString());
            int iTime = 0;

            if (iUnit == 0)
            {
                iTime = iTimeLong / (24 * 60 * 60);
                if (iTime > 0)
                {
                    strTime += iTime + "天";
                    iTimeLong = iTimeLong % (24 * 60 * 60);
                }

                iTime = iTimeLong / (60 * 60);

            }

            if (iUnit == 1)
                iTime = iTimeLong / (60 * 60);

            if (iTime > 0)
            {
                strTime += iTime + "小时";
                iTimeLong = iTimeLong % (60 * 60);
            }

            iTime = iTimeLong / 60;
            if (iTime > 0)
            {
                strTime += iTime + "分";
                iTimeLong = iTimeLong % 60;
            }
            if (iTimeLong > 0)
                strTime += iTimeLong + "秒";

            return string.IsNullOrEmpty(strTime) ? "0秒" : strTime;
        }


        /// <summary>
        /// 判断给定的字符串数组(strNumber)中的数据是不是都为数值型
        /// </summary>
        /// <param name="strNumber">要确认的字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsNumericArray(string[] strNumber)
        {
            if (strNumber == null)
            {
                return false;
            }
            return strNumber.Length >= 1 && strNumber.All(IsNumeric);
        }

        /// <summary>
        /// 是否为数值串列表，各数值间用","间隔
        /// </summary>
        /// <param name="numList"></param>
        /// <returns></returns>
        public static bool IsNumericList(string numList)
        {
            return numList != "" && numList.Split(',').All(IsNumeric);
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="decimals">精度 最多8位小数</param>
        /// <returns></returns>
        public static double Round(double value, int decimals)
        {
            if (decimals > 8)
                decimals = 8;

            if (value >= 0)
                value += 5 * Math.Pow(10, -(decimals + 1));
            else
                value += -5 * Math.Pow(10, -(decimals + 1));

            string str = value.ToString("0.00000000");
            string[] strs = str.Split('.');
            string postStr = strs[1];
            if (postStr.Length > decimals)
                postStr = str.Substring(str.IndexOf(".") + 1, decimals);

            str = strs[0] + "." + postStr;
            value = Double.Parse(str);
            return value;
        }

        /// <summary>
        /// 四舍五入 小数点后取2位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Round2(double value)
        {
            return Round(value, 2);
        }

        /// <summary>
        /// 四舍五入 小数点后取2位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Round2(string value)
        {
            return Round(TypeParse.StrToDouble(value, 0), 2);
        }

        /// <summary>
        /// 四舍五入 小数点后取2位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Round2(object value)
        {
            return Round(TypeParse.StrToDouble(value, 0), 2);
        }

        /// <summary>
        /// 四舍五入 小数点后取2位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RoundString2(string value)
        {
            return Round2(value).ToString("0.00");
        }

        /// <summary>
        /// 四舍五入 小数点后取2位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RoundString2(double value)
        {
            return Round2(value).ToString("0.00");
        }

        /// <summary>
        /// 四舍五入 小数点后取2位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RoundString2(object value)
        {
            return Round2(value).ToString("0.00");
        }

        /// <summary>
        /// 格式化一个金额类型字符串(2位小数，千位使用逗号分割例如:123,230,000.00)
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static string FormatMoney(object money)
        {
            return string.Format("{0:N}", Round2(money));
        }

        /// <summary>
        /// 格式化一个金额类型字符串(2位小数，千位使用逗号分割例如:123,230,000.00)
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static string FormatMoney(double money)
        {
            return string.Format("{0:N}", Round2(money));
        }

        /// <summary>
        /// 四舍五入 小数点后取4位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Round6(double value)
        {
            return Round(value, 6);
        }

        /// <summary>
        /// 四舍五入 小数点后取4位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Round6(string value)
        {
            return Round(TypeParse.StrToDouble(value, 0), 6);
        }
        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNull(string str)
        {
            if (string.IsNullOrEmpty(str))
                return true;
            if (str.Trim().Length == 0)
                return true;
            return false;
        }

        /// <summary>
        /// 是否为数值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumber(string str)
        {
            if (IsNull(str))
                return false;

            return TypeParse.IsDouble(str);
        }

        /// <summary>
        /// 大于0的数值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsZNumber(string str)
        {
            if (!IsNumber(str))
                return false;

            if (Convert.ToDouble(str) <= 0)
                return false;

            return true;
        }

        /// <summary>
        /// 大等于0的数值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsZNumber0(string str)
        {
            if (!IsNumber(str))
                return false;

            if (Convert.ToDouble(str) < 0)
                return false;

            return true;
        }

        /// <summary>
        /// 是否为整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(string str)
        {
            if (str.IndexOf(".") > -1)
                return false;

            return TypeParse.IsNumeric(str);
        }

        /// <summary>
        /// 大于0的整数值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsZInt(string str)
        {
            if (!IsInt(str))
                return false;

            if (Convert.ToInt32(str) <= 0)
                return false;

            return true;
        }
        /// <summary>
        /// 大等于0的整数值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsZInt0(string str)
        {
            if (!IsInt(str))
                return false;

            if (Convert.ToInt32(str) < 0)
                return false;

            return true;
        }


        /// <summary>
        /// 是否为日期
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDate(object obj)
        {
            string strDate = obj.ToString();
            try
            {
                DateTime dt = DateTime.Parse(strDate);
                if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 短日期 2000-01-01
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ShortDate(string date)
        {
            DateTime? dat = TypeParse.StrToDateTime(date, DateTime.Now);
            if (!dat.HasValue) dat = DateTime.Now;
            return ShortDate(dat.Value);
        }

        /// <summary>
        /// 短日期 2000-01-01
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ShortDate(object date)
        {
            return ShortDate(Convert.ToString(date));
        }

        /// <summary>
        /// 短日期 2000-01-01
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ShortDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 长日期 2000-01-01 24:59:59
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string LongDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 长日期 2000-01-01 24:59:59
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string LongDate(string date)
        {
            DateTime? dat = TypeParse.StrToDateTime(date, DateTime.Now);
            if (!dat.HasValue) dat = DateTime.Now;
            return LongDate(dat.Value);
        }

        /// <summary>
        /// 长日期 2000-01-01 24:59:59
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string LongDate(object date)
        {
            return LongDate(Convert.ToString(date));
        }

        /// <summary>
        /// 只取年和月 2000-01
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string DateFormatYearMonth(DateTime date)
        {
            return date.ToString("yyyy-MM");
        }

        /// <summary>
        /// 只取年和月 2000-01
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string DateFormatYearMonth(string date)
        {
            DateTime? dat = TypeParse.StrToDateTime(date, DateTime.Now);
            if (!dat.HasValue) dat = DateTime.Now;
            return DateFormatYearMonth(dat.Value);
        }

        /// <summary>
        /// 输入时间格式内容 返回 yyyy-MM 格式字符串
        /// </summary>
        /// <param name="date">时间格式内容</param>
        /// <returns> yyyy-MM 格式字符串</returns>
        public static string DateFormatYearMonth(object date)
        {
            return DateFormatYearMonth(Convert.ToString(date) + string.Empty);
        }



        /// <summary>
        /// 备注文字显示
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string DescripText(string text)
        {
            return text.Replace("\r\n", "<br/>");
        }

        /// <summary>
        /// 是否为有效的年月（yyyy-MM）格式日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsValidYearMonth(string date)
        {
            var flag = false;
            Regex reg = new Regex(@"^(\d{4})(-|\/)(\d{1,2})$");
            flag = reg.IsMatch(date);
            return flag;
        }


        /// <summary>
        /// 查询的开始时间
        /// </summary>
        /// <param name="date"></param>
        public static string StartDateForQuery(DateTime date)
        {
            return "to_date('" + ShortDate(date) + "','yyyy-mm-dd') ";
        }

        /// <summary>
        /// 查询的结束时间
        /// </summary>
        /// <param name="date"></param>
        public static string EndDateForQuery(DateTime date)
        {
            return "to_date('" + LongDate(date).Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
        }

        /// <summary>
        /// 查询的开始时间
        /// </summary>
        /// <param name="date"></param>
        public static string StartDateForQuery(string date)
        {
            return StartDateForQuery(TypeParse.StrToDateTime(date));
        }

        /// <summary>
        /// 查询的结束时间
        /// </summary>
        /// <param name="date"></param>
        public static string EndDateForQuery(string date)
        {
            return EndDateForQuery(TypeParse.StrToDateTime(date));
        }

        /// <summary>
        /// 本月第一天
        /// </summary>
        public static DateTime ThisMonthFirstDate()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }

        /// <summary>
        /// 本年第一天
        /// </summary>
        public static DateTime ThisYearFirstDate()
        {
            return new DateTime(DateTime.Now.Year, 1, 1);
        }

        /// <summary>
        /// 当前时间段所在会计区间起始日期
        /// </summary>
        public static DateTime ThisFinanceFirstDate()
        {
            return FinanceFirstDate(DateTime.Now);
        }

        /// <summary>
        /// 指定日期所在会计区间起始日期
        /// </summary>
        /// <param name="dat"></param>
        /// <returns></returns>
        public static DateTime FinanceFirstDate(DateTime dat)
        {
            DateTime date = new DateTime(dat.Year, dat.Month, 28);
            if (dat.Day <= 28)//每月的28号为截止日
            {
                date = date.AddMonths(-1).AddDays(1);//上个月29号
            }
            else
            {
                date = date.AddDays(1);//这个月29号
            }

            return date;
        }

        /// <summary>
        /// 获取两个时间月份差绝对值+1
        /// </summary>
        /// <param name="fromDate">其实时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>获取两个时间月份差绝对值+1</returns>
        public static int GetDiffMoneths(DateTime fromDate, DateTime endDate)
        {
            return Math.Abs((fromDate.Year - endDate.Year) * 12 + (fromDate.Month - endDate.Month)) + 1;
        }

        /// <summary>
        /// 一周星开始时间
        /// </summary>
        /// <param name="date"></param> 
        /// <returns></returns>
        public static DateTime GetBeginningWeek(DateTime date)
        {
            return GetBeginningWeek(date, 1);
        }

        /// <summary>
        /// 一周星开始时间
        /// </summary>
        /// <param name="date"></param>
        /// <param name="firstDay">星期几作为一周的开始时间</param>
        /// <returns></returns>
        public static DateTime GetBeginningWeek(DateTime date, int firstDay)
        {
            int weekOrder = (int)date.DayOfWeek;
            //星期天计数设置为7
            if (weekOrder == 0) weekOrder = 7;

            if (firstDay == 0) firstDay = 7;
            int diff = firstDay - weekOrder;
            //如果diff大于0，则应该去上一个周期的数字
            if (diff > 0) diff = diff - 7;
            return new DateTime(date.Year, date.Month, date.Day).AddDays(diff);
        }
         
        /// <summary>
        /// 将对象转换为指定类型对象
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ConvertTo<T>(this object obj)
        {
            object result;
            try
            {
                result = Convert.ChangeType(obj, typeof(T), null);
            }
            catch
            {
                result = default(T);
            }

            return (T)result;
        }
    }
}