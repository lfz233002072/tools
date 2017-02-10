//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : Utils
//        DESCRIPTION : 工具箱类
//
//        Created By 林芳崽 at  2012-08-25 19:32:40
//        https://git.oschina.net/lfz/tools
//
//====================================================================== 

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lfz.Logging;

namespace Lfz.Utitlies
{
    /// <summary>
    /// 辅助工具箱，包括IP获取、字符转换、枚举处理等
    /// </summary>
    public partial class Utils
    {
        /// <summary>
        /// 日志处理
        /// </summary>
        private static readonly ILogger Logger;

        static Utils()
        {
            Logger = LoggerFactory.GetLog();
        }

        #region 把REPEATER导出EXCEL浏览器输出----REPEATER控件中不能有列排序的服务器控件
        /// <summary>
        /// 把REPEATER导出EXCEL浏览器输出----REPEATER控件中不能有列排序的服务器控件
        /// </summary>
        /// <param name="repeaterName"></param>
        /// <param name="repeaterSource"></param>
        public static void ResponseExcel(string repeaterName, Repeater repeaterSource)
        {
            if (repeaterName == "")
                repeaterName = DateTime.Now.ToShortDateString();
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.Charset = "gb2312";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(repeaterName + ".xls", System.Text.Encoding.UTF8).ToString());
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            repeaterSource.RenderControl(htw);
            HttpContext.Current.Response.Write(sw.ToString());
            HttpContext.Current.Response.End();
        }
        #endregion

        #region 将字符串转换为Color

        /// <summary>
        /// 将字符串转换为Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToColor(string color)
        {
            int red, green, blue = 0;
            char[] rgb;
            color = color.TrimStart('#');
            color = Regex.Replace(color.ToLower(), "[g-zG-Z]", "");
            switch (color.Length)
            {
                case 3:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[0].ToString(), 16);
                    green = Convert.ToInt32(rgb[1].ToString() + rgb[1].ToString(), 16);
                    blue = Convert.ToInt32(rgb[2].ToString() + rgb[2].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                case 6:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[1].ToString(), 16);
                    green = Convert.ToInt32(rgb[2].ToString() + rgb[3].ToString(), 16);
                    blue = Convert.ToInt32(rgb[4].ToString() + rgb[5].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                default:
                    return Color.FromName(color);

            }
        }

        #endregion

        #region 随机密码生成功

        /// <summary>
        /// 随机密码生成功
        /// </summary>
        /// <param name="pwdchars"></param>
        /// <param name="pwdlen"></param>
        /// <returns></returns>
        public static string MakePassword(int pwdlen, string pwdchars = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            string tmpstr = "";
            int iRandNum;
            Random rnd = new Random();
            for (int i = 0; i < pwdlen; i++)
            {
                iRandNum = rnd.Next(pwdchars.Length);
                tmpstr += pwdchars[iRandNum];
            }
            return tmpstr;
        }

        /// <summary>
        /// 生成随机验证码
        /// </summary>
        /// <param name="pwdchars"></param>
        /// <returns></returns>
        public static string MakeVerifyCode(int pwdlen = 6, string pwdchars = "0123456789")
        {
            return MakePassword(pwdlen, pwdchars);
        }

        #endregion

        #region 替换html字符

        /// <summary>
        /// 替换html字符
        /// </summary>
        public static string EncodeHtml(string strHtml)
        {
            if (strHtml != "")
            {
                strHtml = strHtml.Replace(",", "&def");
                strHtml = strHtml.Replace("'", "&dot");
                strHtml = strHtml.Replace(";", "&dec");
                return strHtml;
            }
            return "";
        }
        #endregion

        #region 将字符串转(ASCII码字符串)换为字节码

        /// <summary>
        /// 将字符串转换为字节码
        /// </summary>
        /// <param name="s">ASCII码字符串</param>
        /// <returns>ASCII码字符串对应的字节码</returns>
        public static byte[] StringToBytes(string s)
        {
            byte[] tmp = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
                tmp[i] = (byte)s[i];
            return tmp;
        }

        #endregion

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 获取指定字符串中的第一个Ip地址
        /// </summary>
        /// <param name="searchAddress"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool GetFirstIpAddress(string searchAddress, out string ip)
        {
            bool flag = false;
            ip = "127.0.0.1";
            MatchCollection mcList =
                new Regex(@"((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)").Matches(searchAddress);
            if (mcList.Count > 0)
            {
                ip = mcList[0].Value;
                flag = true;
            }
            return flag;
        }

