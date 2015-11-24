using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EB360Observer.Common
{
    public class RegexHelper
    {
        /// <summary>
        /// 正则表达式匹配
        /// </summary>
        /// <param name="originalStr">原始串</param>
        /// <param name="pattern">匹配模式</param>
        /// <param name="splitChar">符合条件的结果分隔符</param>
        /// <returns></returns>
        public static string GetRegexMatch(string originalStr, string pattern, char splitChar = ';')
        {
            var reg = new Regex(pattern);
            var match = reg.Match(originalStr);
            var result = string.Empty;
            while (match.Success)
            {
                result += match.Groups[1].Value + splitChar;
                match = match.NextMatch();
            }
            return result.TrimEnd(splitChar);
        }
    }
}
