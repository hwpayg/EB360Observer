using EB360Observer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB360Observer.DAL
{
    public class MsSqlEbItemDal
    {
        public long Insert(EBItem item)
        {
            using (var db = new EbItemDbContext())
            {
                db.EBItems.Add(item);
                db.SaveChanges();
            }
            return item.Id;
        }

        public EBItem GetLatestItemBySkuid(string skuid, string electricBusiness)
        {
            using (var db = new EbItemDbContext())
            {
                var items = db.EBItems.Where(p => p.Skuid == skuid && p.ElectricBusiness == electricBusiness).OrderByDescending(p => p.CreateTime).Take(1).ToList();
                if (items != null && items.Count > 0)
                {
                    return items.First();
                }
            }
            return null; ;
        }

        public IEnumerable<EBItem> GetList(DateTime? beginDate, DateTime? endDate, string name, string electricBusiness)
        {
            using (var db = new EbItemDbContext())
            {
                var query = db.EBItems.Where(p => p.Id > 0);
                if (beginDate.HasValue)
                {
                    query = query.Where(p => p.CreateTime >= beginDate.Value);
                }
                if (endDate.HasValue)
                {
                    endDate = endDate.Value.AddDays(1);
                    query = query.Where(p => p.CreateTime < endDate.Value);
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(p => p.Name.Contains(name));
                }
                if (!string.IsNullOrWhiteSpace(electricBusiness))
                {
                    query = query.Where(p => p.ElectricBusiness == electricBusiness);
                }
                return query.ToList();
            }
        }

        public IEnumerable<EBItem> GetPagerList(DateTime? beginDate, DateTime? endDate, string name, string electricBusiness, int pageSize, int pageIndex, out long totalCount)
        {
            using (var db = new EbItemDbContext())
            {
                var query = db.EBItems.Where(p => p.Id > 0);
                if (beginDate.HasValue)
                {
                    query = query.Where(p => p.CreateTime >= beginDate.Value);
                }
                if (endDate.HasValue)
                {
                    endDate = endDate.Value.AddDays(1);
                    query = query.Where(p => p.CreateTime < endDate.Value);
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(p => p.Name.Contains(name));
                }
                if (!string.IsNullOrWhiteSpace(electricBusiness))
                {
                    query = query.Where(p => p.ElectricBusiness == electricBusiness);
                }
                totalCount = query.LongCount();
                return query.OrderByDescending(p => p.CreateTime).Skip(pageSize * pageIndex).Take(pageSize).ToList();
            }
        }
    }
}
