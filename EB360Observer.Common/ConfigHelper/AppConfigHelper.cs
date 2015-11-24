using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB360Observer.Common
{
    public static class AppConfigHelper
    {
        public static readonly string DataSourceType = System.Configuration.ConfigurationManager.AppSettings["DataSourceType"];
        public static readonly string PriceAdjustmentRange = System.Configuration.ConfigurationManager.AppSettings["PriceAdjustmentRange"];
        public static readonly string MailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
        public static readonly string MailFromAccount = System.Configuration.ConfigurationManager.AppSettings["MailFromAccount"];
        public static readonly string MailFromPwd = System.Configuration.ConfigurationManager.AppSettings["MailFromPwd"];
        public static readonly string MailSmtpServer = System.Configuration.ConfigurationManager.AppSettings["MailSmtpServer"];
        public static readonly string MailTo = System.Configuration.ConfigurationManager.AppSettings["MailTo"];
    }
}
