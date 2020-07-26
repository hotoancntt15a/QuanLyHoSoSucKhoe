using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc; 
using zModules;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Areas.danhmuc.Controllers
{
    public class bacsyController : Controller
    {
        private string tableName = "bacsy";
        // GET: danhmuc/bacsy
        [Properties(Name = "Bác sỹ", Note = "fa fa-list")]
        public ActionResult Index() { return View(); }
        public ActionResult delid()
        {
            var id = Request.getValue("id");
            if (string.IsNullOrEmpty(id)) { return Content("<div class=\"alert alert-danger\">Tham số không đúng: mã</div>"); }
            using (var db = local.getDataObject())
            {
                try
                {
                    var tsql = $"delete from {tableName} where id=N'{id.getValueField()}'";
                    db.ExecuteCommand(tsql);
                }
                catch (Exception ex) { return Content("<div class=\"alert alert-danger\">" + ex.saveMessage() + "</div>"); }
            }
            return Content("<div class=\"alert alert-info\">" + messageKey.actionSuccess + "</div>");
        }
        public ActionResult timkiem()
        {
            var where = new List<string>();
            var tmp = Request.getValue("id");
            if (string.IsNullOrEmpty(tmp) == false) { where.Add(tmp.buildLikeSql("p1.id")); }
            tmp = Request.getValue("ten");
            if (string.IsNullOrEmpty(tmp) == false) { where.Add(tmp.buildLikeSql("p1.hoten")); }
            tmp = Request.getValue("khoa");
            if (string.IsNullOrEmpty(tmp) == false) { where.Add(tmp.buildLikeSql("p2.khoa")); }
            tmp = where.Count > 0 ? " where " + string.Join(" and ", where) : "";
            var tsql = $"select p1.*, p2.khoa from {tableName} as p1 inner join dmkhoa as p2 on p1.idkhoa = p2.id " + tmp + " order by p1.hoten";
            using (var db = local.getDataObject())
            {
                try { ViewBag.data = db.getDataSet(tsql).Tables[0]; }
                catch (Exception ex) { return Content("<div class=\"alert alert-danger\">" + ex.Message + "</div>"); }
            }
            return View();
        }
        public ActionResult capnhat()
        {
            var id = Request.getValue("id");
            if (Request["mode"] != null)
            {
                var data = new Dictionary<string, string>();
                try
                {
                    data["id"] = Request.getValue("ma").Trim();
                    data["hoten"] = Request.getValue("hoten").Trim();
                    data["ngaysinh"] = Request.getValue("ngaysinh").Trim();
                    data["gioitinh"] = Request.getValue("gioitinh").Trim();
                    data["idkhoa"] = Request.getValue("idkhoa").Trim();
                    data["ghichu"] = Request.getValue("ghichu").Trim();

                    if (string.IsNullOrEmpty(data["id"])) { throw new Exception("Tham số không đúng: mã"); }
                    if (string.IsNullOrEmpty(data["hoten"])) { throw new Exception("Tham số không đúng: họ tên"); }
                    if (string.IsNullOrEmpty(id) == false) { if (id != data["id"]) { throw new Exception("Tham số không đúng mã - id"); } }
                }
                catch (Exception ex) { return Content("<div class=\"alert alert-danger\">" + ex.Message + "</div>"); }
                using (var db = local.getDataObject())
                {
                    try
                    {
                        var tmp = $"{db.getValue($"select id from {tableName} where id=N'{data["id"].getValueField()}'")}";
                        if (tmp.Length > 10) { tmp = ""; }
                        if (!string.IsNullOrEmpty(tmp)) { tmp = $"id=N'{data["id"].getValueField()}'"; data.Remove("id"); }
                        db.Execute(data, tableName, tmp);
                        return Content("<div class=\"alert alert-info\">" + messageKey.actionSuccess + "</div>");
                    }
                    catch (Exception ex) { return Content($"<div class=\"alert alert-danger\">{ex.saveMessage()}</div>"); }
                }
            }
            else
            {
                using (var db = local.getDataObject())
                {
                    var tsql = $"select * from {tableName} where id = N'{id.getValueField()}'";
                    try { 
                        ViewBag.data = db.getValueDefaultRow(tsql);
                        var dmkhoa = db.getDataSet("select id,khoa from dmkhoa order by khoa").Tables[0];
                        if(dmkhoa.Rows.Count == 0) { throw new Exception("Chưa có danh mục khoa; Hãy cập nhật danh mục khoa trước khi thao tác"); }
                        ViewBag.dmkhoa = dmkhoa;
                    }
                    catch (Exception ex) { return Content($"<div class=\"alert alert-danger\">{ex.saveMessage(tsql)}</div>"); }
                }
            }
            return View();
        }
    }
}