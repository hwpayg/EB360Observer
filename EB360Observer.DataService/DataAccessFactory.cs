using EB360Observer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB360Observer.DataService
{
    public class DataAccessFactory
    {
        /// <summary>
        /// 获取DataAccess
        /// </summary>
        /// <param name="dataSourceEnum"></param>
        /// <returns></returns>
        public static IDataAccess GetDataAccess(DataSourceEnum dataSourceEnum)
        {
            switch (dataSourceEnum)
            {
                case DataSourceEnum.Mongo:
                    return new MongoDataAccess();
                case DataSourceEnum.MsSql:
                    return new MsSqlDataAccess();
                case DataSourceEnum.MySql:
                    return new MySqlDataAccess();
                default:
                    return new MongoDataAccess();
            }
        }
    }
}
