using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using zModules;
using zModules.AspNet;
using zModules.Crypt;
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
        [CheckLogin()]
        public ActionResult update()
        {
            var data = new Dictionary<string, string>();
            var mode = Request.getValue("mode").decryptMD5(AppConfig.Key);
            var w = "";
            data["id"] = Request.getValue("id");
            data["khoa"] = Request.getValue("khoa");
            data["ghichu"] = Request.getValue("ghichu");
            try
            {
                if (Regex.IsMatch(mode, "^[01]|[0-9]{2}/[0-9]{2}/[0-9]{4} [0-9]{2}:[0-9]{2}:[0-9]{2}$") == false) { throw new Exception(messageKey.thamSoKhongDung); }
                if (string.IsNullOrEmpty(data["khoa"])) { throw new Exception("Tên khoa để trống"); }
                if (string.IsNullOrEmpty(data["id"]) == false)
                {
                    if (Regex.IsMatch(data["id"], "^[0-9a-zA-Z]+$") == false) { throw new Exception(messageKey.thamSoKhongDung); }
                }
                if (mode.StartsWith("1"))
                {
                    if (data["id"] == "") { throw new Exception(messageKey.thamSoKhongDung); }
                    w = $"id = N'{data["id"]}'";
                    data.Remove("id");
                } else { data["id"] = $"{DateTime.Now:yyyyMMddHHmmss}"; }

            }
            catch (Exception ex) { return Content(ex.Message.BootstrapAlter("danger")); }
            using (var db = local.getDataObject())
            {
                try { db.Execute(data, "dmkhoa", w); }
                catch (Exception ex) { return Content(ex.saveMessage().BootstrapAlter("danger")); }
            }
            return Content(messageKey.actionSuccess.BootstrapAlter());
        }
        public ActionResult viewid()
        {
            var id = Request.getValue("id");
            if (string.IsNullOrEmpty(id) == false)
            {
                if (Regex.IsMatch(id, "^[A-Za-z0-9]+$") == false) { return Content(messageKey.thamSoKhongDung.BootstrapAlter("danger")); }
            }
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.mode = $"0|{DateTime.Now:dd/MM/yyyy HH:mm:ss}".encryptMD5(AppConfig.Key);
                var data = new DataTable("dmkhoa");
                data.Columns.Add("id"); data.Columns.Add("khoa"); data.Columns.Add("ghichu");
                data.Rows.Add("", "", "");
                ViewBag.data = data;
                return View();
            }
            using (var db = local.getDataObject())
            {
                try
                {
                    ViewBag.mode = $"1|{DateTime.Now:dd/MM/yyyy HH:mm:ss}".encryptMD5(AppConfig.Key);
                    ViewBag.data = db.getDataSet($"select * from dmkhoa where id=N'{id}'").Tables[0];
                }
                catch (Exception ex) { return Content(ex.saveMessage().BootstrapAlter("danger")); }
            }
            return View();
        }
    }
}