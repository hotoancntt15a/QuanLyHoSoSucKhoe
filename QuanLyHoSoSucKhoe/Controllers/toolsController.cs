﻿using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using zModules;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Controllers
{
    public class toolsController : Controller
    {
        // GET: tools
        public ActionResult Index()
        {
            var f = Request.getValue("f");
            if (string.IsNullOrEmpty(f)) { return View(); }
            var mi = (typeof(toolsController)).GetMethod(f);
            return Content($"{mi.Invoke(null, null)}");
        }
        public ActionResult optiondmhuyen()
        {
            var idtinh = Request.getValue("id");
            var setall = Request.getValue("all");
            var value = Request.getValue("value");
            var rs = new List<string>();
            if (setall == "1") { rs.Add("<option value=\"\"> -- Chọn -- </option>"); }
            using (var db = local.getDataObject())
            {
                try
                {
                    var dmhuyen = db.getDataSet($"select id,ten from dmhuyen where idtinh=N'{idtinh.getValueField()}' order by ten").Tables[0];
                    foreach (DataRow r in dmhuyen.Rows) { rs.Add($"<option value=\"{r[0]}\"{(value == r[0].ToString() ? " selected=\"selected\"" : "")}>{r[1]}</option>"); }
                }
                catch (Exception ex) { rs.Add($"<option value=\"\">{ex.saveMessage()}</option>"); }
            }
            return Content(string.Join("", rs));
        }
        public ActionResult optiondmxa()
        {
            var idhuyen = Request.getValue("id");
            var setall = Request.getValue("all");
            var value = Request.getValue("value");
            var rs = new List<string>();
            if (setall == "1") { rs.Add("<option value=\"\"> -- Chọn -- </option>"); }
            using (var db = local.getDataObject())
            {
                try
                {
                    var dmxa = db.getDataSet($"select id,ten from dmxa where idhuyen=N'{idhuyen.getValueField()}' order by ten").Tables[0];
                    foreach (DataRow r in dmxa.Rows) { rs.Add($"<option value=\"{r[0]}\"{(value == r[0].ToString() ? " selected=\"selected\"" : "")}>{r[1]}</option>"); }
                }
                catch (Exception ex) { rs.Add($"<option value=\"\">{ex.saveMessage()}</option>"); }
            }
            return Content(string.Join("", rs));
        }
        public ActionResult optiondmtinh()
        {
            var setall = Request.getValue("all");
            var value = Request.getValue("value");
            var rs = new List<string>();
            if (setall == "1") { rs.Add("<option value=\"\"> -- Chọn -- </option>"); }
            using (var db = local.getDataObject())
            {
                try
                {
                    var dmtinh = db.getDataSet($"select id,ten from dmtinh order by ten").Tables[0];
                    foreach (DataRow r in dmtinh.Rows) { rs.Add($"<option value=\"{r[0]}\"{(value == r[0].ToString() ? " selected=\"selected\"" : "")}>{r[1]}</option>"); }
                }
                catch (Exception ex) { rs.Add($"<option value=\"\">{ex.saveMessage()}</option>"); }
            }
            return Content(string.Join("", rs));
        }

        public ActionResult getFileFromClient()
        {
            if (Request.Files.Count == 0) { return Content("<div class=\"alert alert-danger\">Error Not File</div>"); }
            try
            {
                var path = "";
                var name = Request.getValue("name").ToLower();
                if (name.StartsWith("/temp/img") || name.StartsWith("img")) {
                    if(name.StartsWith("/temp/img")) { path = Server.MapPath("~"+name); if (System.IO.File.Exists(path)) { System.IO.File.Delete(path); } }
                    name = $"img{(long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds)}{Path.GetExtension(Request.Files[0].FileName).ToLower()}"; 
                } 
                else { name = Request.Files[0].FileName; }
                path = Folders.temp + "\\" + name;
                if (System.IO.File.Exists(path)) { System.IO.File.Delete(path); }
                Request.Files[0].SaveAs(path);
                return Content("<img src=\"/temp/" + name + "\" alt=\"\" />");
            }
            catch (Exception ex) { return Content("<div class=\"alert alert-danger\">" + ex.getLineHTML() + "</div>"); }
        }
    }
}