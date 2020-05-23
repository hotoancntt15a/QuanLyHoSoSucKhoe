using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using zModules;
using zModules.AspNet;

namespace QuanLyHoSoSucKhoe.Areas.sys.Controllers
{
    public class filesController : Controller
    {
        [CheckLogin]
        [Properties(Name = "Quản lý tập tin", Note = "fa fa-file")]
        public ActionResult Index()
        {
            return View();
        }

        [CheckLogin]
        public ActionResult views()
        {
            var ps = Server.MapPath("~");
            var path = Request.getValue("p");
            if (string.IsNullOrEmpty(path)) { path = ps; }
            else { path = ps + path; }
            try
            {
                var d = new System.IO.DirectoryInfo(path);
                if (d.Exists == false) { return Content($"Thư mục: {path} không tồn tại trên hệ thống".BootstrapAlter(KeyBootstrapAlerts.Danger)); }
                ViewBag.folders = d.GetDirectories().OrderBy(p => p.Name).ToList();
                ViewBag.files = d.GetFiles().OrderBy(p => p.Name).ToList();
                var ls = new List<string>();
                var s = d.FullName.Replace(ps, "").Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                var temp = "";
                var link = Url.Action("views");
                ls.Add($"<a href=\"javascript:viewfolders('{link}')\"> .. </a>");
                foreach (var v in s)
                {
                    temp = $"{temp}/{v}";
                    ls.Add($"<a href=\"javascript:viewfolders('{link}?p={Server.UrlPathEncode(temp)}')\"> {v} </a>");
                }
                ViewBag.vitri = string.Join(" \\ ", ls);
            }
            catch (Exception ex) { return Content(ex.Message.BootstrapAlter(KeyBootstrapAlerts.Danger)); }
            return View();
        }
    }
}