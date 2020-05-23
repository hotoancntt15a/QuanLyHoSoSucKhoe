using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using zModules;
using zModules.AspNet;
using zModules.Config;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Areas.sys.Controllers
{
    public class configController : Controller
    { 
        /* GET: hethong/config */

        [CheckLogin]
        [Properties(Name = "Cấu hình", Note = "fa fa-cogs")]
        public ActionResult Index()
        {
            using (var db = local.getDataObject())
            {
                try
                {
                    ViewBag.dmtinh = db.getDataSet("select * from dmtinh order by ten").Tables[0];
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                    ViewBag.dmtinh = new List<Models.dmtinh>();
                }
            }
            return View();
        }

        [CheckLogin(EveryOne:true)]
        public ActionResult load()
        {
            string tinh = Request.getValue("id");
            string pathConfig = local.getPathConfig(tinh);
            if (string.IsNullOrEmpty(pathConfig)) { return Redirect(Url.Action("index", "error", new { area = "" }) + "?message=" + Server.UrlEncode(messageKey.thamSoKhongDung)); }
            var d = new Dictionary<string, string>();
            d.GetFile(pathConfig);
            ViewBag.data = d;
            return View();
        }

        [CheckLogin(EveryOne: true)]
        public ActionResult save()
        {
            string tinh = Request.getValue("id");
            string pathConfig = local.getPathConfig(tinh);
            if (string.IsNullOrEmpty(pathConfig)) { return Redirect(Url.Action("index", "error", new { area = "" }) + "?message=" + Server.UrlEncode(messageKey.thamSoKhongDung)); }
            string key = Request.getValue("key");
            string value = Request.getValue("value");
            if (key == "") { return Content(messageKey.khongThamSo); }
            var cfg = new Config(pathConfig);
            if (Request.getValue("cmd") != "1") { cfg.ModifiItemAppSetting(key, value); }
            else { cfg.RemoveItemKeyValue(key); }
            local.setHistory();
            return Content(messageKey.actionSuccess);
        }

        [CheckLogin]
        public ActionResult fix()
        {
            string key = Request.getValue("key");
            string value = Request.getValue("value");
            string cmd = Request.getValue("cmd");
            string redirect = Request.getValue(keyS.redirect);
            try
            {
                ViewBag.value = value;
                if (cmd == "1") { key.setValueAppConfig(value); return Content($"<div>{messageKey.actionSuccess}</div>"); }
                else
                {
                    if (string.IsNullOrEmpty(key)) { return Content($"<div>{messageKey.khongThamSo}</div>"); }
                    ViewBag.value = AppConfig.value(key);
                }
            }
            catch (Exception ex) { return Content($"<div>{ex.Message}</div>"); }
            return View();
        }

        [CheckLogin]
        public ActionResult Variables()
        {
            var s = new List<string>();
            foreach (var v in Request.ServerVariables.AllKeys) { s.Add($"<tr><td>{v}</td><td>{Request.ServerVariables[v]}</td></tr>"); }
            ViewBag.data = string.Join("", s);
            return View();
        }

        [CheckLogin(EveryOne: true)]
        public ActionResult TableDescription(string id = "")
        {
            string tablename = string.IsNullOrEmpty(id) ? "table_description" : id;
            ViewBag.Title = string.IsNullOrEmpty(id) ? "Table Description" : "Column Description";
            string col1 = "column", col2 = "type", col3 = "description";
            if (string.IsNullOrEmpty(id)) { col1 = "tablename"; col2 = "description"; col3 = ""; };
            var f = new FileInfo($"{Folders.App_Data}\\{tablename}.xml");
            var s = new List<string>();
            int i = 0;
            var data = new DataTable(id);
            if (f.Exists) { data.ReadXml(f.FullName); }
            var db = local.getDataObject();
            try
            {
                if (Request[col1] != null)
                {
                    data = new DataTable(tablename);
                    if (string.IsNullOrEmpty(id))
                    {
                        /* Diễn giải tên Table(Description="") */
                        var tab = Request.Form.GetValues(col1).ToList();
                        var des = Request.Form.GetValues(col2).ToList();
                        data.Columns.Add(col1); data.Columns.Add(col2);
                        for (i = 0; i < tab.Count; i++)
                        {
                            DataRow r = data.NewRow();
                            r[col1] = tab[i].Trim();
                            r[col2] = des[i].Trim();
                            data.Rows.Add(r);
                        }
                    }
                    else
                    {
                        /* Diễn giải tên Colum(Name="", Type="", Description="") */
                        var col = Request.Form.GetValues(col1).ToList();
                        var type = Request.Form.GetValues(col2).ToList();
                        var des = Request.Form.GetValues(col3).ToList();
                        data.Columns.Add(col1); data.Columns.Add(col2); data.Columns.Add(col3);
                        for (i = 0; i < col.Count; i++)
                        {
                            DataRow r = data.NewRow();
                            r[col1] = col[i].Trim();
                            r[col2] = type[i].Trim();
                            r[col3] = des[i].Trim();
                            data.Rows.Add(r);
                        }
                    }
                    if (f.Exists) { f.Delete(); }
                    data.WriteXml(f.FullName, XmlWriteMode.WriteSchema);
                    string redirect = Request["redirect"] == null ? "" : Request["redirect"].Trim();
                    if (string.IsNullOrEmpty(redirect)) { ViewBag.Message = "Cập nhật thành công"; }
                    else
                    {
                        Session.saveMessage(messageKey.actionSuccess);
                        local.setHistory();
                        return Redirect(redirect);
                    }
                }
                i = 0;
                s.Add("<table class=\"table table-bordered table-hover\">");
                if (string.IsNullOrEmpty(id))
                {
                    /* Diễn giải tên Table(Description="") */
                    s.Add("<thead><tr><th>Tên bảng</th><th>Diễn giải</th></tr></thead>");
                    s.Add("<tbody>");
                    foreach (var v in db.getAllTables(true).OrderBy(p=>p).ToList()) { s.Add($"<tr><td> <a href=\"{Url.Action("TableDescription", "Config", new { id = v })}\">{v}</a> <input type=\"hidden\" name=\"{col1}\" value=\"{v}\" readonly=\"readonly\" /> </td><td> <input class=\"form-control\" type=\"text\" name=\"{col2}\" value=\"{data.getValue(col1, v, col2)}\" /> </td></tr>"); }
                }
                else
                {
                    /* Diễn giải tên Column(Type="",Description="") */
                    var listColumn = db.getColumns(id);
                    s.Add($"<caption style=\"font-weight: bold; text-align: center;\">TÊN BẢNG: {id}</caption>");
                    s.Add("<thead><tr><th>Tên Cột</th><th style=\"width: 180px;\">Kiểu</th><th>Diễn giải</th></tr></thead>");
                    s.Add("<tbody>");
                    foreach (DataColumn c in listColumn) { s.Add($"<tr><td> <input class=\"form-control\" type=\"text\" name=\"{col1}\" value=\"{c.ColumnName}\" readonly=\"readonly\" /> </td><td> <input class=\"form-control\" type=\"text\" name=\"{col2}\" value=\"{c.DataType}\" readonly=\"readonly\" /> </td><td> <input id=\"des{i}\" class=\"form-control\" type=\"text\" name=\"{col3}\" value=\"{Server.HtmlEncode(data.getValue(col1, c.ColumnName, col3))}\" /> </td></tr>"); }
                }
                s.Add("</tbody>");
                s.Add("</table>");
            }
            catch (Exception ex) { ViewBag.Error = ex.getLineHTML(a: System.Reflection.Assembly.GetExecutingAssembly()); }
            db.Dispose();
            ViewBag.table = string.Join("", s);
            return View();
        }
    }
}