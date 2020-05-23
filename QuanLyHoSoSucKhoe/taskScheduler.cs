using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using zModules;
using zModules.MSSQLServer;
using zModules.NPOIExcel;

namespace QuanLyHoSoSucKhoe
{
    public static class taskScheduler
    {
        public static string callTaskScheduler(this itemScheduler args)
        {
            bool waitrun = false;
            /* Khung giờ chỉ dành cho đồng bộ dữ liệu vào cập nhật phiên bản */
            if (local.taskList[args.action] != null)
            {
                if (args.function == keyFunction.Other) { return $"Tiến trình {args.action} đang chạy. Không thể thao tác. Vui lòng chờ tiến trình kết thúc"; }
                waitrun = true;
            }
            else
            {
                if (local.taskList.Count() > 0)
                {
                    var obj = new List<keyFunction>() { keyFunction.importCV93, keyFunction.ExecTSQL, keyFunction.importExcel, keyFunction.importTGLaoDong };
                    if (obj.Contains(args.function)) { waitrun = true; }
                }
            }
            if (waitrun)
            {
                /* phân tích args cho phép thêm nhiều, lưu lại xml ghi nhớ */
                var path = new FileInfo(Folders.App_Data + "\\taskscheduler.xml");
                var dt = new DataTable("taskscheduler");
                try { dt.ReadXml(path.FullName); }
                catch { }
                if (dt.Rows.Count == 0)
                {
                    var cols = "iduser,action,function,typeprocess,idObject,packetsize,listfile,request".Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    dt = new DataTable("taskscheduler");
                    foreach (var v in cols) { dt.Columns.Add(v); }
                    dt.Columns.Add("times", typeof(DateTime));
                }
                //var dv = new DataView(dt, $"iduser='{args.idUser}' and action='{args.action}'", "action", DataViewRowState.CurrentRows);
                dt.Rows.Add(args.idUser, args.action, $"{args.function}", args.typeProcess, args.idObject, args.packetSize, args.listFile, args.request.Replace(",", ", "), args.times);
                dt.WriteXml(path.FullName, XmlWriteMode.WriteSchema);
                return $"Đã thêm hàng đợi tiến trình xử lý. <a href=\"/task\" alt=\"Tiến trình\">Chuyển sang mục tiến trình để biết thông tin chi tiết</a>";
            }
            Thread t;
            try
            {
                if (args.function == keyFunction.Other) { t = new Thread(new ThreadStart(() => actionOther(args))); }
                else
                {
                    var mi = (typeof(taskScheduler)).GetMethod($"{args.function}");
                    t = new Thread(new ThreadStart(() => mi.Invoke(null, new object[] { args })));
                }
                t.Start();
                return "Đã thêm vào tiến trình xử lý. <a href=\"/task\" alt=\"Tiến trình\">Chuyển sang mục tiến trình để theo dõi quá trình thao tác</a>";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public static void reCallTaskScheduler()
        {
            var dt = getListTaskSchedulerWait();
            if (dt.Rows.Count == 0) { return; }
            try
            {
                DataRow r = dt.Rows[0];
                var key = Enum.Parse(typeof(keyFunction), $"{r["function"]}", true);
                var args = new itemScheduler();
                /*cols = "iduser,action,function,typeprocess,idObject,packetsize,listfile,request,times";*/
                args.action = $"{r["action"]}";
                args.function = (keyFunction)key;
                if (args.function == keyFunction.Other)
                {
                    dt.Rows.RemoveAt(0);
                    dt.WriteXml(Folders.App_Data + "\\taskscheduler.xml", XmlWriteMode.WriteSchema);
                    reCallTaskScheduler();
                    return;
                }
                args.listFile = $"{r["listfile"]}";
                if (args.function == keyFunction.ExecTSQL)
                {
                    /* Format ListFile TSQL: NewLine + Go + NewLine */
                    if (string.IsNullOrEmpty(args.listFile))
                    {
                        dt.Rows.RemoveAt(0);
                        dt.WriteXml(Folders.App_Data + "\\taskscheduler.xml", XmlWriteMode.WriteSchema);
                        reCallTaskScheduler();
                        return;
                    }
                    var s = Regex.Matches(args.listFile, "[\n]go[\n]");
                    var a = new List<string>();
                    for (int i = 0; i < s.Count; i++) { a.Add(s[i].Value); }
                    args.data = a;
                }
                args.idObject = $"{r["idObject"]}";
                args.idUser = $"{r["iduser"]}";
                args.packetSize = int.Parse($"{r["packetsize"]}");
                args.request = $"{r["request"]}";
                args.times = DateTime.Parse($"{r["times"]}");
                args.typeProcess = $"{r["typeprocess"]}";
                args.callTaskScheduler();
                dt.Rows.RemoveAt(0);
                dt.WriteXml(Folders.App_Data + "\\taskscheduler.xml", XmlWriteMode.WriteSchema);
            }
            catch (Exception ex) { ex.save(); }
        }

        public static DataTable getListTaskSchedulerWait()
        {
            var path = new FileInfo(Folders.App_Data + "\\taskscheduler.xml");
            if (path.Exists == false) { return new DataTable(); }
            try
            {
                var dt = new DataTable("taskscheduler");
                dt.ReadXml(path.FullName);
                return dt;
            }
            catch (Exception ex) { ex.save(); return new DataTable(); }
        }

        public static void removeTaskScheduleWait(string iduser, string actionName, string time)
        {
            var dt = getListTaskSchedulerWait();
            if (dt.Rows.Count == 0) { return; }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (iduser != $"{dt.Rows[i]["iduser"]}") { continue; }
                if (actionName != $"{dt.Rows[i]["action"]}") { continue; }
                if (time != $"{dt.Rows[i]["times"]:dd/MM/yyyy HH:mm}") { continue; }
                dt.Rows.RemoveAt(i);
                dt.WriteXml(Folders.App_Data + "\\taskscheduler.xml", XmlWriteMode.WriteSchema);
                return;
            }
        }

        public static string callThread(this HttpSessionStateBase s, string action, string request = "", string list = "", int packetsize = 1000, Action obj = null)
        {
            var args = new itemScheduler();
            args.idUser = s.getIdUser();
            args.action = action;
            args.packetSize = 1000;
            args.listFile = list;
            args.times = DateTime.Now;
            args.request = request;
            args.data = obj;
            return args.callTaskScheduler();
        }

        private static void actionOther(itemScheduler args)
        {
            if (args.data is Action)
            {
                Progressbar p = new Progressbar(args.action, args.action);
                p.message = $"{args.idUser}: processing ..";
                local.taskList.Add(p);
                var obj = args.data as Action;
                try { obj(); } catch (Exception ex) { ex.save(); }
                local.taskList.Remove(p);
            }
        }

        public static void ExecTSQL(itemScheduler args)
        {
            if (args.data is List<string>)
            {
                Progressbar p = new Progressbar(args.action, args.action);
                p.message = $"{args.idUser}: processing ..";
                local.taskList.Add(p);
                var obj = args.data as List<string>;
                using (var db = local.getDataObject())
                {
                    try
                    {
                        foreach (var v in obj) { db.Execute(v); }
                    }
                    catch (Exception ex) { ex.save(); }
                }
                local.taskList.Remove(p);
                reCallTaskScheduler();
            }
        }

        public static void importExcel(itemScheduler args)
        {
            args.times = DateTime.Now;
            var p = new Progressbar(args.action) { message = "Kiểm tra dữ liệu đầu vào" };
            int rowProcessed = 0;
            bool flagContinue = true;
            var db = local.getDataObject();
            var actions = new List<Action>();
            var msgError = new List<string>();
            var cols = new List<string>() { "iduser", "action" };
            string wImport = $"iduser=N'{args.idUser.getValueField()}' and [action]=N'{args.action}'";
            string tImport = keyTable.importData + "2";
            for (int index = 0; index < 57; index++) { cols.Add($"f{index}"); }
            var dt = new DataTable(tImport); foreach (var v in cols) { dt.Columns.Add(v); }
            /* Excel */
            p.name = $"{args.idUser} Import Excel";
            local.taskList.Add(p);
            actions.Add(() =>
            {
                local.taskList.Modify(args.action, "Reset Import Excel ..");
                db.Execute($"delete from {tImport} where {wImport};");
            });
            char separator = ','; if (args.listFile.Contains("|")) { separator = '|'; }
            foreach (var filename in args.listFile.Split(separator))
            {
                actions.Add(() =>
                {
                    var f = new FileInfo(filename);
                    local.taskList.Modify(args.action, $"{f.Name}: {rowProcessed:#,0} processed ..");
                    var xls = new OleDbConnection(excel.getConnectionString(filename, false));
                    try
                    {
                        var r = xls.ExcelReader();
                        dt = new DataTable(tImport); foreach (var v in cols) { dt.Columns.Add(v); }
                        string val = "";
                        while (r.Read())
                        {
                            if (dt.Rows.Count >= args.packetSize)
                            {
                                try
                                {
                                    dt.bulkCopy(local.connectionstring, tImport);
                                    rowProcessed += dt.Rows.Count;
                                    local.taskList.Modify(args.action, $"{f.Name}: {rowProcessed:#,0} processed ..");
                                }
                                catch (Exception ex)
                                {
                                    var s = ex.saveMessage();
                                    if (s.Contains("Column 'madvi'") == false) { msgError.Add($"{DateTime.Now:HH:mm:ss}-{f.Name}: " + s); }
                                    local.taskList.Modify(args.action, $"{s}");
                                }
                                dt = new DataTable(tImport); foreach (var v in cols) { dt.Columns.Add(v); }
                            }
                            DataRow row = dt.NewRow();
                            row[0] = args.idUser;
                            row[1] = args.action;
                            for (int index = 0; index < (r.FieldCount >= dt.Columns.Count ? dt.Columns.Count : r.FieldCount); index++)
                            {
                                val = $"{r[index]}".Trim(); if (val.Length > 500) { val = val.Substring(0, 499).Trim(); }
                                row[index + 2] = val;
                            }
                            dt.Rows.Add(row);
                        }
                        if (dt.Rows.Count > 0)
                        {
                            try
                            {
                                dt.bulkCopy(local.connectionstring, tImport);
                                rowProcessed += dt.Rows.Count;
                                local.taskList.Modify(args.action, $"{f.Name}: {rowProcessed:#,0} processed ..");
                            }
                            catch (Exception ex)
                            {
                                var s = ex.saveMessage();
                                msgError.Add($"{DateTime.Now:HH:mm:ss}-{filename.Substring(Folders.pathApp.Length)}: " + s);
                                local.taskList.Modify(args.action, $"{s}");
                            }
                            dt = new DataTable(tImport);
                        }
                        r.Close();
                    }
                    catch (Exception ex)
                    {
                        msgError.Add($"{DateTime.Now:HH:mm:ss}-{filename.Substring(Folders.pathApp.Length)}: " + ex.saveMessage());
                        local.taskList.Modify(args.action, $"{msgError[msgError.Count - 1]}");
                    }
                    xls.Dispose();
                });
            }
            if (actions.Count > 0)
            {
                int index = 0;
                local.taskList.setValueMax(args.action, actions.Count);
                /* Lưu quá trình tiến trình */
                local.setHistoryTaskList(local.taskList[args.action], $"{args.idUser}", requeststring: args.request);
                foreach (var action in actions)
                {
                    try
                    {
                        action();
                        if (flagContinue == false) { break; }
                        index++;
                        local.taskList.Modify(args.action, "", index);
                    }
                    catch (Exception ex)
                    {
                        msgError.Add(ex.saveMessage($"Index: {index} - {local.taskList[args.action].message}"));
                    }
                }
                /* Lưu kết quả tiến trình */
                var t = DateTime.Now - p.timeStart;
                var rs = $"Time Processed: {t.Hours}:{t.Minutes}:{t.Seconds}; {string.Join("; ", msgError)}";
                local.setHistoryTaskList(local.taskList[args.action], $"{args.idUser}", rs);
            }
            db.Dispose();
            local.taskList.Remove(p);
            reCallTaskScheduler();
        }

        private static void bulkCopyTableFromAccess(this Models.SQLServerDataContext db, string fileName, string tableName, itemScheduler args, ref int countRow, ref List<string> msgError, string where = "", string fromTable = "")
        {
            var cols = new List<string>();
            var dt = db.getDataSet($"select top 1 * from {tableName}").Tables[0];
            foreach (DataColumn c in dt.Columns) { cols.Add(c.ColumnName); }
            string col = $"[{string.Join("],[", cols)}]";
            var f = new FileInfo(fileName);
            var acc = new access(f.FullName, "");
            local.taskList.Modify(args.action, $"{f.Name}-{tableName}: {countRow:#,0} processed ..");
            if (string.IsNullOrEmpty(fromTable)) { fromTable = tableName; }
            string tsql = $"select {col} from {fromTable}{(string.IsNullOrEmpty(where) ? "" : " where " + where)};";
            var r = acc.reader(tsql);
            try
            {
                if (r.HasRows == false) { r.Close(); acc.close(); return; }
                dt = new DataTable(tableName);
                foreach (var item in cols) { dt.Columns.Add(item); }
                local.taskList.Modify(args.action, $"{f.Name}-{tableName}: {countRow:#,0} processed ..");
                while (r.Read())
                {
                    if (dt.Rows.Count >= args.packetSize)
                    {
                        dt.bulkCopy(local.connectionstring, tableName);
                        countRow += dt.Rows.Count;
                        dt = new DataTable(tableName);
                        foreach (var item in cols) { dt.Columns.Add(item); }
                        local.taskList.Modify(args.action, $"{f.Name}-{tableName}: {countRow:#,0} processed ..");
                    }
                    DataRow row = dt.NewRow();
                    foreach (var item in cols) { row[item] = $"{r[item]}"; }
                    dt.Rows.Add(row);
                }
                if (dt.Rows.Count > 0)
                {
                    dt.bulkCopy(local.connectionstring, tableName);
                    countRow += dt.Rows.Count;
                    dt = new DataTable(tableName);
                    local.taskList.Modify(args.action, $"{f.Name}-{tableName}: {countRow:#,0} processed ..");
                }
                r.Close();
                acc.close();
            }
            catch (Exception ex)
            {
                try { r.Close(); } catch { }
                acc.close();
                msgError.Add($"{DateTime.Now:HH:mm:ss}-{f.Name}-{tableName}: " + ex.saveMessage());
                local.taskList.Modify(args.action, $"{msgError[msgError.Count - 1]}");
            }
        }
    }

