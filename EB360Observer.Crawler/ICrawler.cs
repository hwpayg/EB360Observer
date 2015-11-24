using EB360Observer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB360Observer.Crawler
{
    public interface ICrawler
    {
        EBItem Crawl(ItemModel item);
    }
}
