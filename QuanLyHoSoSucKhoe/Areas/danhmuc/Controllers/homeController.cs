using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using zModules;
using zModules.AspNet;
using zModules.Crypt;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Areas.danhmuc.Controllers
{
    public class homeController : Controller
    {
        // GET: danhmuc/home
        public ActionResult Index()
        {
            return View();
        }


        [CheckLogin()]
        public ActionResult delid()
        {
            var id = Request.getValue("id");
            if (Regex.IsMatch(id, "^[0-9a-zA-Z,]+$") == false) { return Content(messageKey.thamSoKhongDung.BootstrapAlter("danger")); }
            var mode = Request.getValue("mode").decryptMD5(AppConfig.Key);
            if (Regex.IsMatch(mode, "^[01]|[0-9]{2}/[0-9]{2}/[0-9]{4} [0-9]{2}:[0-9]{2}:[0-9]{2}$") == false) { return Content(messageKey.thamSoKhongDung.BootstrapAlter("danger")); }
            var t = Request.getValue("t");
            if (Regex.IsMatch(t, "^[0-9a-zA-Z_]+$") == false) { return Content(messageKey.thamSoKhongDung.BootstrapAlter("danger")); }
            if (id.Contains(",") == false) { id = "id=N'{id}'"; }
            else
            {
                var dsid = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (dsid.Count() == 0) { return Content(messageKey.thamSoKhongDung.BootstrapAlter("danger")); }
                id = $"id IN (N'{string.Join("',N'", dsid)}')";
            }
            using (var db = local.getDataObject())
            {
                try { db.Execute($"delete form {t} where " + id); }
                catch (Exception ex) { return Content(ex.saveMessage().BootstrapAlter("danger")); }
            }
            return Content(messageKey.actionSuccess.BootstrapAlter());
        }
    }
}