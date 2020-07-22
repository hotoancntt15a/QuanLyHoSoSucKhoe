using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using zModules;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Areas.danhmuc.Controllers
{
    public class quanheController : Controller
    {
        // GET: danhmuc/quanhe
        [Properties(Name = "Quan hệ", Note = "fa fa-list")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult timkiem()
        {
            var where = new List<string>();
            var tmp = Request.getValue("id");
            if (string.IsNullOrEmpty(tmp) == false) { where.Add(tmp.buildLikeSql("id")); }
            tmp = Request.getValue("ten");
            if (string.IsNullOrEmpty(tmp) == false) { where.Add(tmp.buildLikeSql("quanhe")); }
            tmp = where.Count > 0 ? " where " + string.Join(" and ", where) : "";
            var tsql = "select * from dmmoiquanhe " + tmp + " order by quanhe";
            using (var db = local.getDataObject())
            {
                try { ViewBag.data = db.getDataSet(tsql).Tables[0]; }
                catch (Exception ex) { return Content("<div class=\"alert alert-danger\">" + ex.Message + "</div>"); }
            }
            return View();
        }
        public ActionResult capnhat()
        {
            return View();
        }
    }
}