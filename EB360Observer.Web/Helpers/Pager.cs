using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace EB360Observer.Web.Helpers
{
    public class Pager
    {
        private const string URLSPLIT = "_";

        public Pager()
        {
            GetPageNumber();
        }
        public Pager(int pgSize, int rdCount)
        {
            GetJumper(pgSize, rdCount);
        }
        public Pager(int pgNumber, int pgSize, int rdCount)
        {
            GetJumper(pgNumber, pgSize, rdCount);
        }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int RecordCount { get; set; }
        public int PageCount = -1;
        public string UidPrefix = string.Empty;
        public string GetJumper()
        {
            if (PageCount == -1 || RecordCount == -1) { return "请您先设置好相关数据。"; }
            if (RecordCount <= 0)
            {
                return string.Empty;
            }
            if (PageCount <= 1)
            {
                ComputePageNumber();
                if (PageCount <= 1)
                {
                    return string.Empty;
                }

            }
            string url = HttpContext.Current.Request.FilePath;
            StringBuilder printer = new StringBuilder("<div class=\"Pager_Normal\">");
            int iFirst = PageIndex - 5;
            int iLast = PageIndex + 5;
            if (iFirst < 1) { iFirst = 1; }
            if (PageIndex < 7) { iLast = 10; }
            if (iLast > PageCount) { iLast = PageCount; }

            url += "?";
            string _flag = "";
            foreach (string _str in HttpContext.Current.Request.QueryString)
            {
                if (_str == "pn") { continue; }
                url += _flag + _str + "=" + HttpContext.Current.Request.QueryString[_str];
                if (_flag == "") { _flag = "&"; }
            }

            url += _flag;

            if (PageIndex > 1)
            {
                printer.Append("<a href=\"" + url + "pn=1\">&lt;&lt;</a><a href=\"" + url + "pn=" + (PageIndex - 1).ToString() + "\">&lt;</a>");
            }

            for (int i = iFirst; i <= iLast; i++)
            {
                if (i == PageIndex) { printer.Append("<span class=\"current\">" + PageIndex.ToString() + "</span>"); }
                else printer.Append("<a href=\"" + url + "pn=" + i.ToString() + "\">" + i.ToString() + "</a>");
            }

            if (PageIndex < PageCount)
            {
                printer.Append("<a href=\"" + url + "pn=" + (PageIndex + 1).ToString() + "\">&gt;</a><a href=\"" + url + "pn=" + PageCount.ToString() + "\">&gt;&gt;</a>");
            }



            printer.Append("</div>");
            return printer.ToString();
        }

        /// <summary>
        /// 根据ajax参数方法等输出pager代码
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <param name="ajaxProviderFormat"></param>
        /// <returns></returns>
        public string GetJumperForAjax(int pageIndex, int pageSize, int recordCount, string ajaxProviderFormat, string className = "pagination pagination-right", string code = "")
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            RecordCount = recordCount;
            ComputePageNumber();

            if (PageCount == -1 || RecordCount == -1) { return "请您先设置好相关数据。"; }
            if (RecordCount <= 0)
            {
                return string.Empty;
            }

            StringBuilder printer = new StringBuilder();
            int iFirst = PageIndex - 5;
            int iLast = PageIndex + 5;
            if (iFirst < 1) { iFirst = 1; }
            if (PageIndex < 7) { iLast = 10; }
            if (iLast > PageCount) { iLast = PageCount; }
            printer.AppendFormat("<ul class=\"{0}\">", className);

            bool isFirst = PageIndex <= 1;
            bool isLast = PageIndex >= PageCount;
            printer.AppendFormat("<li style=\"float:left;margin-right:20px;\">共{0}条数据</li>", RecordCount);
            printer.AppendFormat("<li class=\"{1}\"><a {0}>&laquo;</a></li>", isFirst ? "disabled=\"disabled\" " : string.Format("title=\"转到上一页\" href=\"javascript:void(0);\" onclick=\"{0}\"", string.Format(ajaxProviderFormat, PageIndex - 1)), isFirst ? "disabled" : "");
            for (int i = iFirst; i <= iLast; i++)
            {
                if (i == PageIndex)
                {
                    printer.AppendFormat("<li class=\"active\"><a title=\"第{0}页\">{0}</span></li>", i);
                }
                else
                {
                    printer.AppendFormat("<li><a title=\"转到第{0}页\" href=\"javascript:void(0);\" onclick=\"{1}\">{0}</a></li>", i, string.Format(ajaxProviderFormat, i));
                }
            }
            printer.AppendFormat("<li class=\"{1}\"><a {0}>&raquo;</a><li>", isLast ? "disabled=\"disabled\" " : string.Format("title=\"转到下一页\" href=\"javascript:void(0);\" onclick=\"{0}\"", string.Format(ajaxProviderFormat, PageIndex + 1)), isLast ? "disabled" : "");
            printer.AppendFormat("<li style=\"margin-left:20px;\">到<input class=\"c_page_num\" type=\"text\" value=\"{1}\" size=\"3\" id=\"txtGoPage" + code + "\" />页<input class=\"c_page_submit\" type=\"button\" value=\"确定\" onclick=\"{2}\"/></li>", RecordCount, pageIndex, string.Format(ajaxProviderFormat, "0"));
            printer.AppendFormat("</ul>");

            return printer.ToString();
        }

        public string GetJumper(int pgNumber, int pgSize, int rdCount)
        {
            PageIndex = pgNumber;
            PageSize = pgSize;
            RecordCount = rdCount;
            return GetJumper();
        }

        public string GetJumper(int pgSize, int rdCount)
        {
            GetPageNumber();
            PageSize = pgSize;
            RecordCount = rdCount;
            return GetJumper();
        }
        private void GetPageNumber()
        {
            int pn = 0;
            int.TryParse(HttpContext.Current.Request.QueryString["pn"], out pn);
            if (pn > 0) { PageIndex = pn; }
            else PageIndex = 1;
        }

        private void ComputePageNumber()
        {
            if (RecordCount <= 0 || PageSize <= 0)
            {
                PageCount = 0;
                return;
            }
            PageCount = RecordCount / PageSize;
            if (PageCount * PageSize < RecordCount) PageCount++;
            if (PageIndex < 1) { PageIndex = 1; }
            else if (PageIndex > PageCount) { PageIndex = PageCount; }
        }
    }
}