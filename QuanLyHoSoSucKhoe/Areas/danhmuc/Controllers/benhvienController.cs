using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using zModules;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Areas.danhmuc.Controllers
{
    public class benhvienController : Controller
    {
        private string tableName = "dmbenhvien";
        // GET: danhmuc/benhvien
        [Properties(Name = "Bệnh viện", Note = "fa fa-list")]
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
            if (string.IsNullOrEmpty(tmp) == false) { where.Add(tmp.buildLikeSql("id")); }
            tmp = Request.getValue("ten");
            if (string.IsNullOrEmpty(tmp) == false) { where.Add(tmp.buildLikeSql("ten")); }
            tmp = Request.getValue("diachi");
            if (string.IsNullOrEmpty(tmp) == false) { where.Add(tmp.buildLikeSql("diachi")); }
            tmp = where.Count > 0 ? " where " + string.Join(" and ", where) : "";
            var tsql = $"select * from {tableName} " + tmp + " order by ten";
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
                    data["matinh"] = Request.getValue("idtinh").Trim();
                    data["mabv"] = Request.getValue("mabv").Trim();
                    data["ten"] = Request.getValue("ten").Trim();
                    data["diachi"] = Request.getValue("diachi").Trim();
                    data["sothe"] = Request.getValue("sothe").Trim();
                    data["doituongkcb"] = Request.getValue("doituongkcb").Trim();
                    data["mahuyen"] = Request.getValue("idhuyen").Trim();
                    data["dunghoatdong"] = Request.getValue("dunghoatdong").Trim(); 
                    if(data["dunghoatdong"] != "")
                    {
                        /* Convert */
                        if(Regex.IsMatch(data["dunghoatdong"], "^[0-9]{2}/[0-9]{2}/[0-9]{4}/$") == false) { throw new Exception("Thời gian dừng hoạt động không đúng " + data["dunghoatdong"]); } 
                        var tmp = data["dunghoatdong"].Split('/');
                        data["dunghoatdong"] = tmp[2] + tmp[1] + tmp[0];
                    }
                    if (string.IsNullOrEmpty(data["id"])) { throw new Exception("Tham số không đúng: mã"); }
                    if (string.IsNullOrEmpty(data["ten"])) { throw new Exception("Tham số không đúng: tên bệnh viện"); }
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
                        var item = db.getValueDefaultRow(tsql);
                        var tmp = $"{item["dunghoatdong"]}";
                        if (tmp != "")
                        {
                            tmp = tmp.Substring(tmp.Length - 2) + "/" + tmp.Substring(4, 2) + "/" + tmp.Substring(0, 4);
                            item["dunghoatdong"] = tmp;
                        }
                        ViewBag.data = item;
                        var idhuyen = $"{item["mahuyen"]}";
                        ViewBag.idhuyen = idhuyen;
                        var dmtinh = db.getDataSet("select id,ten from dmtinh order by ten").Tables[0];
                        if (dmtinh.Rows.Count == 0) { return Content($"<div class=\"alert alert-danger\">Không có dữ liệu tỉnh</div>"); }
                        var idtinh = $"{db.getValue($"select top 1 idtinh from dmhuyen where id=N'{idhuyen.getValueField()}'")}";
                        if (idtinh == "") { idtinh = $"{dmtinh.Rows[0]["id"]}"; }
                        ViewBag.idtinh = idtinh;
                        var dmhuyen = db.getDataSet($"select id,ten from dmhuyen where idtinh=N'{idtinh.getValueField()}' order by ten").Tables[0];
                        ViewBag.dmtinh = dmtinh;
                        ViewBag.dmhuyen = dmhuyen;
                    }
                    catch (Exception ex) { return Content($"<div class=\"alert alert-danger\">{ex.saveMessage(tsql)}</div>"); }
                }
            }
            return View();
        }
    }
}