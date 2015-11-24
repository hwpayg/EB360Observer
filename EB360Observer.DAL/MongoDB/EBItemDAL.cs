using EB360Observer.Entity;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;


namespace EB360Observer.DAL
{
    public class EBItemDAL : MongoBase<EBItem>
    {
        public long Insert(EBItem item)
        {
            var id = Session.CreateIncId<EBItem>();
            item.Id = id;
            Session.Insert(item);
            return id;
        }

        /// <summary>
        /// 根据skuid获取创建时间最近的数据
        /// </summary>
        /// <param name="skuid"></param>
        /// <returns></returns>
        public EBItem GetLatestItemBySkuid(string skuid,string electricBusiness)
        {
            return Session.Get<EBItem>(Query.And(Query.EQ("Skuid", skuid), Query.EQ("ElectricBusiness", electricBusiness)), new SortByBuilder().Descending("CreateTime"));
        }

        public IEnumerable<EBItem> GetList(DateTime? beginDate, DateTime? endDate, string name, string electricBusiness)
        {
            IMongoQuery query;
            query = Query.GT("_id", 0);
            if (beginDate.HasValue)
            {
                query = Query.And(query, Query.GTE("CreateTime", beginDate));
            }
            if (endDate.HasValue)
            {
                query = Query.And(query, Query.LT("CreateTime", endDate.Value.AddDays(1)));
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = Query.And(query, Query.Matches("Name", "/" + name + "/i"));
            }
            if (!string.IsNullOrWhiteSpace(electricBusiness))
            {
                query = Query.And(query, Query.EQ("ElectricBusiness", electricBusiness));
            }
            return GetList(query);
        }

        public MongoCursor<EBItem> GetPagerList(DateTime? beginDate, DateTime? endDate, string name, string electricBusiness, int pageSize, int pageIndex)
        {
            IMongoQuery query;
            query = Query.GT("_id", 0);
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = Query.And(query, Query.Matches("Name", "/" + name + "/i"));
            }
            if (!string.IsNullOrWhiteSpace(electricBusiness))
            {
                query = Query.And(query, Query.EQ("ElectricBusiness", electricBusiness));
            }
            if (beginDate.HasValue)
            {
                query = Query.And(query, Query.GTE("CreateTime", beginDate));
            }
            if (endDate.HasValue)
            {
                query = Query.And(query, Query.LT("CreateTime", endDate.Value.AddDays(1)));
            }
            IMongoSortBy sortBy;
            sortBy = new SortByBuilder().Descending("CreateTime");
            return GetList(query, sortBy, pageIndex, pageSize);
        }
    }
}
