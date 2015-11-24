using EB360Observer.Common;
using EB360Observer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EB360Observer.Crawler
{
    public class JDCrawler : EbItemCreator, ICrawler
    {
        public Entity.EBItem Crawl(Entity.ItemModel item)
        {            
            if (item.ElectricBusiness == ElectricBusinessEnum.JD.ToString())
            {
                var ebItem = CreateEBItem(item);
                var priceResponse = new HttpClientDownloader().Download(string.Format(item.PriceCrawlUrl, item.Skuid) + item.PriceQueryString.Replace('`', '&'), HttpConstant.Method.Get.ToString());
                ebItem.PriceDesc = priceResponse;
                var price = RegexHelper.GetRegexMatch(priceResponse, "p\":\"(.+?)\",");//这里需要确保只有一个价格匹配
                if (!string.IsNullOrWhiteSpace(price))
                {
                    ebItem.Price = Convert.ToDecimal(price);
                }

                var promotionResponse = new HttpClientDownloader().Download(string.Format(item.PromotionCrawlUrl, item.Skuid, item.PromotionQueryString.Replace('`', '&')), HttpConstant.Method.Get.ToString());
                ebItem.PromotionDesc = promotionResponse;
                var gift = RegexHelper.GetRegexMatch(promotionResponse, "nm\":\"(.+?)\",\"sid\"");
                ebItem.Gift = gift;

                ebItem.ProductUrl = string.Format("http://item.jd.com/{0}.html", item.Skuid);
                return ebItem;
            }
            return null;
        }
    }
}
