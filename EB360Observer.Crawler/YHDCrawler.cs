﻿using EB360Observer.Common;
using EB360Observer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EB360Observer.Crawler
{
    public class YHDCrawler : EbItemCreator, ICrawler
    {
        public Entity.EBItem Crawl(Entity.ItemModel item)
        {            
            if (item.ElectricBusiness == ElectricBusinessEnum.YHD.ToString())
            {
                var ebItem = CreateEBItem(item);
                var priceResponse = new HttpClientDownloader().Download(string.Format(item.PriceCrawlUrl, item.Skuid) + item.PriceQueryString.Replace('`', '&'), HttpConstant.Method.Get.ToString());
                ebItem.PriceDesc = priceResponse;
                var price = RegexHelper.GetRegexMatch(priceResponse, "currentPrice\":(\\d+),");//这里需要确保只有一个价格匹配
                if (!string.IsNullOrWhiteSpace(price))
                {
                    ebItem.Price = Convert.ToDecimal(price);
                }

                var merchantId = RegexHelper.GetRegexMatch(priceResponse, "merchantId\":(\\d+),");//这里需要确保只有一个匹配
                var productId = RegexHelper.GetRegexMatch(priceResponse, "productId\":(\\d+),");//这里需要确保只有一个匹配
                var promotionResponse = new HttpClientDownloader().Download(string.Format(item.PromotionCrawlUrl, item.Skuid,
                    string.Format(item.PromotionQueryString.Replace('`', '&'), merchantId, productId)),
                    HttpConstant.Method.Get.ToString());
                ebItem.PromotionDesc = promotionResponse;

                ebItem.ProductUrl = string.Format("http://item.yhd.com/item/{0}", item.Skuid);
                return ebItem;
            }
            return null;
        }
    }
}
