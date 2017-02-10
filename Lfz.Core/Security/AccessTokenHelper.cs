// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :AccessTokenHelper.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2015-05-13 9:11
// *        https://git.oschina.net/lfz/tools
// *
// *======================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using Lfz.Security.Cryptography;
using Lfz.Utitlies;

namespace Lfz.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessTokenHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private const string Key = "AccessTokenHelper";

        /// <summary>
        /// 生产token
        /// </summary>
        /// <param name="id"></param>
        /// <param name="openUserId"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Generate(Guid id, string openUserId, IEnumerable<KeyValuePair<string, string>> args = null)
        {
            string value = string.Format("{0}|{1}|{2}", id.ToString("n").ToLower(), openUserId, DateTime.Now.Ticks);
            if (args.Any())
                foreach (var item in args)
                {
                    value += string.Format("|{0}={1}", item.Key, item.Value);
                }
            return DESHelper.EncryptDES(value, Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param> 
        /// <param name="expired">默认有效事件2个小时</param>
        /// <returns></returns>
        public static AccessTokenResult Match(string accessToken, int expired = 7200)
        {
            AccessTokenResult result = new AccessTokenResult() { IsMatch = false, Args = new List<KeyValuePair<string, string>>() };
            var value = DESHelper.DecryptDES(accessToken, Key) ?? string.Empty;
            var list = value.Split('|');
            if (list.Length < 3) return result;
            result.CompanyId = TypeParse.StrToGuid(list[0]);
            long ticks = TypeParse.StrToInt64(list[1]);
            result.OpenUserId = list[2];
            if (expired < 600) expired = 600;
            var expiredTime = TimeSpan.FromTicks(ticks).Add(TimeSpan.FromSeconds(expired));
            //仍然未过期
            if (expiredTime > TimeSpan.FromTicks(DateTime.Now.Ticks)) result.IsMatch = true;
            if (result.IsMatch)
            {
                for (int i = 2; i < list.Length; i++)
                {
                    var tempList = (list[i] ?? string.Empty).Split('=');
                    if (tempList.Length == 2 && !string.IsNullOrEmpty(tempList[0]))
                    {
                        result.Args.Add(new KeyValuePair<string, string>(tempList[0], tempList[1]));
                    }
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 访问token
    /// </summary>
    public class AccessToken
    {
        /// <summary>
        /// 商家ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OpenUserId { get; set; }

        /// <summary>
        /// 其它参数
        /// </summary>
        public List<KeyValuePair<string, string>> Args { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class AccessTokenResult : AccessToken
    {
        /// <summary>
        /// 是否匹配成功
        /// </summary>
        public bool IsMatch { get; set; }
    }
}