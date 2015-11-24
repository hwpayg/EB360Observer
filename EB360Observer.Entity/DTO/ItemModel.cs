using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB360Observer.Entity
{
    public class ItemModel
    {
        public string Skuid { get; set; }
        public string Name { get; set; }
        public string PriceQueryString { get; set; }
        public string PromotionQueryString { get; set; }

        public string ElectricBusiness { get; set; }
        public string PriceCrawlUrl { get; set; }
        public string PromotionCrawlUrl { get; set; }
    }
}
