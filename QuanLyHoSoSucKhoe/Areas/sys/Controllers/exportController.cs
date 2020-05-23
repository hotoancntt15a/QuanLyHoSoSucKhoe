using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using zModules;
using zModules.AspNet;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Areas.sys.Controllers
{
    public class exportController : Controller
    {
        public string actionName = "export.accdb";
        private string pathsource = $"{Folders.App_Data}\\db.accdb";
        private string pathExport = $"{Folders.temp}\\export";

        protected override void HandleUnknownAction(string actionName) => this.HttpContext.HandleUnknownAction(actionName);

        // GET: nhapxuat/export
        [CheckLogin]
        [Properties(Name = "Xuất dữ liệu", Note = "fa fa-file")]
        public ActionResult Index()
        {
            if (Directory.Exists(Folders.temp) == false) { Directory.CreateDirectory(Folders.temp); }
            if (Directory.Exists(pathExport) == false) { Directory.CreateDirectory(pathExport); }
            var lsfile = new List<FileInfo>();
            foreach (var filename in Directory.GetFiles(pathExport, "*"))
            {
                var f = new FileInfo(filename);
                lsfile.Add(f);
            }
            ViewBag.listfile = lsfile;
            var s = new List<string>();
            var db = local.getDataObject();
            try { s = db.getAllTables(); }
            catch (Exception ex) { ViewBag.Error = ex.saveMessage(); }
            ViewBag.listtable = s;
            db.Dispose();
            return View();
        }

        [CheckLogin]
        [Properties(Name = "Xuất Access")]
        public ActionResult accdb()
        {
            var args = new itemScheduler();
            args.action = actionName;
            args.packetSize = int.Parse(Request.getValue("packetsize"));
            args.request = Request.toRequestString();
            args.listFile = Request.getValue("table");
            args.idUser = Session.getIdUser();
            args.data = (Action)(() => bwDoWork(args));
            Session[keyS.Message] = args.callTaskScheduler();
            return RedirectToAction("index");
        }

        [CheckLogin]
        [Properties(Name = "Xóa file xuất")]
        public ActionResult delaccdb()
        {
            string filename = Request.getValue("f");
            if (string.IsNullOrEmpty(filename)) { Session.saveError(messageKey.thamSoKhongDung); }
            else
            {
                try
                {
                    var f = new FileInfo(pathExport + "\\" + filename);
                    if (f.Exists) { f.Delete(); }
                    local.setHistory();
                }
                catch (Exception ex) { Session.saveError(ex.Message); }
            }
            return RedirectToAction("index");
        }

        public void bwDoWork(itemScheduler args)
        {
            Progressbar p = new Progressbar(actionName);
            p.name = "Xuất dữ liệu";
            if (local.taskList[actionName] == null) { local.taskList.Add(p); }
            local.taskList.Modify(actionName, $"{args.idUser} Kiểm tra dữ liệu ..");
            var db = local.getDataObject();
            var v = new List<string>();
            var msgError = new List<string>();
            var actions = new List<Action>();
            string pathaccdb = "";
            if (args.typeProcess == "")
            {
                /* Access */
                actions.Add(() =>
                {
                    pathaccdb = Folders.temp;
                    if (Directory.Exists(pathaccdb) == false) { Directory.CreateDirectory(pathaccdb); }
                    if (Directory.Exists(pathExport) == false) { Directory.CreateDirectory(pathExport); }
                    pathaccdb = pathExport + $"\\data_{DateTime.Now:yyyyMMdd_HHmmss}.accdb";
                    var f = new FileInfo(pathsource);
                    f.CopyTo(pathaccdb);
                });
                foreach (var table in args.listFile.Split(','))
                {
                    actions.Add(() =>
                    { 
                        if (table == "") { return; }
                        var values = new List<string>();
                        local.taskList.Modify(actionName, $"{args.idUser} {table}: Xuất dữ liệu ..");
                        int processed = 0;
                        db.toAccess(table, pathaccdb, ref processed);
                    });
                }
            }
            if (actions.Count > 0)
            {
                int index = 0;
                local.taskList.setValueMax(actionName, actions.Count);
                /* Lưu quá trình tiến trình */
                db.setHistoryTaskList(local.taskList[actionName], args.idUser, requeststring: args.request);
                foreach (var action in actions)
                {
                    try { action(); index++; local.taskList.Modify(actionName, "", index); }
                    catch (Exception ex) { msgError.Add(ex.saveMessage($"Index: {index} - {local.taskList[actionName].message}")); }
                }
                /* Lưu kết quả tiến trình */
                var t = DateTime.Now - p.timeStart;
                db.setHistoryTaskList(local.taskList[actionName], args.idUser, $"{t.Hours}:{t.Minutes}:{t.Seconds} {string.Join("; ", msgError)}");
            }
            db.Dispose();
            local.taskList.Remove(p);
        }
    }
}