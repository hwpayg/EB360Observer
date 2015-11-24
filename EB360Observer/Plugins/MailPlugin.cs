using EB360Observer.Common.Mail;
using EB360Observer.Entity;
using EB360Observer.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EB360Observer.Common;

namespace EB360Observer.Plugins
{
    public class MailPlugin
    {
        private static MailPlugin instance = null;
        private static readonly object lockHelper = new object();
        private MailPlugin()
        {

        }

        public static MailPlugin GetMailPluginInstance()
        {
            if (instance == null)
            {
                lock (lockHelper)
                {
                    if (instance == null)
                    {
                        instance = new MailPlugin();
                    }
                }
            }
            return instance;
        }
        public void SendMail(EBItem ebItem)
        {
            if (ebItem != null)
            {
                if (ebItem.Price > 0)//新数据价格大于0
                {
                    var dataAccess = DataAccessFactory.GetDataAccess((DataSourceEnum)Enum.Parse(typeof(DataSourceEnum), AppConfigHelper.DataSourceType));
                    var item = dataAccess.GetLastestItemBySkuid(ebItem.Skuid, ebItem.ElectricBusiness);
                    if (item != null && item.Price > 0)//原数据价格大于0 
                    {
                        var range = (Math.Abs(item.Price - ebItem.Price) / item.Price) * 100;
                        if (range > Convert.ToInt32(AppConfigHelper.PriceAdjustmentRange))
                        {
                            var type = item.Price - ebItem.Price > 0 ? "降价" : "加价";
                            var title = string.Format("[{0}]{1}{2}{3}%", item.ElectricBusiness, item.Name, type, range.ToString("f2"));
                            var mailContent = string.Format("产品ID:{0}<br/>产品名称:{1}<br/>商家:{2}<br/>商品链接:<a href='{3}' target='_blank'>{4}</a><br/><strong style='color:red'>{5}幅度：{6}%</strong><br/><strong style='color:red'>价格：{7}RMB</strong>",
                                item.Skuid, item.Name, item.ElectricBusiness, item.ProductUrl, item.Name, type, range.ToString("f2"), ebItem.Price.ToString("f2"));
                            NetSendMail.MailSend(AppConfigHelper.MailFrom, AppConfigHelper.MailFromAccount, AppConfigHelper.MailFromPwd, AppConfigHelper.MailSmtpServer, mailTo: new List<string> { AppConfigHelper.MailTo }, mailCC: new List<string>(), mailBCC: new List<string>(), mailTitle: title, mailContent: mailContent, mailAttachments: new List<string>(), encoding: Encoding.UTF8, isBodyHtml: true);
                        }
                    }
                }
            }
        }
    }
}
