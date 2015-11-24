using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using log4net;
using System.ComponentModel;
using System.Reflection;
using EB360Observer.Entity;
using EB360Observer.Common;
using EB360Observer.Crawler;
using EB360Observer.DataService;
using log4net.Repository.Hierarchy;

namespace EB360Observer
{
    public class EBCrawl
    {
        private static readonly ILog Loger = LogManager.GetLogger("EBCrawl");
        public List<ICrawler> CrawlerList { get; set; }
        public EBCrawl()
        {
            CrawlerList = new List<ICrawler>
            {
                new JDCrawler(),
                new YHDCrawler(),
                new SUNINGCrawler()
            };
        }
        public void Run()
        {
            var dataAccess = DataAccessFactory.GetDataAccess((DataSourceEnum)Enum.Parse(typeof(DataSourceEnum), AppConfigHelper.DataSourceType));
            var items = XmlHelper.GetItems("Items.xml");
            foreach (var item in items)
            {
                Parallel.ForEach(CrawlerList, crawler =>
                {
                    try
                    {
                        var ebItem = crawler.Crawl(item);
                        if (ebItem != null)
                        {
                            if (System.Configuration.ConfigurationManager.AppSettings["MailNotice"] == "1")//邮件通知
                            {
                                try
                                {
                                    EB360Observer.Plugins.MailPlugin.GetMailPluginInstance().SendMail(ebItem);
                                }
                                catch (Exception ex)
                                {
                                    Loger.Fatal("发送邮件失败:", ex);
                                }
                            }
                            dataAccess.Insert(ebItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        Loger.Fatal("webclient请求失败", ex);
                    }
                });
            }
        }

    }
}
