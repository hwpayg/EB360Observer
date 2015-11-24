using EB360Observer.Common;
using EB360Observer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB360Observer.Crawler
{
    public class EbItemCreator
    {
        public EBItem CreateEBItem(Entity.ItemModel item)
        {
            return new EBItem
            {
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                Name = item.Name,
                Skuid = item.Skuid,
                ElectricBusiness = EnumHelper.GetDescription(item.ElectricBusiness)
            };
        }
    }
}
