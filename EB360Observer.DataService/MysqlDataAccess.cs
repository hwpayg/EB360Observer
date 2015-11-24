using EB360Observer.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB360Observer.DataService
{
    public class MySqlDataAccess:IDataAccess
    {
        public long Insert(Entity.EBItem item)
        {
            return new MySqlEbItemDal().Insert(item);
        }

        public Entity.EBItem GetLastestItemBySkuid(string skuid, string electricBusiness)
        {
            return new MySqlEbItemDal().GetLatestItemBySkuid(skuid, electricBusiness);
        }

        public List<Entity.EBItem> GetList(DateTime? beginDate, DateTime? endDate, string name, string electricBusiness)
        {
            var list = new MySqlEbItemDal().GetList(beginDate, endDate, name, electricBusiness);
            if (list != null)
            {
                return list.ToList();
            }
            return null;
        }

        public List<Entity.EBItem> GetPagerList(DateTime? beginDate, DateTime? endDate, string name, string electricBusiness, int pageSize, int pageIndex, out long totalCount)
        {
            var list = new MySqlEbItemDal().GetPagerList(beginDate, endDate, name, electricBusiness, pageSize, pageIndex, out totalCount);
            if (list != null)
            {
                return list.ToList();
            }
            return null;
        }
    }
}
