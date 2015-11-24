using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB360Observer.Common
{
    public static class EnumHelper
    {
        public static string GetDescription(string ebEnum)
        {
            switch (ebEnum)
            {
                case "JD": return "京东";
                case "YHD": return "一号店";
                case "SUNING": return "苏宁";
            }
            return string.Empty;
        }
        public static string GetEnumDesc(int busness)
        {
            switch (busness)
            {
                case 1: return "京东";
                case 2: return "一号店";
                case 3: return "苏宁";
            }
            return string.Empty;
        }
    }
}
