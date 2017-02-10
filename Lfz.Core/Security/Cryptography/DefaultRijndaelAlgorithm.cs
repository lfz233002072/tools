// /*======================================================================
//  *
//  *        Copyright (C)  1996-2012  lfz    
//  *        All rights reserved
//  *
//  *        Filename :Symmetric.cs
//  *        DESCRIPTION :
//  *
//  *        Created By 林芳崽 at 2013-07-31 10:53
//  *        https://git.oschina.net/lfz/tools
//  *
//  *======================================================================*/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lfz.Security.Cryptography
{
    /// <summary>
    /// 对称算法加密(Rijndael)
    /// </summary>
    public class DefaultRijndaelAlgorithm
    {
        private readonly SymmetricAlgorithm _mobjCryptoService;
        private readonly string _key;

        /// <summary>    
        /// 对称加密类的构造函数    
        /// </summary>    
        public DefaultRijndaelAlgorithm()
        {
            _mobjCryptoService = new RijndaelManaged();
            _key = "Guz(%&hj7x89H$yuBI0456FtmaT5&fvHUFCy76*h%(HilJ$lhj!y6&(*jkP87jH7";
        }
        /// <summary>    
        /// 获得密钥    
        /// </summary>    
        /// <returns>密钥</returns>    
        private byte[] GetLegalKey()
        {
            string sTemp = _key;
            _mobjCryptoService.GenerateKey();
            byte[] bytTemp = _mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /// <summary>    
        /// 获得初始向量IV    
        /// </summary>    
        /// <returns>初试向量IV</returns>    
        private byte[] GetLegalIV()
        {
            string sTemp = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";
            _mobjCryptoService.GenerateIV();
            byte[] bytTemp = _mobjCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /// <summary>    
        /// 加密方法    
        /// </summary>    
        /// <param name="Source">待加密的串</param>    
        /// <returns>经过加密的串</returns>    
        public string Encrypto(string Source)
        {
            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
            MemoryStream ms = new MemoryStream();
            _mobjCryptoService.Key = GetLegalKey();
            _mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = _mobjCryptoService.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();
            ms.Close();
            byte[] bytOut = ms.ToArray();
            return Convert.ToBase64String(bytOut);
        }
        /// <summary>    
        /// 解密方法    
        /// </summary>    
        /// <param name="Source">待解密的串</param>    
        /// <returns>经过解密的串</returns>    
        public string Decrypto(string Source)
        {
            byte[] bytIn = Convert.FromBase64String(Source);
            MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
            _mobjCryptoService.Key = GetLegalKey();
            _mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = _mobjCryptoService.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}