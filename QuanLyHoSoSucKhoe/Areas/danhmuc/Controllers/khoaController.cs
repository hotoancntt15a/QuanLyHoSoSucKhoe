using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using zModules;
using zModules.AspNet;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Areas.danhmuc.Controllers
{
    public class khoaController : Controller
    {
        // GET: danhmuc/khoa

        [CheckLogin()]
        public ActionResult Index()
        {
            return View();
        }

        [CheckLogin()]
        public ActionResult tracuu()
        {
            var w = new List<string>();
            var tmp = Request.getValue("khoa");
            if (string.IsNullOrEmpty(tmp) == false) { w.Add(tmp.buildLikeSql("khoa")); }
            tmp = Request.getValue("ghichu");
            if (string.IsNullOrEmpty(tmp) == false) { w.Add(tmp.buildLikeSql("ghichu")); }
            tmp = w.Count > 0 ? "where " + string.Join(" and ", w) : "";
            using (var db = local.getDataObject())
            {
                try
                {
                    ViewBag.data = db.getDataSet("select * from dmkhoa " + tmp).Tables[0];
                }
                catch (Exception ex) { return Content(ex.saveMessage().BootstrapAlter("danger")); }
            }
            return View();
        }

        public ActionResult viewid()
        {
            var id = Request.getValue("id");
            if (string.IsNullOrEmpty(id) == false)
            {
                if (Regex.IsMatch(id, "^[A-Za-z0-9]+$") == false) { return Content(messageKey.thamSoKhongDung.BootstrapAlter("danger")); }
            }
            using (var db = local.getDataObject())
            {
                try
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        var data = new DataTable("dmkhoa");
                        data.Columns.Add("id"); data.Columns.Add("khoa"); data.Columns.Add("ghichu");
                        data.Rows.Add("", "", "");
                        ViewBag.data = data;
                    }
                    else { ViewBag.data = db.getDataSet($"select * from dmkhoa where id=N'{id}'").Tables[0]; }
                }
                catch (Exception ex) { return Content(ex.saveMessage().BootstrapAlter("danger")); }
            }
            return View();
        }
    }
}