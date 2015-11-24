using EB360Observer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB360Observer.DataService
{
    public interface IDataAccess
    {
        long Insert(EBItem item);

        EBItem GetLastestItemBySkuid(string skuid, string electricBusiness);

        List<EBItem> GetList(DateTime? beginDate, DateTime? endDate, string name, string electricBusiness);

        List<EBItem> GetPagerList(DateTime? beginDate, DateTime? endDate, string name, string electricBusiness, int pageSize, int pageIndex,out long totalCount);
    }
}
