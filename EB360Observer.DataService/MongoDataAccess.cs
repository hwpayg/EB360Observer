using EB360Observer.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB360Observer.DataService
{
    public class MongoDataAccess : IDataAccess
    {
        public long Insert(Entity.EBItem item)
        {
            return new EBItemDAL().Insert(item);
        }

        public Entity.EBItem GetLastestItemBySkuid(string skuid, string electricBusiness)
        {
            return new EBItemDAL().GetLatestItemBySkuid(skuid, electricBusiness);
        }

        public List<Entity.EBItem> GetList(DateTime? beginDate, DateTime? endDate, string name, string electricBusiness)
        {
            var list = new EBItemDAL().GetList(beginDate, endDate, name, electricBusiness);
            if (list != null && list.Count() > 0)
            {
                return list.ToList();
            }
            return null;
        }

        public List<Entity.EBItem> GetPagerList(DateTime? beginDate, DateTime? endDate, string name, string electricBusiness, int pageSize, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            var cusor = new EBItemDAL().GetPagerList(beginDate, endDate, name, electricBusiness, pageSize, pageIndex);
            if (cusor != null)
            {
                totalCount = cusor.Count();
                return cusor.ToList();
            }           
            return null;
        }
    }
}