    /// <summary>
    /// keyFunction: Tên hàm hỗ trợ chạy lần tiếp nếu nó đang chạy rồi
    /// </summary>
    public enum keyFunction { Other = 0, ExecTSQL = 1, importCV93 = 3, importExcel = 4, importTGLaoDong = 5, importTTCNTT = 6, importDieuTraDanSo = 7 }

    public class itemScheduler
    {
        public itemScheduler()
        {
        }

        public itemScheduler(string ActionName, string IdUser = "", keyFunction FunctionName = keyFunction.ExecTSQL, string TypeProcess = "", string IdObject = "", string ListFile = "", string iRequest = "", object iData = null, int PacketSize = 1000)
        {
            action = ActionName;
            idUser = IdUser;
            function = FunctionName;
            typeProcess = TypeProcess;
            idObject = IdObject;
            packetSize = PacketSize;
            listFile = ListFile;
            request = iRequest;
            data = iData;
            times = DateTime.Now;
        }

        /// <summary>
        /// Mã tài khoản
        /// </summary>
        public string idUser { get; set; } = "";

        /// <summary>
        /// Mã
        /// </summary>
        public string action { get; set; } = "";

        /// <summary>
        /// Tên thủ tục xử lý (Function)
        /// </summary>
        public keyFunction function { get; set; } = keyFunction.Other;

        /// <summary>
        /// Kiểu xử lý dữ liệu: Excel, Access, Sqlite, MssqlServer
        /// </summary>
        public string typeProcess { get; set; } = "";

        /// <summary>
        /// Mã đối tượng: ví dụ mã huyện, mã Bảo hiểm Cấp huyện
        /// </summary>
        public string idObject { get; set; } = "";

        /// <summary>
        /// Số bản ghi xử lý trên 1 lần xử lý
        /// </summary>
        public int packetSize { get; set; } = 1000;

        /// <summary>
        /// Danh sách tập tin cần xử lý
        /// </summary>
        public string listFile { get; set; } = "";

        /// <summary>
        /// Request => Mục đích lưu lại lịch sử xử lý
        /// </summary>
        public string request { get; set; } = "";

        public object data { get; set; }
        public DateTime times { get; set; } = DateTime.Now;
    }
}