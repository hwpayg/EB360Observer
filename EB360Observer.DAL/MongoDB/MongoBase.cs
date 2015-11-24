using System;
using System.Collections.Generic;
using System.Linq;
using LsGrep.Hwpayg.Lib.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace EB360Observer.DAL
{
    /// <summary>
    /// MongoDB数据访问类基类
    /// </summary>
    public class MongoBase<T> where T : class, new()
    {
        //MongoSession
        protected readonly MongoSession Session;
        //webconfig中MongoDB数据库连接名称,默认为MongoDB，可更改
        protected const string DefaultConfigNode = "EB";
        //默认MongoDB数据库名称，可更改
        protected const string DefaultDbName = "EB";
        //默认MongoDB数据库为读写分离
        protected const bool IsSlaveOk = true;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <param name="configNode">数据库连接</param>
        public MongoBase(string dbName = DefaultDbName, string configNode = DefaultConfigNode)
        {
            Session = new MongoSession(dbName, configNode);
        }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MongoBase()
        {
            Session = new MongoSession(DefaultDbName, DefaultConfigNode, isSlaveOk: IsSlaveOk);
        }

        /// <summary>
        /// 根据查询条件获取实体
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 根据查询条件和排序条件获取实体
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sortBy"></param>
        /// <param name="fieldsDict"></param>
        /// <returns></returns>
        public virtual T Get(IMongoQuery query, IMongoSortBy sortBy = null, IDictionary<string, int> fieldsDict = null)
        {
            return Session.Get<T>(query, sortBy, GetFields(fieldsDict));
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fieldsDict"></param>
        /// <returns></returns>
        public virtual T Get(long id, IDictionary<string, int> fieldsDict = null)
        {
            return Get(Query.EQ("_id", id), null, fieldsDict);
        }

        /// <summary>
        /// 统计符合查询条件的记录数
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual long Count(IMongoQuery query)
        {
            return Session.Count<T>(query);
        }

        /// <summary>
        /// 根据查询条件判断记录是否存在
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual bool IsExists(IMongoQuery query)
        {
            if (Count(query) > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 根据ID判断记录是否存在
        /// </summary>
        /// <returns></returns>
        public virtual bool IsExists(long id)
        {
            return IsExists(Query.EQ("_id", id));
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        public virtual void Update(IMongoQuery query, IMongoUpdate update, UpdateFlags updateFlag = UpdateFlags.None)
        {
            Session.Update<T>(query, update, updateFlag);
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(T entity)
        {
            Session.Update(entity);
        }

        /// <summary>
        /// 字段增减操作
        /// </summary>
        /// <param name="query"></param>
        /// <param name="fieldName"></param>
        /// <param name="val"></param>
        public virtual void IncFieldCount(IMongoQuery query, string fieldName, long val)
        {
            Session.Inc<T>(query, fieldName, val);
        }

        /// <summary>
        /// 排序获取,默认取前10条
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="sortBy">排序条件</param>
        /// <param name="count">条数</param>
        /// <param name="fields">选择字段</param>
        /// <returns></returns>
        public virtual MongoCursor<T> Top(IMongoQuery query, IMongoSortBy sortBy = null, int count = 10, IDictionary<string, int> fields = null)
        {
            return Session.Top<T>(query, sortBy, count, GetFields(fields));
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="sortBy">排序条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="fields">选择字段</param>
        /// <returns></returns>
        public virtual MongoCursor<T> GetList(IMongoQuery query = null, IMongoSortBy sortBy = null, int pageIndex = 0, int pageSize = 0, IDictionary<string, int> fields = null)
        {
            return Session.Query<T>(query, sortBy, pageIndex, pageSize, GetFields(fields));
        }

        /// <summary>
        /// 获取实体列表（用于非标准分页）
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="start">当前第几条</param>
        /// <param name="sortBy">排序条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="fields">选择字段</param>
        /// <returns></returns>
        public virtual MongoCursor<T> GetList(int start, IMongoQuery query = null, IMongoSortBy sortBy = null, int pageIndex = 0, int pageSize = 0, IDictionary<string, int> fields = null)
        {
            return Session.Query<T>(start, query, sortBy, pageIndex, pageSize, GetFields(fields));
        }

        /// <summary>
        /// 找到并修改对象(原子性操作)
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sortyBy"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public virtual T FindAndModify(IMongoQuery query, IMongoSortBy sortyBy = null, IMongoUpdate update = null)
        {
            return Session.FindAndModify<T>(query, sortyBy, update);
        }

        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="fieldsDict"></param>
        /// <returns></returns>
        protected FieldsDocument GetFields(IDictionary<string, int> fieldsDict)
        {
            FieldsDocument fieldDocument = null;
            if (fieldsDict != null && fieldsDict.Count() > 0)
            {
                fieldDocument = new FieldsDocument();

                foreach (var field in fieldsDict)
                {
                    fieldDocument.Add(new BsonElement(field.Key, field.Value));
                }
            }
            return fieldDocument;
        }


        #region Linq操作

        /// <summary>
        /// linq 根据查询条件和排序条件获取实体
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public T Get(Expression<Func<T, bool>> filter)
        {
            return Session.Get(filter);
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体
        /// </summary>
        /// <returns></returns>
// ReSharper disable once MethodOverloadWithOptionalParameter
        public virtual T Get(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null)
        {
            return Session.Get(filter, orderby);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="dynamicPredicate"></param>
        /// <param name="dynamicFunc"></param>
        /// <returns></returns>
        public virtual T Get(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> dynamicPredicate, Func<dynamic, T> dynamicFunc)
        {
            return Session.Get(predicate, dynamicPredicate, dynamicFunc);
        }

        /// <summary>
        /// distinct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="key"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public virtual IEnumerable<dynamic> Distinct(Expression<Func<T, bool>> filter, Expression<Func<T, dynamic>> key, IEqualityComparer<dynamic> comparer = null)
        {
            return Session.Distinct(filter, key, comparer);
        }

        /// <summary>
        /// 多个条件的distinct
        /// </summary>
        /// <param name="expressionList"></param>
        /// <param name="key"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public virtual IEnumerable<dynamic> Distinct(IEnumerable<Expression<Func<T, bool>>> expressionList, Expression<Func<T, dynamic>> key,
            IEqualityComparer<dynamic> comparer = null)
        {
            return Session.Distinct(expressionList, key, comparer);
        }

        /// <summary>
        /// 统计符合查询条件的记录数 linq,type参数无任何意义，只是为了区分与原先的count的混淆
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual long Count(Expression<Func<T, bool>> filter, string type = "linq")
        {
            return Session.Count(filter);
        }
        /// <summary>
        /// 多条件count,type参数无任何意义，只是为了区分与原先的count的混淆
        /// </summary>
        /// <param name="expressionList"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual long Count(IEnumerable<Expression<Func<T, bool>>> expressionList, string type = "linq")
        {
            return Session.Count(expressionList);
        }

        /// <summary>
        /// 根据查询条件判断记录是否存在 linq
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual bool IsExists(Expression<Func<T, bool>> filter)
        {
            if (Count(filter) > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 排序获取,默认取前10条 linq
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderby"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Top(Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, int topCount = 10)
        {
            return Session.Top(filter, orderby, topCount);
        }

        ///// <summary>
        ///// 排序获取，默认前10条 linq
        ///// </summary>
        ///// <param name="filter"></param>
        ///// <param name="entry"></param>
        ///// <param name="topCount"></param>
        ///// <returns></returns>
        //public virtual IEnumerable<T> Top(Expression<Func<T, bool>> filter,
        //   QueryableOrderEntry<T> entry = null, int topCount = 10)
        //{
        //    return _session.Top<T>(filter, entry, topCount);
        //}

        /// <summary>
        /// 排序获取
        /// </summary>
        /// <param name="expressionList"></param>
        /// <param name="orderby"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Top(IEnumerable<Expression<Func<T, bool>>> expressionList,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, int topCount = 10)
        {
            return Session.Top(expressionList, orderby, topCount);
        }

        ///// <summary>
        ///// 排序获取
        ///// </summary>
        ///// <param name="expressionList"></param>
        ///// <param name="entry"></param>
        ///// <param name="topCount"></param>
        ///// <returns></returns>
        //public virtual IEnumerable<T> Top(IEnumerable<Expression<Func<T, bool>>> expressionList,
        //     QueryableOrderEntry<T> entry, int topCount = 10)
        //{
        //    return _session.Top<T>(expressionList, entry, topCount);
        //}

        /// <summary>
        /// 获取实体列表 linq
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderby"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetList(Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, int pageIndex = 0, int pageSize = 0)
        {
            return Session.Query(filter, orderby, pageIndex, pageSize);
        }

        ///// <summary>
        ///// 获取实体列表
        ///// </summary>
        ///// <param name="filter"></param>
        ///// <param name="entry"></param>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <returns></returns>
        //public virtual IEnumerable<T> GetList(Expression<Func<T, bool>> filter,
        //     QueryableOrderEntry<T> entry = null, int pageIndex = 0, int pageSize = 0)
        //{
        //    return _session.Query<T>(filter, entry, pageIndex, pageSize);
        //}
        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="expressionList"></param>
        /// <param name="orderby"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetList(IEnumerable<Expression<Func<T, bool>>> expressionList,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, int pageIndex = 0, int pageSize = 0)
        {
            return Session.Query(expressionList, orderby, pageIndex, pageSize);
        }
        ///// <summary>
        ///// 获取实体列表
        ///// </summary>
        ///// <param name="expressionList"></param>
        ///// <param name="entry"></param>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <returns></returns>
        //public virtual IEnumerable<T> GetList(IEnumerable<Expression<Func<T, bool>>> expressionList,
        //     QueryableOrderEntry<T> entry = null, int pageIndex = 0, int pageSize = 0)
        //{
        //    return _session.Query<T>(expressionList, entry, pageIndex, pageSize);
        //}

        /// <summary>
        /// 获取实体列表（用于非标准分页） linq
        /// </summary>
        /// <param name="start"></param>
        /// <param name="filter"></param>
        /// <param name="orderby"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetList(int start, Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, int pageIndex = 0, int pageSize = 0)
        {
            return Session.Query(start, filter, orderby, pageIndex, pageSize);
        }
        ///// <summary>
        ///// 获取实体列表(用于非标准分页) linq
        ///// </summary>
        ///// <param name="start"></param>
        ///// <param name="filter"></param>
        ///// <param name="entry"></param>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <returns></returns>
        //public virtual IEnumerable<T> GetList(int start, Expression<Func<T, bool>> filter,
        //   QueryableOrderEntry<T> entry = null, int pageIndex = 0, int pageSize = 0)
        //{
        //    return _session.Query<T>(start, filter, entry, pageIndex, pageSize);
        //}
        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="start"></param>
        /// <param name="expressionList"></param>
        /// <param name="orderby"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetList(int start, IEnumerable<Expression<Func<T, bool>>> expressionList,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, int pageIndex = 0, int pageSize = 0)
        {
            return Session.Query(start, expressionList, orderby, pageIndex, pageSize);
        }
        ///// <summary>
        ///// 获取实体列表
        ///// </summary>
        ///// <param name="start"></param>
        ///// <param name="expressionList"></param>
        ///// <param name="entry"></param>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <returns></returns>
        //public virtual IEnumerable<T> GetList(int start, IEnumerable<Expression<Func<T, bool>>> expressionList,
        //     QueryableOrderEntry<T> entry = null, int pageIndex = 0, int pageSize = 0)
        //{
        //    return _session.Query<T>(start, expressionList, entry, pageIndex, pageSize);
        //}
        /// <summary>
        /// 获取linq
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetLinqCollection()
        {
            return Session.GetLinqCollection<T>();
        }
        #endregion
    }
}
