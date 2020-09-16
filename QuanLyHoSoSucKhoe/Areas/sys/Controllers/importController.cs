using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using zModules;
using zModules.AspNet;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Areas.sys.Controllers
{
    public class importController : Controller
    {
        public string actionName = "importExcel";

        // GET: hethong/importtst

        [Properties(Name = "Nhập Excel", Note = "fa fa-file")]
        public ActionResult index()
        {
            string foldertmp = Folders.temp + "\\test";
            if (Directory.Exists(foldertmp) == false) { Directory.CreateDirectory(foldertmp); }
            try
            {
                if (Request.Files.Count > 0)
                {
                    string foldersave = Folders.temp + "\\test"; 
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var filenName = foldersave + "\\" + Request.Files[i].FileName.xoaDauTV();
                        var f = new FileInfo(filenName);
                        if (f.Exists) { f.Delete(); }
                        Request.Files[i].SaveAs(filenName);
                        return Redirect("/sys/import/viewxls?file=" + HttpUtility.UrlEncode(filenName.Replace(Folders.pathApp, "")));
                    }
                    ViewBag.Message = messageKey.actionSuccess;
                }
            }
            catch (Exception ex) { ViewBag.Error = ex.getLineHTML(); }
            return View();
        }
        /// <summary>
        /// ID: Mã bảo hiểm cấp huyện; f: Tên tập tin xử lý; Mode = 1: Xóa tập tin, ngược lại cập nhật danh sách tst; listfile: danh sách tập tin cần xử lý
        /// </summary>
        /// <returns></returns>
        public ActionResult import()
        { 
            string folder = Folders.temp + "\\test";
            string filename = "";
            /* Check Exits File, Kiểm tra tham số */
            try
            {
                if (Request.getValue("mode") == "1")
                {
                    /* Xóa tập tin */
                    if (Request["f"] != null)
                    {
                        var f = new FileInfo(folder + "\\" + Request.getValue("f"));
                        if (f.Exists) { f.Delete(); }
                    }
                    else
                    {
                        foreach (var v in Request.Form.GetValues("listfile"))
                        {
                            var f = new FileInfo(folder + "\\" + v);
                            if (f.Exists) { f.Delete(); }
                        }
                    }
                    Session[keyS.Message] = messageKey.actionSuccess;
                    return RedirectToAction("index");
                }
                string id = Request.getValue("f");
                if (string.IsNullOrEmpty(id) == false)
                {
                    var f = new FileInfo(folder + "\\" + id);
                    if (f.Exists == false) { throw new Exception(messageKey.notFoundFile(f.FullName)); }
                    filename = f.FullName;
                }
                else { if (Request["listfile"] == null) { throw new Exception(messageKey.thamSoKhongDung); } }
            }
            catch (Exception ex)
            {
                Session[keyS.Error] = ex.saveMessage();
                return RedirectToAction("index");
            }
            /* */
            try
            {
                var args = new itemScheduler
                {
                    function = keyFunction.importExcel,
                    idObject = "",
                    idUser = Session.getIdUser(),
                    action = actionName,
                    packetSize = 10000,
                    listFile = filename,
                    typeProcess = "",
                    times = DateTime.Now,
                    request = Request.toRequestString()
                };
                if (string.IsNullOrEmpty(filename))
                {
                    /* Excel */
                    var ls = new List<string>();
                    var vals = Request.Form.GetValues("listfile");
                    foreach (var v in vals) { ls.Add(folder + "\\" + v); }
                    args.listFile = string.Join(",", ls);
                }
                else { throw new Exception("Chưa chọn tập tin import"); } 
                Session[keyS.Message] = args.callTaskScheduler();
            }
            catch (Exception ex) { Session.saveError(ex.saveMessage()); }
            return RedirectToAction("index");
        }

        public ActionResult viewxls()
        {
            ViewBag.listsheet = new List<string>();
            var file = Request.getValue("file");
            ViewBag.file = file;
            if (string.IsNullOrEmpty(file)) { ViewBag.Error = messageKey.thamSoKhongDung; return View(); }
            file = Folders.pathApp + file;
            if (System.IO.File.Exists(file) == false) { ViewBag.Error = $"Tập tin '{file}' không tồn tại trên hệ thống"; return View(); }
            ViewBag.listsheet = zModules.NPOIExcel.XLS.getSheetNames(file);
            return View();
        }
        public ActionResult getdatasheet()
        { 
            var file = Request.getValue("file"); 
            if (string.IsNullOrEmpty(file)) { return Content($"<div class=\"alert alert-danger\">{messageKey.thamSoKhongDung}</div>"); }
            file = Folders.pathApp + file;
            var tmp = Request.getValue("sheetindex");
            if(Regex.IsMatch(tmp, "^[0-9]+$") == false) { return Content($"<div class=\"alert alert-danger\">{messageKey.thamSoKhongDung}</div>"); }
            var sheetIndex = int.Parse(tmp);
            tmp = Request.getValue("maxrow");
            var maxrow = Regex.IsMatch(tmp, "^[0-9]+$") == false ? 50 : int.Parse(tmp);
            ViewBag.data = zModules.NPOIExcel.XLS.getDataFromExcel97_2003(new FileInfo(file), sheetIndex: sheetIndex, maxRow: maxrow);
            return View(); 
        }
        public ActionResult getfiles()
        {
            string foldertmp = Folders.temp + "\\test";
            /* View Files */
            var s = new List<string>();
            foreach (var v in Directory.GetFiles(foldertmp, "*.xls*"))
            {
                var f = new FileInfo(v);
                s.Add($"<li> [ <input type=\"checkbox\" name=\"listfile\" value=\"{f.Name}\" /> ] <i class=\"fa fa-file\"></i> {f.Name} ");
                s.Add($" <a href=\"/sys/import/viewxls?file={HttpUtility.UrlEncode(f.FullName.Replace(Folders.pathApp, ""))}\"> <i class=\"fa fa-eye\"></i> </a> ");
                s.Add($"(<i>{f.LastWriteTime:dd/MM/yyyy HH:mm}</i>)</li>");
            }
            if (s.Count > 0) { ViewBag.listxls = string.Join("", s); }
            return View();
        }

        public ActionResult info()
        {
            var iduser = Session.getIdUser();
            using (var db = local.getDataObject())
            {
                try
                {
                    
                    var where = $"where [iduser]=N'{iduser.getValueField()}' and [action]=N'importExcel'";
                    var dt = db.getDataSet($"select top 1 * from [{keyTable.importData}2] {where}").Tables[0];
                    if(dt.Rows.Count == 0) { return View(); }
                    var cols = new List<string>();
                    foreach (DataColumn c in dt.Columns)
                    {
                        if(dt.Rows[0][c.ColumnName] == DBNull.Value) { break; }
                        if(c.ColumnName == "iduser" || c.ColumnName == "action") { continue; }
                        cols.Add($"[{c.ColumnName}]");
                    }
                    ViewBag.data = db.getDataSet($"select top 100 {string.Join(",", cols)} from [{keyTable.importData}2] {where}").Tables[0];
                    return View();
                }
                catch (Exception ex) { return Content($"<b>Lỗi: </b>{ex.saveMessage()}"); }
            }
        }
    }
}