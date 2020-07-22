using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using zModules;
using zModules.AspNet;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Areas.sys.Controllers
{
    public class dataController : Controller
    {

        [Properties(Name = "Dữ liệu", Note = "fa fa-database")]
        public ActionResult Index()
        {
            using (var db = local.getDataObject())
            {
                try
                {
                    ViewBag.dmtinh = db.getDataSet("select * from dmtinh").Tables[0];
                }
                catch (Exception ex) { ViewBag.Error = ex.Message; }
            }
            return View();
        }

        public ActionResult getfilebak()
        {
            var s = new List<string>();
            ViewBag.ListFile = new List<string>();
            var d = new DirectoryInfo(Folders.Backup);
            if (!d.Exists) { d.Create(); }
            foreach (var f in d.GetFiles()) { s.Add(f.Name); }
            ViewBag.ListFile = s;
            return View();
        }

        [zModules.Permission(Delete = true)]
        public ActionResult Delete()
        {
            string name = Request["v"] == null ? "" : Request["v"].Trim();
            if (string.IsNullOrEmpty(name))
            {
                Session[keyS.Error] = "Chưa chọn tập tin cần xóa";
                return RedirectToAction("Index");
            }
            try
            {
                var f = new FileInfo($"{Folders.Backup}\\{name}");
                f.Delete();
            }
            catch (Exception ex) { Session[keyS.Error] = ex.getLineHTML(); return RedirectToAction("Index"); }
            Session[keyS.Message] = $"Xóa tập tin '{name}' thành công";
            local.setHistory();
            return RedirectToAction("Index");
        }

        [zModules.Permission(AddNew = true)]
        public ActionResult Backup()
        {
            string iduser = Session.getIdUser();
            string name = $"db_{DateTime.Now:ddMMyy_HHmmss}.bak";
            var tsql = new List<string> { local.getTsqlInsertHistory("Backup Data", iduser, "Backup Data") };
            using (var db = local.getDataObject()) { tsql.Add($"BACKUP DATABASE [{db.Connection.Database}] TO  DISK = N'{Folders.Backup}\\{name}' WITH NOFORMAT, NOINIT,  NAME = N'Full Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10 "); }
            var arg = new itemScheduler("Backup Data", iduser, ListFile: string.Join("\ngo\n", tsql), iRequest: Request.toRequestString(), iData: tsql);
            return Content(arg.callTaskScheduler());
        }

        [zModules.Permission(AddNew = true)]
        public ActionResult shrink()
        {
            string iduser = Session.getIdUser();
            var tsql = new List<string> { local.getTsqlInsertHistory("shrink Data", iduser, "shrink Data") };
            using (var db = local.getDataObject()) { tsql.Add($"DBCC SHRINKDATABASE ([{db.Connection.Database}], TRUNCATEONLY)"); }
            var arg = new itemScheduler("shrink Data", iduser, ListFile: string.Join("\ngo\n", tsql), iRequest: Request.toRequestString(), iData: tsql);
            return Content(arg.callTaskScheduler()); 
        }

        public ActionResult Restore()
        {
            string iduser = Session.getIdUser();
            Session[keyS.Message] = Session.callThread("Restore Data", Request.toRequestString(), obj: (Action)(() =>
            {
                var db = local.getDataObject();
                string name_old = $"{Folders.App_Data}\\db_{DateTime.Now:yyMMddHHmmss}.db3";
                string rqname = Request["f"] == null ? "" : Request["f"].Trim();
                try
                {
                    if (rqname == "") throw new Exception("Không tìm thấy tập tin phục hồi ");
                    var f = new FileInfo($"{Folders.Backup}\\{rqname}");
                    if (!f.Exists) throw new Exception($"Không tìm thấy tập tin phục hồi '{rqname}' ");
                    /* Lấy đường dẫn tập tin dữ liệu */
                    System.Threading.Thread.Sleep(5000);
                    db.restore(f.FullName);
                    local.setHistory("Restore Data", iduser, "Restore Data");
                    /* Đóng kết nối */
                    db.Dispose();
                }
                catch (Exception ex) { db.Dispose(); ex.save(); }
            }));
            return RedirectToAction("Index");
        }

        [zModules.Permission(Upload = true)]
        public ActionResult UpFile()
        {
            if (Request.Files.Count == 0)
            {
                Session[keyS.Error] = "Chưa chọn tập tin";
                return RedirectToAction("Index");
            }
            try
            {
                var f = new FileInfo($"{Folders.Backup}\\{Request.Files[0].FileName}");
                if (f.Exists)
                {
                    string name = $"t{DateTime.Now:ddMMyyHHmmss}_{Request.Files[0].FileName}";
                    Request.Files[0].SaveAs($"{Folders.Backup}\\{name}");
                }
                else { Request.Files[0].SaveAs(f.FullName); }
                Session.saveMessage($"Tải tập tin '{Request.Files[0].FileName}' liên hệ thống thành công");
                local.setHistory();
            }
            catch (Exception ex) { Session.saveError(ex); }
            return RedirectToAction("Index");
        }

        public ActionResult info()
        {
            var db = local.getDataObject();
            ViewBag.Connection = db.Connection;
            ViewBag.dataSize = db.DatabaseSize();
            ViewBag.ramInstall = db.RAMInstallSize(true);
            ViewBag.ramAvabile = db.RamAvailableSize(true);
            return View();
        }

        [HttpPost]
        public ActionResult runsql()
        {
            var time = DateTime.Now;
            var tsql = Request.getValue("tsql");
            var mode = Request.getValue("mode");
            try
            {
                while (true)
                {
                    if (mode != "1") { throw new Exception("Mode Not Found"); }
                    if (string.IsNullOrEmpty(tsql)) { throw new Exception("TSQL Not Found"); }
                    break;
                }
            }
            catch (Exception ex) { return Content(ex.Message.BootstrapAlter(KeyBootstrapAlerts.Danger)); }
            using (var db = local.getDataObject())
            {
                try
                {
                    db.Execute(tsql);
                    var t = DateTime.Now - time;
                    return Content(($"{messageKey.actionSuccess} {t.Hours:00}:{t.Minutes:00}:{t.Seconds:00}").BootstrapAlter());
                }
                catch (Exception ex) { return Content(($"{ex.Message}").BootstrapAlter(KeyBootstrapAlerts.Danger)); }
            }
        }

        [HttpPost]
        public ActionResult createdata()
        {
            if (Request.getValue("mode") != "1") { return Content(messageKey.thamSoKhongDung.BootstrapAlter(KeyBootstrapAlerts.Danger)); }
            var s = buildDatabase.createDatabase();
            if (string.IsNullOrEmpty(s)) { return Content("Cập nhật cấu trúc dữ liệu thành công".BootstrapAlter()); }
            return Content(s.BootstrapAlter(KeyBootstrapAlerts.Danger));
        }

        [HttpPost]
        public ActionResult export()
        {
            var tsql = Request.getValue("tsql");
            using (var db = local.getDataObject())
            {
                try
                {
                    var dt = db.getDataSet(tsql);
                    if (dt.Tables.Count == 0) { return Content("Không có dữ liệu"); }
                    using (var ms = new System.IO.MemoryStream())
                    {
                        dt.WriteXml(ms, System.Data.XmlWriteMode.WriteSchema);
                        ms.Position = 0;
                        return File(ms.ToArray(), "application/octet-stream", "tsql.xml");
                    }
                }
                catch (Exception ex) { return Content(ex.Message); }
            }
        }

        [HttpPost]
        public ActionResult export7z()
        {
            var d = new System.IO.DirectoryInfo($"{Folders.temp}\\tsql");
            try
            {
                if (d.Exists == false) { d.Create(); }
                foreach (var v in d.GetFiles()) { v.Delete(); }
            }
            catch (Exception ex) { return Content($"{ex.Message}"); }
            var tsql = Request.getValue("tsql");
            using (var db = local.getDataObject())
            {
                try
                {
                    var ds = db.getDataSet(tsql);
                    if (ds.Tables.Count == 0) { return Content("Không có dữ liệu"); }
                    ds.WriteXml($"{d.FullName}\\tsql.xml", System.Data.XmlWriteMode.WriteSchema);
                    var c = new zModules.Compress($"{Folders.pathApp}\\7z.exe");
                    c.add($"{d.FullName}\\tsql.7z", d.FullName);
                    return File($"{d.FullName}\\tsql.7z", "application/octet-stream", "tsql.7z");
                }
                catch (Exception ex) { return Content(ex.Message); }
            }
        }
    }
}