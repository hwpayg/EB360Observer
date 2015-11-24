using EB360Observer.Common;
using EB360Observer.DataService;
using EB360Observer.Entity;
using EB360Observer.Web.Conditions;
using EB360Observer.Web.Helpers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EB360Observer.Web.Controllers
{
    public class HomeController : Controller
    {
        public IEnumerable<string> Categorys { get; set; }
        public Dictionary<string, List<decimal>> Series { get; set; }
        public IEnumerable<string> Names { get; set; }
        //
        // GET: /Home/
        [HttpGet]
        public ActionResult Index()
        {
            var startDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            var endDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            BindData(startDate, endDate, "", "");
            ViewBag.Categorys = Categorys;
            ViewBag.Series = Series;
            return View();
        }
        [HttpPost]
        public ActionResult Index(DateTime? startDate, DateTime? endDate, string nam, int ElectricBusiness)
        {
            startDate = startDate ?? Convert.ToDateTime(DateTime.Now.ToShortDateString());
            endDate = endDate ?? Convert.ToDateTime(DateTime.Now.ToShortDateString());
            BindData(startDate.Value, endDate.Value, nam, EnumHelper.GetEnumDesc(ElectricBusiness));
            ViewBag.Categorys = Categorys;
            ViewBag.Series = Series;
            return View();
        }
        private void BindData(DateTime? startDate, DateTime? endDate, string nam, string electricBusiness)
        {

            var dataAccess = DataAccessFactory.GetDataAccess((DataSourceEnum)Enum.Parse(typeof(DataSourceEnum), AppConfigHelper.DataSourceType));
            var list = dataAccess.GetList(startDate, endDate, nam, electricBusiness);
            if (list != null && list.Count() > 0)
            {
                var itemList = list.ToList();
                Categorys = itemList.Select(p => p.CreateTime.Value.ToString("yyyy-MM-dd HH点")).Distinct();

                Names = itemList.Select(p => "[" + p.ElectricBusiness + "]" + p.Name).Distinct();
                foreach (var name in Names)
                {
                    foreach (var category in Categorys)
                    {
                        if (itemList.Exists(p => p.CreateTime.Value.ToString("yyyy-MM-dd HH点") == category && ("[" + p.ElectricBusiness + "]" + p.Name) == name))
                        {
                            var existItem = itemList.First(p => p.CreateTime.Value.ToString("yyyy-MM-dd HH点") == category && ("[" + p.ElectricBusiness + "]" + p.Name) == name);
                            if (Series == null)
                            {
                                Series = new Dictionary<string, List<decimal>>();
                            }
                            if (Series.Keys.Contains(name))
                            {
                                if (Series[name] == null)
                                {
                                    Series[name] = new List<decimal>();
                                }
                                Series[name].Add(existItem.Price);
                            }
                            else
                            {
                                Series.Add(name, new List<decimal>());
                                Series[name].Add(existItem.Price);
                            }
                        }
                        else
                        {
                            if (Series == null)
                            {
                                Series = new Dictionary<string, List<decimal>>();
                            }
                            if (Series.Keys.Contains(name))
                            {
                                if (Series[name] == null)
                                {
                                    Series[name] = new List<decimal>();
                                }
                                Series[name].Add(0);
                            }
                            else
                            {
                                Series.Add(name, new List<decimal>());
                                Series[name].Add(0);
                            }
                        }
                    }
                }
            }
        }


        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ListView(ItemListSearchCondition condition)
        {
            var list = new List<EBItem>();
            var totalCount = 0L;
            if (condition != null)
            {

                var dataAccess = DataAccessFactory.GetDataAccess((DataSourceEnum)Enum.Parse(typeof(DataSourceEnum), AppConfigHelper.DataSourceType));
                list = dataAccess.GetPagerList(condition.BeginDate, condition.EndDate, condition.Name, EnumHelper.GetEnumDesc(condition.ElectricBusiness), condition.PageSize, condition.PageIndex - 1, out totalCount);
            }
            var strPager = "";
            if (list != null && list.Count > 0)
            {
                strPager = new Pager().GetJumperForAjax(condition.PageIndex, condition.PageSize, (Int32)totalCount, "searchList({0})");
            }
            ViewBag.PageIndex = condition.PageIndex;
            ViewBag.ItemList = list;
            ViewBag.StrPager = strPager;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
