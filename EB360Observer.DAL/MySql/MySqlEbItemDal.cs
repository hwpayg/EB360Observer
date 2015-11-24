using EB360Observer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
namespace EB360Observer.DAL
{
    public class MySqlEbItemDal
    {
        private readonly string SqlConnectionStr = System.Configuration.ConfigurationManager.ConnectionStrings["EBMySql"].ToString();

        //获取MySql的连接数据库对象。MySqlConnection  
        public MySqlConnection OpenConnection()
        {
            MySqlConnection connection = new MySqlConnection(SqlConnectionStr);
            connection.Open();
            return connection;
        }

        public long Insert(EBItem item)
        {
            using (MySqlConnection conn = OpenConnection())
            {
                const string query = "insert into EBItems(CreateTime,UpdateTime,Skuid,Name,Price,PriceDesc,Gift,PromotionDesc,ProductUrl,ElectricBusiness) values(@CreateTime,@UpdateTime,@Skuid,@name,@Price,@PriceDesc,@Gift,@PromotionDesc,@ProductUrl,@ElectricBusiness)";
                int row = conn.Execute(query, item);
                //更新对象的Id为数据库里新增的Id,假如增加之后不需要获得新增的对象，  
                //只需将对象添加到数据库里，可以将下面的一行注释掉。  
                SetIdentity(conn, id => item.Id = id, "id", "EBItems");
                return item.Id;
            }
        }
        public void SetIdentity(IDbConnection conn, Action<int> setId, string primarykey
                          , string tableName)
        {
            if (string.IsNullOrEmpty(primarykey)) primarykey = "id";
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("tableName参数不能为空，为查询的表名");
            }
            string query = string.Format("SELECT max({0}) as Id FROM {1}", primarykey
                                 , tableName);
            NewId identity = conn.Query<NewId>(query, null).Single<NewId>();
            setId(identity.Id);
        }
        public class NewId
        {
            public int Id { get; set; }
        }

        public EBItem GetLatestItemBySkuid(string skuid, string electricBusiness)
        {
            using (MySqlConnection conn = OpenConnection())
            {
                const string query = "select * from EBItems where Skuid=@skuid and ElectricBusiness=@electricBusiness order by CreateTime desc";
                var list = conn.Query<EBItem>(query, new { skuid = skuid, electricBusiness = electricBusiness });
                if (list != null && list.Count() > 0)
                {
                    return list.First();
                }
                return null;
            }
        }

        public IEnumerable<EBItem> GetList(DateTime? beginDate, DateTime? endDate, string name, string electricBusiness)
        {
            using (MySqlConnection conn = OpenConnection())
            {
                string query = "select * from EBItems where 1=1";
                if (beginDate.HasValue)
                {
                    query = query + " and CreateTime>=@beginDate";
                }
                if (endDate.HasValue)
                {
                    query = query + " and CreateTime<@endDate";
                }
                if (!string.IsNullOrWhiteSpace(name))
                {

                    query = query + " and Name like CONCAT(\"%\", @name, \"%\")";
                }
                if (!string.IsNullOrWhiteSpace(electricBusiness))
                {
                    query = query + " and ElectricBusiness=@electricBusiness";
                }
                return conn.Query<EBItem>(query, new
                {
                    beginDate = beginDate.HasValue ? beginDate.Value : new DateTime(),
                    endDate = endDate.HasValue ? endDate.Value.AddDays(1) : new DateTime(),
                    name = name,
                    electricBusiness = electricBusiness
                }).ToList();
            }
        }

        public IEnumerable<EBItem> GetPagerList(DateTime? beginDate, DateTime? endDate, string name, string electricBusiness, int pageSize, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            using (MySqlConnection conn = OpenConnection())
            {
                string query = "select * from EBItems where 1=1";
                var totalCountQuery = "select count(0) from EBItems where 1=1";
                var condition = string.Empty;
                if (beginDate.HasValue)
                {
                    condition = condition + " and CreateDate>=@beginDate";

                }
                if (endDate.HasValue)
                {
                    condition = condition + " and CreateDate<@endDate";
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    condition = condition + " and Name like CONCAT(\"%\", @name, \"%\")";
                }
                if (!string.IsNullOrWhiteSpace(electricBusiness))
                {
                    condition = condition + " and ElectricBusiness=@electricBusiness";
                }
                query += condition;
                totalCountQuery += condition;
                query = query + " order by CreateTime desc limit @start,@end";
                query = totalCountQuery + ";" + query;
                var result = conn.QueryMultiple(query, new
                {
                    beginDate = beginDate.HasValue ? beginDate.Value : new DateTime(),
                    endDate = endDate.HasValue ? endDate.Value.AddDays(1) : new DateTime(),
                    name = name,
                    electricBusiness = electricBusiness,
                    start = pageSize * pageIndex,
                    end = pageSize
                });
                totalCount = result.Read<long>().Single();
                return result.Read<EBItem>();
            }
        }
    }
}
