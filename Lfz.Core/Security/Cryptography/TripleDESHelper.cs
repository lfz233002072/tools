// /*======================================================================
//  *
//  *        Copyright (C)  1996-2012  lfz    
//  *        All rights reserved
//  *
//  *        Filename :Class1.cs
//  *        DESCRIPTION :
//  *
//  *        Created By 林芳崽 at 2013-07-31 10:52
//  *        https://git.oschina.net/lfz/tools
//  *
//  *======================================================================*/

using System;
using System.Security.Cryptography;
using System.Text;
using Lfz.Logging;

namespace Lfz.Security.Cryptography
{
    /// <summary>
    /// TripleDES加密辅助类
    /// </summary>
    public class TripleDESHelper
    {
        private static readonly ILogger Logger;
        private static string _key;

        static TripleDESHelper()
        {
            Logger = LoggerFactory.GetLog();
            _key = "linfangzai";
        }


        /// <summary>
        /// 设置键值
        /// </summary>
        public static string Key
        {
            set
            {
                _key = value;
            }
        }

        /// <summary>
        /// Encrypt the given string using the default key.
        /// </summary>
        /// <param name="strToEncrypt">The string to be encrypted.</param>
        /// <returns>The encrypted string.</returns>
        public static string Encrypt(string strToEncrypt)
        {
            return Encrypt(strToEncrypt, _key);

        }

        /// <summary>
        /// Decrypt the given string using the default key.
        /// </summary>
        /// <param name="strEncrypted">The string to be decrypted.</param>
        /// <returns>The decrypted string.</returns>
        public static string Decrypt(string strEncrypted)
        {
            return Decrypt(strEncrypted, _key);
        }

        /// <summary>
        /// Encrypt the given string using the specified key.
        /// </summary>
        /// <param name="strToEncrypt">The string to be encrypted.</param>
        /// <param name="strKey">The encryption key.</param>
        /// <returns>The encrypted string.加密失败返回原字符串</returns>
        public static string Encrypt(string strToEncrypt, string strKey)
        {
            //字符串为空或者加密密钥为空，那么返回原字符串
            if (string.IsNullOrEmpty(strToEncrypt) || string.IsNullOrEmpty(strKey)) return strToEncrypt;
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

                byte[] byteHash, byteBuff;
                string strTempKey = strKey;

                byteHash = objHashMD5.ComputeHash(Encoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

                byteBuff = Encoding.UTF8.GetBytes(strToEncrypt);
                return Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return strToEncrypt;
            }
        }

        /// <summary>
        /// Decrypt the given string using the specified key.
        /// </summary>
        /// <param name="strEncrypted">The string to be decrypted.</param>
        /// <param name="strKey">The decryption key.</param>
        /// <returns>The decrypted string.解密失败返回原字符串</returns>
        public static string Decrypt(string strEncrypted, string strKey)
        {
            //字符串为空或者解密密钥为空，那么返回原字符串
            if (string.IsNullOrEmpty(strEncrypted) || string.IsNullOrEmpty(strKey)) return strEncrypted;
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

                byte[] byteHash, byteBuff;
                string strTempKey = strKey;

                byteHash = objHashMD5.ComputeHash(Encoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

                byteBuff = Convert.FromBase64String(strEncrypted);
                string strDecrypted = Encoding.UTF8.GetString(objDESCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                objDESCrypto = null;

                return strDecrypted;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return strEncrypted;
            }
        }
    }
}