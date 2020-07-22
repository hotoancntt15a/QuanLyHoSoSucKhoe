using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using zModules;
using zModules.AspNet;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Areas.sys.Controllers
{
    public class historyController : Controller
    {
        private string tablename = "sys_history"; 

        [Properties(Name = "Nhật ký", Note = "fa fa-history")]
        public ActionResult Index()
        {
            string name = Request["name"] == null ? "" : Request["name"].Trim();
            string vitri = Request["vitri"] == null ? "" : Request["vitri"].Trim();
            int pagesize = AppConfig.pageSize;
            int pageIndex = Request.getPageIndex();
            ViewBag.Name = name;
            ViewBag.ViTri = vitri;
            ViewBag.Page = "";
            var tsql = "";
            /* Build where */
            var s = new List<string>();
            if (name != "") { s.Add(name.buildLikeSql("iduser")); }
            if (string.IsNullOrEmpty(vitri) == false) { s.Add(vitri.buildLikeSql("[action]")); }
            var w = s.Count == 0 ? "" : $" where {string.Join(" and ", s)}";
            s = new List<string>();
            var db = local.getDataObject();
            try
            {
                ViewBag.Page = int.Parse($"{db.getValue($"select count(id) as x from [{tablename}] {w}")}").BootstrapPage(ref pageIndex, pagesize, idform: "frm_history");
                tsql = $"select * from (select {db.tsqlRowid("times desc")},* from [{tablename}] {w}) as t1 where {db.tsqlRowidPage(pageIndex, pagesize, "t1.rowid")} order by times desc";
                var d = db.getDataSet(tsql).Tables[0];
                d.Columns.RemoveAt(0);
                foreach (DataRow r in d.Rows)
                {
                    /* <tr> <td class=\"txtc\"><input type=\"checkbox\" name=\"id\" value=\"{id}\"></td> <td>{user}</td> <td>{action}</td> <td>{content}</td> <td>{time}</td> </tr> */
                    s.Add($"<tr> <td><input type=\"checkbox\" name=\"id\" value=\"{r["id"]}\"></td> <td>{r["times"]:dd/MM/yyyy HH:mm}</td> <td><a href=\"javascript:void(0);\" onclick=\"viewfirst(this);\"> {r["iduser"]}</a></td> <td>{r["action"]}</td> <td>{r["content"]}</td> </tr>");
                }
            }
            catch (Exception ex) { ViewBag.Error = ex.saveMessage(); }
            db.Dispose();
            ViewBag.Data = string.Join(" ", s);
            ViewBag.PageIndex = pageIndex;
            return View();
        }

        public ActionResult Delete()
        {
            string id = Request.getValue("id");
            if (id == "")
            {
                Session.saveError("Chưa tích vào các dòng cần xóa");
                return RedirectToAction("Index");
            }
            var db = local.getDataObject();
            try
            {
                db.Execute($"delete from [{tablename}] where id in ({id})");
                Session.saveMessage("Xóa các dòng nhật ký thành công " + id);
                local.setHistory();
            }
            catch (Exception ex) { Session.saveError(ex); }
            db.Dispose();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteAll()
        {
            var db = local.getDataObject();
            try
            {
                db.truncate(tablename);
                Session.saveMessage("Xóa toàn bộ nhật ký thành công");
                local.setHistory();
            }
            catch (Exception ex) { Session.saveError(ex); }
            db.Dispose();
            return RedirectToAction("Index");
        }
    }
}