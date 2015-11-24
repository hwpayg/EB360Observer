using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EB360Observer.Web.Conditions
{
    /// <summary>
    /// 列表搜索条件
    /// </summary>
    public class ItemListSearchCondition
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 电商名称
        /// </summary>
        public int ElectricBusiness { get; set; }
        /// <summary>
        /// 第多少页
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页多少条
        /// </summary>
        public int PageSize { get; set; }
    }
}