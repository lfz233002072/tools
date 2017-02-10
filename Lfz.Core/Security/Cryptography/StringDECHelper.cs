using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lfz.Security.Cryptography
{
    /// <summary>
    /// 加解密
    /// </summary>
    public class StringDECHelper
    {
        #region 密钥

        /// <summary> 
        /// KEY 固定
        /// </summary> 
        public const string KEY_64 = "pm%@3!*Y";

        #endregion

        #region 加密

        /// <summary>
        /// 字符串加密（自带密钥）
        /// </summary>
        /// <param name="pToEncrypt">待加密字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string pToEncrypt)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //把字符串放到byte数组中  
            //原来使用的UTF8编码，我改成Unicode编码了，不行  
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            //byte[]  inputByteArray=Encoding.Unicode.GetBytes(pToEncrypt);  

            //建立加密对象的密钥和偏移量  
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法  
            //使得输入密码必须输入英文文本  
            des.Key = ASCIIEncoding.ASCII.GetBytes(KEY_64);
            des.IV = ASCIIEncoding.ASCII.GetBytes(KEY_64);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            //Write  the  byte  array  into  the  crypto  stream  
            //(It  will  end  up  in  the  memory  stream)  
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //Get  the  data  back  from  the  memory  stream,  and  into  a  string  
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                //Format  as  hex  
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="pToEncrypt">待加密字符串</param>
        /// <param name="key">密钥（8位字符串）</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string pToEncrypt, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //把字符串放到byte数组中  
            //原来使用的UTF8编码，我改成Unicode编码了，不行  
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            //byte[]  inputByteArray=Encoding.Unicode.GetBytes(pToEncrypt);  

            //建立加密对象的密钥和偏移量  
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法  
            //使得输入密码必须输入英文文本  
            des.Key = ASCIIEncoding.ASCII.GetBytes(key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            //Write  the  byte  array  into  the  crypto  stream  
            //(It  will  end  up  in  the  memory  stream)  
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //Get  the  data  back  from  the  memory  stream,  and  into  a  string  
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                //Format  as  hex  
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        #endregion

        #region 解密

        /// <summary>
        /// 字符串解密（自带密钥）
        /// </summary>
        /// <param name="pToDecrypt">字符串</param>
        /// <returns>解密后的字符串（解密失败返回“”）</returns>
        public static string Decrypt(string pToDecrypt)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                //Put  the  input  string  into  the  byte  array  
                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                //建立加密对象的密钥和偏移量，此值重要，不能修改  
                des.Key = ASCIIEncoding.ASCII.GetBytes(KEY_64);
                des.IV = ASCIIEncoding.ASCII.GetBytes(KEY_64);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                //Flush  the  data  through  the  crypto  stream  into  the  memory  stream  
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                //Get  the  decrypted  data  back  from  the  memory  stream  
                //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象  
                StringBuilder ret = new StringBuilder();

                return System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="pToDecrypt">字符串</param>
        /// <param name="key">密钥（8位字符串）</param>
        /// <returns>解密后的字符串（解密失败返回“”）</returns>
        public static string Decrypt(string pToDecrypt, string key)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                //Put  the  input  string  into  the  byte  array  
                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                //建立加密对象的密钥和偏移量，此值重要，不能修改  
                des.Key = ASCIIEncoding.ASCII.GetBytes(key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(key);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                //Flush  the  data  through  the  crypto  stream  into  the  memory  stream  
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                //Get  the  decrypted  data  back  from  the  memory  stream  
                //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象  
                StringBuilder ret = new StringBuilder();

                return System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region MD5加密

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string Md5(string str)
        {
            var b = Encoding.UTF8.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            var ret = "";
            for (var i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }

        #endregion

        #region 哈希字符串加密

        /// <summary>
        /// 哈希字符串转换格式
        /// </summary>
        /// <param name="toHash">要转换的hash字符串</param>
        /// <returns>转换后的字符串</returns>
        public static string Hash(string toHash)
        {
            MD5CryptoServiceProvider crypto = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF7.GetBytes(toHash);
            bytes = crypto.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte num in bytes)
            {
                sb.AppendFormat("{0:x2}", num);
            }
            return sb.ToString();
        }

        #endregion
    }
}
