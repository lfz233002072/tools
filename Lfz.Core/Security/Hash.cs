/*======================================================================
 * 
 *        Copyright (C)  1996-2013  lfz 
 *        All rights reserved
 *
 *        Filename :Hash.cs
 *        DESCRIPTION : 
 *
 *        Created By 林芳崽(Datura) at  2013-05-18 10:56
 *        http://www.carvingsky.com/ 
 *
 *======================================================================*/

using System;
using System.Globalization;

namespace Lfz.Security
{
    /// <summary>
    /// Compute an (almost) unique hash value from various sources.
    /// This allows computing hash keys that are easily storable
    /// and comparable from heterogenous components.
    /// </summary>
    public class Hash
    {
        private long _hash;

        /// <summary>
        /// hash值
        /// </summary>
        public string Value
        {
            get
            {
                return _hash.ToString("x", CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// 重载函数，返回当前Hash对象的Hash值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// 添加Hash值构造字符串
        /// </summary>
        /// <param name="value"></param>
        public void AddString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            _hash += GetStringHashCode(value);
        }

        /// <summary>
        /// 添加使用固定区域性的大小写规则的字符串
        /// </summary>
        /// <param name="value"></param>
        public void AddStringInvariant(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            AddString(value.ToLowerInvariant());
        }

        /// <summary>
        /// 添加引用类型
        /// </summary>
        /// <param name="type"></param>
        public void AddTypeReference(Type type)
        {
            AddString(type.AssemblyQualifiedName);
            AddString(type.FullName);
        }

        /// <summary>
        /// 添加时间
        /// </summary>
        /// <param name="dateTime"></param>
        public void AddDateTime(DateTime dateTime)
        {
            _hash += dateTime.ToBinary();
        }

        /// <summary>
        /// We need a custom string hash code function, because .NET string.GetHashCode()
        /// function is not guaranteed to be constant across multiple executions.
        /// </summary>
        private static long GetStringHashCode(string s)
        {
            unchecked
            {
                long result = 352654597L;
                foreach (var ch in s)
                {
                    long h = ch.GetHashCode();
                    result = result + (h << 27) + h;
                }
                return result;
            }
        }
    }
}