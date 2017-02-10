using System;
using System.Text.RegularExpressions;

namespace Lfz.Utitlies
{
    public partial class Utils
    {
        /// <summary>
        /// 使用正则查找，并安装replaceAction委托执行替换。
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        /// <param name="inputText">输入文本</param>
        /// <param name="replaceAction">替换行为委托。包括一个参数：匹配结果对象
        /// </param>
        public static void ReplaceByRegexAction(string pattern, string inputText, Action<Match> replaceAction)
        {
            MatchCollection mcList = new Regex(pattern).Matches(inputText);
            foreach (Match ma in mcList) if (replaceAction != null) replaceAction.Invoke(ma);
        }
    }
}