        public static bool IsIPSect(string ip)
        {
            return Regex.IsMatch(ip,
                                 @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");
        }


        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                {
                    string[] tmp = { strContent };
                    return tmp;
                }
                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
            {
                return new string[0] { };
            }
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int count)
        {
            var result = new string[count];

            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// 过滤字符串数组中每个元素为合适的大小
        /// 当长度小于minLength时，忽略掉,-1为不限制最小长度
        /// 当长度大于maxLength时，取其前maxLength位
        /// 如果数组中有null元素，会被忽略掉
        /// </summary>
        /// <param name="minLength">单个元素最小长度</param>
        /// <param name="maxLength">单个元素最大长度</param>
        /// <returns></returns>
        public static string[] PadStringArray(string[] strArray, int minLength, int maxLength)
        {
            if (minLength > maxLength)
            {
                int t = maxLength;
                maxLength = minLength;
                minLength = t;
            }

            int iMiniStringCount = 0;
            for (int i = 0; i < strArray.Length; i++)
            {
                if (minLength > -1 && strArray[i].Length < minLength)
                {
                    strArray[i] = null;
                    continue;
                }
                if (strArray[i].Length > maxLength)
                {
                    strArray[i] = strArray[i].Substring(0, maxLength);
                }
                iMiniStringCount++;
            }

            var result = new string[iMiniStringCount];
            for (int i = 0, j = 0; i < strArray.Length && j < result.Length; i++)
            {
                if (strArray[i] != null && strArray[i] != string.Empty)
                {
                    result[j] = strArray[i];
                    j++;
                }
            }


            return result;
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">被分割的字符串</param>
        /// <param name="strSplit">分割符</param>
        /// <param name="ignoreRepeatItem">忽略重复项</param>
        /// <param name="maxElementLength">单个元素最大长度</param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem,
                                           int maxElementLength)
        {
            string[] result = SplitString(strContent, strSplit);

            return ignoreRepeatItem ? DistinctStringArray(result, maxElementLength) : result;
        }

        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem,
                                           int minElementLength, int maxElementLength)
        {
            string[] result = SplitString(strContent, strSplit);

            if (ignoreRepeatItem)
            {
                result = DistinctStringArray(result);
            }
            return PadStringArray(result, minElementLength, maxElementLength);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">被分割的字符串</param>
        /// <param name="strSplit">分割符</param>
        /// <param name="ignoreRepeatItem">忽略重复项</param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem)
        {
            return SplitString(strContent, strSplit, ignoreRepeatItem, 0);
        }

        /// <summary>
        /// 清除字符串数组中的重复项
        /// </summary>
        /// <param name="strArray">字符串数组</param>
        /// <param name="maxElementLength">字符串数组中单个元素的最大长度</param>
        /// <returns></returns>
        public static string[] DistinctStringArray(string[] strArray, int maxElementLength)
        {
            var h = new Hashtable();

            foreach (string s in strArray)
            {
                string k = s;
                if (maxElementLength > 0 && k.Length > maxElementLength)
                {
                    k = k.Substring(0, maxElementLength);
                }
                h[k.Trim()] = s;
            }

            var result = new string[h.Count];

            h.Keys.CopyTo(result, 0);

            return result;
        }

        /// <summary>
        /// 清除字符串数组中的重复项
        /// </summary>
        /// <param name="strArray">字符串数组</param>
        /// <returns></returns>
        public static string[] DistinctStringArray(string[] strArray)
        {
            return DistinctStringArray(strArray, 0);
        }


        /// <summary>
        /// 返回指定IP是否在指定的IP数组所限定的范围内, IP数组内的IP地址可以使用*表示该IP段任意, 例如192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static bool InIPArray(string ip, string[] iparray)
        {
            string[] userip = SplitString(ip, @".");
            for (int ipIndex = 0; ipIndex < iparray.Length; ipIndex++)
            {
                string[] tmpip = SplitString(iparray[ipIndex], @".");
                int r = 0;
                for (int i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*")
                    {
                        return true;
                    }

                    if (userip.Length > i)
                    {
                        if (tmpip[i] == userip[i])
                        {
                            r++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (r == 4)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns>字符长度</returns>
        public static int GetStringLength(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }

        #region 获取本机IP地址列表

        /// <summary>
        /// 获取本机IP地址列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetLocalAddresses()
        {
            // Get host name
            string strHostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

            string[] retval = new string[iphostentry.AddressList.Length];

            int i = 0;
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                retval[i] = ipaddress.ToString();
                i++;
            }
            return retval;
        }

        #endregion

        #region 将结构类型对象转换为字节码

        /// <summary>
        /// struct转换为byte[]
        /// </summary>
        /// <param name="structObj"></param>
        /// <returns></returns>
        public static byte[] StructToBytes(object structObj)
        {
            //得到结构体大小
            int size = Marshal.SizeOf(structObj);
            //分配结构体大小的内存空间
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                //将结构体拷到分配好的内存空间
                Marshal.StructureToPtr(structObj, buffer, false);
                //创建BYTE数组
                byte[] bytes = new byte[size];
                //从内存空间拷贝到BUTE数组
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                //释放内存空间
                Marshal.FreeHGlobal(buffer);
            }
        }

        #endregion

        #region 将字节码转换为指定类型对象(结构体)

        /// <summary>
        /// byte[]转换为struct
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="strcutType"></param>
        /// <returns></returns>
        private static object BytesToStruct(byte[] bytes, Type strcutType)
        {
            //得到结构体大小
            int size = Marshal.SizeOf(strcutType);
            if (size != bytes.Length)
            {
                return null;
            }
            //分配结构体大小的内存空间
            IntPtr buffer = Marshal.AllocHGlobal(size);

            try
            {
                //将BYTE数组拷贝到分配好的内存空间
                Marshal.Copy(bytes, 0, buffer, size);
                //将内存空间转换为目标结构体
                return Marshal.PtrToStructure(buffer, strcutType);
            }
            catch (Exception e)
            {
                Logger.Error(e, String.Format("BytesToStruct构造失败,Type:{0} TypeSize:{2} bytes Size:{3} ErrorMessage: {1} ", strcutType, e.Message, size, bytes.Length));
                return null;
            }
            finally
            {
                //释放内存空间
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// byte[]转换为struct
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T BytesToStruct<T>(byte[] bytes) where T : struct
        {
            Type strcutType = typeof(T);
            object o = BytesToStruct(bytes, strcutType);
            if (o != null && o is T) return (T)o;
            else
            {
                Logger.Error(String.Format("BytesToStruct构造失败,Type:{0} bytesLength:{1}", strcutType, bytes == null ? 0 : bytes.Length));
                return default(T);
            }
        }

        #endregion

        #region 获取MAC地址

        /// <summary>
        /// 获取MAC地址
        /// </summary>
        /// <returns></returns>
        public static string GetNetCardMacAddress()
        {
            var str = "";
            try
            {
                NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
                if (nis.Length > 0) str = nis[0].GetPhysicalAddress().ToString();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            { }
            return str;
        }
        /// <summary>
        /// 获取本地IP地址信息
        /// </summary>
        public static IEnumerable<string> GetAddressIPs()
        {
            var list = new List<string>(); 
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    list.Add(_IPAddress.ToString());
                }
            }
            return list;
        }
        #endregion


        #region 将16进制字符串转换为字节码

        /// <summary>
        /// 将16进制字符串转换为字节码
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] StrToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        #endregion

        #region 将字节码转换为16进制字符串

        /// <summary>
        /// 将字节码转换为16进制字符串
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public static string ByteToHex(byte[] buf)
        {
            StringBuilder ret = new StringBuilder();
            if (buf != null)
            {
                for (int i = 0; i < buf.Length; i++)
                {
                    ret.Append(buf[i].ToString("X2") + " ");
                }
            }
            return ret.ToString();
        }

        #endregion

        #region 获取ASCII码字符串（特殊字符串+数字键+26字母 ASCII值 IN[32，126]）

        /// <summary>
        /// 返回buf的ASCII字符串，移除其中的控制位(ASCII值 IN[0,31] 或[127])。
        /// </summary>
        /// <param name="buf">需要转发为字符串的字节码数组</param>
        /// <returns>返回ASCII字符串</returns>
        public static string GetASCIIString(byte[] buf)
        {
            if (buf == null)
            {
                return string.Empty;
            }
            //127 为DEL 控制位
            //0 为NUL 空
            //1-31为其它控制位
            return Encoding.ASCII.GetString(buf.Where(x => x > 31 && x < 127).ToArray());
        }

        /// <summary>
        /// 返回buf的ASCII字符串，移除其中的控制位(ASCII值 IN[0,31] 或[127])。
        /// </summary>
        /// <param name="buf">需要转发为字符串的字节码数组</param>
        /// <returns>返回ASCII字符串</returns>
        public static string GetUnicodeString(byte[] buf)
        {
            if (buf == null)
            {
                return string.Empty;
            }
            return Encoding.Unicode.GetString(buf, 0, buf.Length).Trim().Trim('\0');
        }


        /// <summary>
        ///  
        /// </summary>
        /// <param name="buf">需要转发为字符串的字节码数组</param>
        /// <returns>返回ASCII字符串</returns>
        public static string GetDefaultString(byte[] buf)
        {
            if (buf == null)
            {
                return string.Empty;
            }
            return Encoding.Default.GetString(buf, 0, buf.Length).Trim().Trim('\0');
        }

        /// <summary>
        /// 获取Unicode子串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <param name="pTailString"></param>
        /// <returns></returns>
        public static string GetUnicodeSubString(string str, int len, string pTailString)
        {
            var result = string.Empty;// 最终返回的结果
            var byteLen = Encoding.Default.GetByteCount(str);// 单字节字符长度
            var charLen = str.Length;// 把字符平等对待时的字符串长度
            var byteCount = 0;// 记录读取进度
            var pos = 0;// 记录截取位置
            if (byteLen > len)
            {
                for (var i = 0; i < charLen; i++)
                {
                    if (Convert.ToInt32(str.ToCharArray()[i]) > 255)// 按中文字符计算加2
                        byteCount += 2;
                    else// 按英文字符计算加1
                        byteCount += 1;
                    if (byteCount > len)
                    {
                        pos = i;
                        break;
                    }
                    if (byteCount != len) continue;
                    pos = i + 1;
                    break;
                }

                if ((0 > pos)) return result;
                result = str.Substring(0, pos) + pTailString;
            }
            else
                result = str;

            return result;
        }

        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="pSrcString">要检查的字符串</param>
        /// <param name="pStartIndex">起始位置</param>
        /// <param name="pLength">指定长度</param>
        /// <param name="pTailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string pSrcString, int pStartIndex, int pLength, string pTailString)
        {
            string myResult = pSrcString;

            var bComments = Encoding.UTF8.GetBytes(pSrcString);
            if (Encoding.UTF8.GetChars(bComments).Any(c => (c > '\u0800' && c < '\u4e00') || (c > '\xAC00' && c < '\xD7A3')))
            {
                return pStartIndex >= pSrcString.Length
                           ? ""
                           : pSrcString.Substring(pStartIndex,
                                                  ((pLength + pStartIndex) > pSrcString.Length)
                                                      ? (pSrcString.Length - pStartIndex)
                                                      : pLength);
            }


            if (pLength >= 0)
            {
                var bsSrcString = Encoding.Default.GetBytes(pSrcString);

                //当字符串长度大于起始位置
                if (bsSrcString.Length > pStartIndex)
                {
                    var pEndIndex = bsSrcString.Length;

                    //当要截取的长度在字符串的有效长度范围内
                    if (bsSrcString.Length > (pStartIndex + pLength))
                    {
                        pEndIndex = pLength + pStartIndex;
                    }
                    else
                    {   //当不在有效范围内时,只取到字符串的结尾

                        pLength = bsSrcString.Length - pStartIndex;
                        pTailString = "";
                    }



                    var nRealLength = pLength;
                    var anResultFlag = new int[pLength];

                    var nFlag = 0;
                    for (var i = pStartIndex; i < pEndIndex; i++)
                    {

                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                            {
                                nFlag = 1;
                            }
                        }
                        else
                        {
                            nFlag = 0;
                        }

                        anResultFlag[i] = nFlag;
                    }

                    if ((bsSrcString[pEndIndex - 1] > 127) && (anResultFlag[pLength - 1] == 1))
                    {
                        nRealLength = pLength + 1;
                    }

                    var bsResult = new byte[nRealLength];

                    Array.Copy(bsSrcString, pStartIndex, bsResult, 0, nRealLength);

                    myResult = Encoding.Default.GetString(bsResult);

                    myResult = myResult + pTailString;
                }
            }

            return myResult;
        }

        #endregion

        #region 解析Http内容


        /// <summary>
        /// 解析HTTP响应信息
        /// </summary> 
        /// <param name="recContent"></param>
        /// <param name="contentLength">如果响应状态为OK，那么为Http Body(Bytes)长度</param>
        /// <param name="body">如果响应状态为OK，那么为Http Body内容</param>
        /// <returns>Http响应状态</returns>
        public static HttpStatusCode ParseHttpResponse(string recContent, out int contentLength, out string body)
        {
            HttpStatusCode statusCode = HttpStatusCode.NotImplemented;
            contentLength = 0;
            body = string.Empty;
            string[] contentAttr = Regex.Split(recContent, "\r\n\r\n");
            //内容是否包含Http头部.Http头部和Http内容是用"\r\n\r\n"分割
            if (contentAttr.Length < 0) return statusCode;
            string headerContent = contentAttr[0].ToUpper();
            //HTTP 头部每条记录是用"\r\n"分割
            string[] headerArr = Regex.Split(headerContent, "\r\n");
            if (headerArr.Length > 2)
            {
                var codeList = headerArr[0].Split(' ');
                if (codeList.Length > 1)
                {
                    if (Enum.IsDefined(typeof(HttpStatusCode), TypeParse.StrToInt(codeList[1])))
                    {
                        statusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), codeList[1]);
                    }
                }
                if (statusCode == HttpStatusCode.OK)
                {
                    //获取Body长度
                    var lengthLine = headerArr.FirstOrDefault(x => x.Contains("CONTENT-LENGTH"));
                    if (!string.IsNullOrEmpty(lengthLine))
                    {
                        contentLength = TypeParse.StrToInt(lengthLine.Replace("CONTENT-LENGTH:", ""));
                        //剔除Http头部信息外全部为Http内容
                        body = recContent.Substring(headerContent.Length + 4);
                    }
                }
            }
            return statusCode;
        }

        /// <summary>
        /// 获取HTTP响应
        /// </summary>
        /// <param name="networkStream"></param>
        /// <param name="maxRecBufSize">最大接收缓存大小</param>
        /// <param name="encoding">编码机制</param>
        /// <returns></returns>
        public static string GetResponse(NetworkStream networkStream, int maxRecBufSize, Encoding encoding)
        {
            var buff = new byte[maxRecBufSize];
            int offSize = 0;
            while (offSize < maxRecBufSize)
            {
                try
                {
                    //一次性最多读取1024个字节
                    int count = Math.Min(1024, maxRecBufSize - offSize);
                    //保证读取的数据不超过_maxRecBufSize。 
                    int readCount = networkStream.Read(buff, offSize, count);
                    if (readCount > 0) offSize += readCount;
                    else break;
                }
                catch (Exception e)
                {
                    Logger.Error("Utils.GetResponse Errormessage:" + e.Message);
                    break;
                }
            }
            return encoding.GetString(buff, 0, offSize);
        }
        #endregion

        #region 获取对象的DisplayNameAttribute属性值（如果为空，那么取类型全称）

        /// <summary>
        /// 获取对象的DisplayNameAttribute属性值（如果为空，那么取类型全称）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetDisplayName(object obj)
        {
            if (obj == null) return string.Empty;
            var t = obj.GetType();
            var desc = t.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault();
            if (desc != null) return ((DisplayNameAttribute)desc).DisplayName;
            else return t.FullName;
        }

        #endregion


        #region 文件管理

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public static string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }
            else
            {
                //not hosted. For example, run in unit tests
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
                return Path.Combine(baseDirectory, path);
            }
        }

        #endregion

        #region 检查指定文件是否存在。如果不存在，那么创建新文件

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public static bool CheckFileExists(string filename)
        {
            bool flag = true;
            try
            {
                if (!File.Exists(filename))
                {
                    int index = filename.LastIndexOf('/');
                    string path = filename.Substring(0, index);
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    File.Create(filename);
                }
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 检查目录是否存在，如果不存在，那么创建。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckDirectoryExists(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    result = false;
                    Logger.Warning("创建文件目录失败: {0}", e.Message);
                }
            return result;
        }
        #endregion

        #region 获取应用程序是否以完全信任级别运行

        /// <summary>
        /// 获取应用程序是否以完全信任级别运行
        /// </summary>
        public static bool IsFullTrust
        {
            get { return AppDomain.CurrentDomain.IsHomogenous && AppDomain.CurrentDomain.IsFullyTrusted; }
        }

        #endregion

        #region 加密方式


        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string Md5(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            var b = Encoding.UTF8.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            var ret = "";
            for (var i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }

        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果(返回长度为44字节的字符串)</returns>
        public static string SHA256(string str)
        {
            var sha256Data = Encoding.UTF8.GetBytes(str);
            var sha256 = new SHA256Managed();
            var result = sha256.ComputeHash(sha256Data);
            return Convert.ToBase64String(result);  //返回长度为44字节的字符串
        }

        #endregion

        #region 数据库连接测试

        /// <summary>
        /// 测试连接数据库是否成功
        /// </summary>
        /// <returns>true :数据库连接正常 false:连接失败</returns>
        public static bool DbConnectionTest(string connectionString)
        {
            //创建连接对象
            SqlConnection mySqlConnection = null;
            //ConnectionTimeout 在.net 1.x 可以设置 在.net 2.0后是只读属性，则需要在连接字符串设置
            //如：server=.;uid=sa;pwd=;database=PMIS;Integrated Security=SSPI; Connection Timeout=30
            //mySqlConnection.ConnectionTimeout = 1;//设置连接超时的时间
            try
            {
                mySqlConnection = new SqlConnection(connectionString);
                mySqlConnection.Open();
                //mySqlConnection.Database;
                // mySqlConnection.DataSource,

                return true;
            }
            catch
            {
            }
            finally
            {
                if (mySqlConnection != null)
                    mySqlConnection.Close();
            }
            return false;
        }


        #endregion

        #region 获取等待天数

        /// <summary>
        /// 获取等待天数
        /// </summary>
        /// <param name="inputTime"></param>
        /// <returns></returns>
        public static string GetWaitingTimes(DateTime? inputTime)
        {
            if (inputTime.HasValue == false) return string.Empty;
            string result = "";
            var timespan = DateTime.Now - inputTime.Value;
            if (timespan.TotalDays >= 1)
            {
                result += string.Format("{0}天", (int)timespan.TotalDays);
            }
            if (timespan.Hours >= 1)
            {
                result += string.Format("{0}小时", (int)timespan.Hours);
            }
            if (timespan.Minutes >= 1)
            {
                result += string.Format("{0}分钟", (int)timespan.Minutes);
            }
            return result;
        }

        #endregion

        #region 拼音，汉字处理

        public static string GetSpell(string x)
        {
            return GetSpell(x, false);
        }

        /// <summary>
        /// 获取汉字的全拼
        /// </summary>
        /// <param name="x"></param>
        /// <param name="isAbbr"> </param>
        /// <returns></returns>
        public static string GetSpell(string x, bool isAbbr)
        {
            var iA = new[]
              {
                  -20319 ,-20317 ,-20304 ,-20295 ,-20292 ,-20283 ,-20265 ,-20257 ,-20242 ,-20230
                  ,-20051 ,-20036 ,-20032 ,-20026 ,-20002 ,-19990 ,-19986 ,-19982 ,-19976 ,-19805
                  ,-19784 ,-19775 ,-19774 ,-19763 ,-19756 ,-19751 ,-19746 ,-19741 ,-19739 ,-19728
                  ,-19725 ,-19715 ,-19540 ,-19531 ,-19525 ,-19515 ,-19500 ,-19484 ,-19479 ,-19467
                  ,-19289 ,-19288 ,-19281 ,-19275 ,-19270 ,-19263 ,-19261 ,-19249 ,-19243 ,-19242
                  ,-19238 ,-19235 ,-19227 ,-19224 ,-19218 ,-19212 ,-19038 ,-19023 ,-19018 ,-19006
                  ,-19003 ,-18996 ,-18977 ,-18961 ,-18952 ,-18783 ,-18774 ,-18773 ,-18763 ,-18756
                  ,-18741 ,-18735 ,-18731 ,-18722 ,-18710 ,-18697 ,-18696 ,-18526 ,-18518 ,-18501
                  ,-18490 ,-18478 ,-18463 ,-18448 ,-18447 ,-18446 ,-18239 ,-18237 ,-18231 ,-18220
                  ,-18211 ,-18201 ,-18184 ,-18183 ,-18181 ,-18012 ,-17997 ,-17988 ,-17970 ,-17964
                  ,-17961 ,-17950 ,-17947 ,-17931 ,-17928 ,-17922 ,-17759 ,-17752 ,-17733 ,-17730
                  ,-17721 ,-17703 ,-17701 ,-17697 ,-17692 ,-17683 ,-17676 ,-17496 ,-17487 ,-17482
                  ,-17468 ,-17454 ,-17433 ,-17427 ,-17417 ,-17202 ,-17185 ,-16983 ,-16970 ,-16942
                  ,-16915 ,-16733 ,-16708 ,-16706 ,-16689 ,-16664 ,-16657 ,-16647 ,-16474 ,-16470
                  ,-16465 ,-16459 ,-16452 ,-16448 ,-16433 ,-16429 ,-16427 ,-16423 ,-16419 ,-16412
                  ,-16407 ,-16403 ,-16401 ,-16393 ,-16220 ,-16216 ,-16212 ,-16205 ,-16202 ,-16187
                  ,-16180 ,-16171 ,-16169 ,-16158 ,-16155 ,-15959 ,-15958 ,-15944 ,-15933 ,-15920
                  ,-15915 ,-15903 ,-15889 ,-15878 ,-15707 ,-15701 ,-15681 ,-15667 ,-15661 ,-15659
                  ,-15652 ,-15640 ,-15631 ,-15625 ,-15454 ,-15448 ,-15436 ,-15435 ,-15419 ,-15416
                  ,-15408 ,-15394 ,-15385 ,-15377 ,-15375 ,-15369 ,-15363 ,-15362 ,-15183 ,-15180
                  ,-15165 ,-15158 ,-15153 ,-15150 ,-15149 ,-15144 ,-15143 ,-15141 ,-15140 ,-15139
                  ,-15128 ,-15121 ,-15119 ,-15117 ,-15110 ,-15109 ,-14941 ,-14937 ,-14933 ,-14930
                  ,-14929 ,-14928 ,-14926 ,-14922 ,-14921 ,-14914 ,-14908 ,-14902 ,-14894 ,-14889
                  ,-14882 ,-14873 ,-14871 ,-14857 ,-14678 ,-14674 ,-14670 ,-14668 ,-14663 ,-14654
                  ,-14645 ,-14630 ,-14594 ,-14429 ,-14407 ,-14399 ,-14384 ,-14379 ,-14368 ,-14355
                  ,-14353 ,-14345 ,-14170 ,-14159 ,-14151 ,-14149 ,-14145 ,-14140 ,-14137 ,-14135
                  ,-14125 ,-14123 ,-14122 ,-14112 ,-14109 ,-14099 ,-14097 ,-14094 ,-14092 ,-14090
                  ,-14087 ,-14083 ,-13917 ,-13914 ,-13910 ,-13907 ,-13906 ,-13905 ,-13896 ,-13894
                  ,-13878 ,-13870 ,-13859 ,-13847 ,-13831 ,-13658 ,-13611 ,-13601 ,-13406 ,-13404
                  ,-13400 ,-13398 ,-13395 ,-13391 ,-13387 ,-13383 ,-13367 ,-13359 ,-13356 ,-13343
                  ,-13340 ,-13329 ,-13326 ,-13318 ,-13147 ,-13138 ,-13120 ,-13107 ,-13096 ,-13095
                  ,-13091 ,-13076 ,-13068 ,-13063 ,-13060 ,-12888 ,-12875 ,-12871 ,-12860 ,-12858
                  ,-12852 ,-12849 ,-12838 ,-12831 ,-12829 ,-12812 ,-12802 ,-12607 ,-12597 ,-12594
                  ,-12585 ,-12556 ,-12359 ,-12346 ,-12320 ,-12300 ,-12120 ,-12099 ,-12089 ,-12074
                  ,-12067 ,-12058 ,-12039 ,-11867 ,-11861 ,-11847 ,-11831 ,-11798 ,-11781 ,-11604
                  ,-11589 ,-11536 ,-11358 ,-11340 ,-11339 ,-11324 ,-11303 ,-11097 ,-11077 ,-11067
                  ,-11055 ,-11052 ,-11045 ,-11041 ,-11038 ,-11024 ,-11020 ,-11019 ,-11018 ,-11014
                  ,-10838 ,-10832 ,-10815 ,-10800 ,-10790 ,-10780 ,-10764 ,-10587 ,-10544 ,-10533
                  ,-10519 ,-10331 ,-10329 ,-10328 ,-10322 ,-10315 ,-10309 ,-10307 ,-10296 ,-10281
                  ,-10274 ,-10270 ,-10262 ,-10260 ,-10256 ,-10254
              };
            var sA = new[] {
              "a","ai","an","ang","ao"

              ,"ba","bai","ban","bang","bao","bei","ben","beng","bi","bian","biao","bie","bin"
              ,"bing","bo","bu"

              ,"ca","cai","can","cang","cao","ce","ceng","cha","chai","chan","chang","chao","che"
              ,"chen","cheng","chi","chong","chou","chu","chuai","chuan","chuang","chui","chun"
              ,"chuo","ci","cong","cou","cu","cuan","cui","cun","cuo"

              ,"da","dai","dan","dang","dao","de","deng","di","dian","diao","die","ding","diu"
              ,"dong","dou","du","duan","dui","dun","duo"

              ,"e","en","er"

              ,"fa","fan","fang","fei","fen","feng","fo","fou","fu"

              ,"ga","gai","gan","gang","gao","ge","gei","gen","geng","gong","gou","gu","gua","guai"
              ,"guan","guang","gui","gun","guo"

              ,"ha","hai","han","hang","hao","he","hei","hen","heng","hong","hou","hu","hua","huai"
              ,"huan","huang","hui","hun","huo"

              ,"ji","jia","jian","jiang","jiao","jie","jin","jing","jiong","jiu","ju","juan","jue"
              ,"jun"

              ,"ka","kai","kan","kang","kao","ke","ken","keng","kong","kou","ku","kua","kuai","kuan"
              ,"kuang","kui","kun","kuo"

              ,"la","lai","lan","lang","lao","le","lei","leng","li","lia","lian","liang","liao","lie"
              ,"lin","ling","liu","long","lou","lu","lv","luan","lue","lun","luo"

              ,"ma","mai","man","mang","mao","me","mei","men","meng","mi","mian","miao","mie","min"
              ,"ming","miu","mo","mou","mu"

              ,"na","nai","nan","nang","nao","ne","nei","nen","neng","ni","nian","niang","niao","nie"
              ,"nin","ning","niu","nong","nu","nv","nuan","nue","nuo"

              ,"o","ou"

              ,"pa","pai","pan","pang","pao","pei","pen","peng","pi","pian","piao","pie","pin","ping"
              ,"po","pu"

              ,"qi","qia","qian","qiang","qiao","qie","qin","qing","qiong","qiu","qu","quan","que"
              ,"qun"

              ,"ran","rang","rao","re","ren","reng","ri","rong","rou","ru","ruan","rui","run","ruo"

              ,"sa","sai","san","sang","sao","se","sen","seng","sha","shai","shan","shang","shao","she"
              ,"shen","sheng","shi","shou","shu","shua","shuai","shuan","shuang","shui","shun","shuo","si"
              ,"song","sou","su","suan","sui","sun","suo"

              ,"ta","tai","tan","tang","tao","te","teng","ti","tian","tiao","tie","ting","tong","tou","tu"
              ,"tuan","tui","tun","tuo"

              ,"wa","wai","wan","wang","wei","wen","weng","wo","wu"

              ,"xi","xia","xian","xiang","xiao","xie","xin","xing","xiong","xiu","xu","xuan","xue","xun"

              ,"ya","yan","yang","yao","ye","yi","yin","ying","yo","yong","you","yu","yuan","yue","yun"

              ,"za","zai","zan","zang","zao","ze","zei","zen","zeng","zha","zhai","zhan","zhang","zhao"
              ,"zhe","zhen","zheng","zhi","zhong","zhou","zhu","zhua","zhuai","zhuan","zhuang","zhui"
              ,"zhun","zhuo","zi","zong","zou","zu","zuan","zui","zun","zuo"
          };
            var s = "";
            var c = x.ToCharArray();
            for (var j = 0; j < c.Length; j++)
            {
                var b = Encoding.Default.GetBytes(c[j].ToString());
                if (b[0] <= 160 && b[0] >= 0)
                {
                    s += c[j];
                }
                else
                {
                    for (var i = (iA.Length - 1); i >= 0; i--)
                    {
                        if (iA[i] > b[0] * 256 + b[1] - 65536) continue;
                        if (isAbbr)
                            s += sA[i].FirstOrDefault();
                        else
                            s += sA[i];
                        break;
                    }
                }
            }

            return s;
        }

        /// <summary>
        /// 获取字符串的首字符
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static char GetFirstChar(string x)
        {
            var charlist = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var spellcharList = (GetSpell(x) + string.Empty).ToUpper().ToCharArray();
            var returnchar = '0';//其它 
            if (spellcharList.Any())
                returnchar = spellcharList[0];
            //26字符中不包含时，表示其它
            if (!charlist.Contains(returnchar))
                returnchar = '0';//其它 
            return returnchar;
        }

        /// <summary>
        /// 汉字拼音缩写
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string GetSpellAbbr(string x)
        {
            return GetSpell(x, true).ToUpper();
        }

        #endregion

        #region 字符串和64位数据的转换

        /// <summary>
        /// 将字符串转为Base64字符串
        /// </summary>
        /// <param name="pureString">原始的字符串</param>
        /// <returns>返回编码后的Base64字符串</returns>
        public static string EncodeToBase64(string pureString)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(pureString));
        }

        /// <summary>
        /// 将Url解码后的Base64字符串还原为其原始值（UrlEncodeToBase64逆向过程）
        /// </summary>
        /// <param name="base64String">base64字符串</param>
        /// <returns></returns>
        public static string DecodeFromBase64(string base64String)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64String));
        }

        #endregion


        #region 是否为图片资源

        /// <summary>
        /// 是否为图片资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsImage(string path)
        {
            //bmp,jpg,tiff,gif,pcx,tga,exif,fpx,svg,psd,cdr,pcd,dxf,ufo,eps,ai,raw
            // *.jpg;*.gif;*.png;*.jpeg;
            if (string.IsNullOrEmpty(path)) return false;
            path = path.ToLower();
            return path.EndsWith("jpg")
                   || path.EndsWith("gif")
                   || path.EndsWith("png")
                   || path.EndsWith("bmp")
                   || path.EndsWith("jpeg")
                   || path.EndsWith("exif")
                   || path.EndsWith("tiff")
                   || path.EndsWith("raw")
                   || path.EndsWith("ai"); 
        }

        #endregion

        /// <summary>
        /// 判断是否是在微信浏览器里
        /// </summary>
        /// <returns></returns>
        public static bool IsWeixinBrowser(HttpRequestBase request)
        {
            var agent = request.ServerVariables["HTTP_USER_AGENT"];
            if (string.IsNullOrEmpty(agent) || agent.IndexOf("icroMessenger", StringComparison.OrdinalIgnoreCase) <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
