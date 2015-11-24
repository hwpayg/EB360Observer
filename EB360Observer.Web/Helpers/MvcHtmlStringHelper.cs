using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace EB360Observer.Web.Helpers
{
    public static class MvcHtmlStringHelper
    {
        /// <summary>
        /// 提供对enum枚举作为数据源的DropDownList扩展
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="helper"></param>
        /// <param name="enumSource">枚举值（如果需要设置默认选中值，则把这个值设为默认的值，否则随便传入任何该枚举值）</param>
        /// <param name="name"></param>
        /// <param name="needAll">是否需要全部选项</param>
        /// <param name="setDefault">是否需要设置默认值，设置了，则enumSource的值会成为默认选中值</param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownList<T>(this HtmlHelper helper, T enumSource, string name, bool needAll = true, bool setDefault = false, object htmlAttributes = null)
        {
            var listItems = GenerateListItems<T>(enumSource, needAll, setDefault);
            return helper.DropDownList(name, listItems, htmlAttributes);
        }

        private static IEnumerable<SelectListItem> GenerateListItems<T>(T enumSource, bool needAll = true, bool setDefault = false)
        {
            var items = new List<SelectListItem>();

            if (needAll)
            {
                items.Add(new SelectListItem
                {
                    Text = "全部",
                    Value = "0",
                });
            }
            foreach (var enumEntity in (T[])Enum.GetValues(typeof(T)))
            {
                var name = Enum.GetName(typeof(T), enumEntity);
                var field = typeof(T).GetField(name);
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                var item = new SelectListItem
                {
                    Text = attribute != null ? attribute.Description : enumEntity.ToString(),
                    Value = Convert.ToInt32(enumEntity).ToString(),
                    Selected = setDefault && enumEntity.ToString() == enumSource.ToString()
                };
                items.Add(item);
            }

            return items;
        }
    }
}