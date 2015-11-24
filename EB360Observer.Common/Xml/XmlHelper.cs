using EB360Observer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EB360Observer.Common
{
    public static class XmlHelper
    {
        public static List<ItemModel> GetItems(string fileName)
        {
            var doc = new XmlDocument();

            doc.Load(fileName);
            var xn = doc.SelectSingleNode("Items");
            // 得到根节点的所有子节点
            var xnl = xn.ChildNodes;
            var items = new List<ItemModel>();
            foreach (XmlNode xn1 in xnl)
            {
                XmlElement xe = (XmlElement)xn1;

                foreach (var xmNode in xn1.ChildNodes)
                {
                    ItemModel item = new ItemModel();
                    item.ElectricBusiness = xe.Attributes["ElectricBusiness"].Value;
                    item.PriceCrawlUrl = xe.Attributes["PriceCrawlUrl"].Value;
                    item.PromotionCrawlUrl = xe.Attributes["PromotionCrawlUrl"].Value;
                    var xee = (XmlElement)xmNode;// 将节点转换为元素，便于得到节点的属性值
                    XmlNodeList xnl0 = xee.ChildNodes;
                    item.Skuid = xnl0.Item(0).InnerText;
                    item.Name = xnl0.Item(1).InnerText;
                    item.PriceQueryString = xnl0.Item(2).InnerText;
                    item.PromotionQueryString = xnl0.Item(3).InnerText;
                    items.Add(item);
                }
            }
            return items.OrderBy(p => p.ElectricBusiness).ToList();
        }
    }
}
