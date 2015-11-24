using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EB360Observer
{
    class Program
    {
        private static readonly ILog Loger = LogManager.GetLogger("Program");
        static void Main(string[] args)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;//http请求最大连接数
            log4net.Config.XmlConfigurator.Configure();
            Loger.Info("EB360Observer启动...");
            new EBCrawl().Run();
            Loger.Info("EB360Observer停止...");           
        }
    }
    class TestTime
    {
        public DateTime CreateTime { get; set; }
    }
}
