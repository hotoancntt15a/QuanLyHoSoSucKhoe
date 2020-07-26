﻿using System;
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
        private string tableName = "dmquanhe";
        // GET: danhmuc/quanhe
        [Properties(Name = "Quan hệ", Note = "fa fa-list")]
        public ActionResult Index() { return View(); }
        public ActionResult timkiem()
        {
            var where = new List<string>();
            var tmp = Request.getValue("id");
            if (string.IsNullOrEmpty(tmp) == false) { where.Add(tmp.buildLikeSql("id")); }
            tmp = Request.getValue("ten");
            if (string.IsNullOrEmpty(tmp) == false) { where.Add(tmp.buildLikeSql("quanhe")); }
            tmp = where.Count > 0 ? " where " + string.Join(" and ", where) : "";
            var tsql = $"select * from {tableName} {tmp} order by quanhe";
            using (var db = local.getDataObject())
            {
                try { ViewBag.data = db.getDataSet(tsql).Tables[0]; }
                catch (Exception ex) { return Content("<div class=\"alert alert-danger\">" + ex.Message + "</div>"); }
            }
            return View();
        }
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
        public ActionResult capnhat()
        {
            var id = Request.getValue("id");
            if (Request["mode"] != null)
            {
                var data = new Dictionary<string, string>();
                try
                {
                    data["id"] = Request.getValue("ma").Trim();
                    data["quanhe"] = Request.getValue("quanhe").Trim();
                    data["viettat"] = data["quanhe"].xoaDauTV().ToLower();
                    if (string.IsNullOrEmpty(data["id"])) { throw new Exception("Tham số không đúng: mã"); }
                    if (string.IsNullOrEmpty(data["quanhe"])) { throw new Exception("Tham số không đúng: quan hệ"); }
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
                    catch (Exception ex) { return Content("<div class=\"alert alert-danger\">" + ex.saveMessage() + "</div>"); }
                }
            }
            else
            {
                using (var db = local.getDataObject())
                {
                    var tsql = $"select * from dmmoiquanhe where id = N'{id.getValueField()}'";
                    try { ViewBag.data = db.getValueDefaultRow(tsql); }
                    catch (Exception ex) { return Content("<div class=\"alert alert-danger\">" + ex.saveMessage(tsql) + "</div>"); }
                }
            }
            return View();
        }
    }
}