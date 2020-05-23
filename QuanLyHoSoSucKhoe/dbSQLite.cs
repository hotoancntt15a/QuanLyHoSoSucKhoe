using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;
using zModules.NPOIExcel;

namespace System.Data.SQLite
{
    [SQLiteFunction(Name = "REGEXP", Arguments = 2, FuncType = FunctionType.Scalar)]
    internal class REGEXP : SQLiteFunction
    {
        /// <summary>
        /// REGEXP (FieldName, Expression): regexp(f0, '^\d+$') = 1
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override object Invoke(object[] args)
        {
            return Regex.IsMatch(Convert.ToString(args[0]), Convert.ToString(args[1]));
        }
    }
}

namespace zModules
{
    public class dbSQLite
    {
        public SQLiteConnection Connection = new SQLiteConnection();
        public int commandTimeout = 3600;

        public dbSQLite(string pathfile = "", string password = "")
        {
            if (string.IsNullOrEmpty(pathfile) == false) { return; }
            var cnstrb = new SQLiteConnectionStringBuilder();
            cnstrb.DataSource = pathfile;
            if (string.IsNullOrEmpty(password) == false) { cnstrb.Password = password; }
            Connection = new SQLiteConnection(cnstrb.ConnectionString);
        }

        public void Dispose()
        {
            try
            {
                if (Connection != null)
                {
                    if (Connection.State != ConnectionState.Closed) Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                }
            }
            catch { }
        }

        public void backup(string pathsave) => Connection.backup(pathsave);

        public void Execute(string tsql) => Connection.Execute(tsql, commandTimeout);

        public void Execute(SQLiteCommand sqlcmd) => Connection.Execute(sqlcmd, commandTimeout);

        public object getValue(string tsql) => Connection.getValue(tsql);

        public object getValue(SQLiteCommand sqlcmd)
        {
            if (Connection.State == ConnectionState.Closed) Connection.Open();
            var dt = new DataTable("dt");
            sqlcmd.Connection = Connection;
            var a = new SQLiteDataAdapter(sqlcmd);
            a.Fill(dt);
            a.Dispose();
            if (dt.Rows.Count == 0) return null;
            return dt.Rows[0][0];
        }

        public DataSet getDataSet(string tsql) => Connection.getDataSet(tsql);

        public DataSet getDataSet(SQLiteCommand sqlcmd)
        {
            if (Connection.State == ConnectionState.Closed) Connection.Open();
            sqlcmd.Connection = Connection;
            var ds = new DataSet("dataset");
            var a = new SQLiteDataAdapter(sqlcmd);
            a.Fill(ds);
            a.Dispose();
            return ds;
        }

        public SQLiteDataReader dataReader(string tsql) => Connection.dataReader(tsql, commandTimeout);

        public SQLiteDataReader dataReader(SQLiteCommand cmd) => Connection.dataReader(cmd, commandTimeout);

        public void delete(string table) => Connection.delete(table);

        public void truncate(string table) => Connection.truncate(table);

        public List<string> getAllTables() => Connection.getAllTables();

        public bool tableExits(string tablename) => Connection.tableExits(tablename);

        public void updateTable(string table, Dictionary<string, string> data, string where = "") => Connection.updateTable(table, data, where);

        public List<DataColumn> getColumns(string tablename) => Connection.getColumns(tablename);

        /// <summary>
        /// Create Table sys_config, sys_history, app_action, app_menu, app_menu_list, importData(id (auto), user(50), action(255), f0->f49(500))
        /// </summary>
        /// <param name="db"></param>
        public void checkTableSystem() => Execute(SQLiteHelper.tsqlCreateTableSystemSqlite());
    }

    public static class SQLiteHelper
    {
        private static Regex regDate = new Regex(@"^\d{4}-\d{1,2}-\d{1,2}$");
        private static string dateVNtoDateSqliteString(this string dateVN)
        {
            var ms = Regex.Matches(dateVN, "[0-9]+");
            if (ms.Count == 0) { return "1900-01-01"; }
            if (ms.Count == 1) { return $"{ms[0].Value}-01-01"; }
            if (ms.Count == 2) { return $"{ms[1].Value}-{ms[0].Value}-01"; }
            if (ms.Count == 3) { return $"{ms[2].Value}-{ms[1].Value}-{ms[0].Value}"; }
            if (ms.Count == 4) { return $"{ms[3].Value}-{ms[2].Value}-{ms[1].Value} {ms[0].Value}:00"; }
            if (ms.Count == 5) { return $"{ms[4].Value}-{ms[3].Value}-{ms[2].Value} {ms[1].Value}:{ms[0].Value}"; }
            return $"{ms[5].Value}-{ms[4].Value}-{ms[3].Value} {ms[2].Value}:{ms[1].Value}:{ms[0].Value}";
        }

        /// <summary>
        /// Date: format dd/MM/yyyy or yyyy-MM-dd; Result: List<string> { where, title }
        /// </summary>
        /// <param name="field"></param>
        /// <param name="string_date1"></param>
        /// <param name="string_date2"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static List<string> sqliteLikeDate(this string field, string string_date1, string string_date2, string title = "", bool getTime = false)
        {
            if (string.IsNullOrEmpty(string_date1) && string.IsNullOrEmpty(string_date2)) { return new List<string>(); }
            var date = new List<string> { string_date1.Trim(), string_date2.Trim() };
            if (string.IsNullOrEmpty(date[0]) == false)
            {
                if (Regex.IsMatch(date[0], "^[0-9]{1,2}/[0-9]{1,2}/[0-9]{4}")) { date[0] = string_date1.dateVNtoDateSqliteString(); }
                else if (Regex.IsMatch(date[0], "^[0-9]{4}-[0-9]{1,2}-[0-9]{1,2}") == false) { return new List<string>(); }
            }
            if (string.IsNullOrEmpty(date[1]) == false)
            {
                if (Regex.IsMatch(date[1], "^[0-9]{1,2}/[0-9]{1,2}/[0-9]{4}")) { date[1] = string_date2.dateVNtoDateSqliteString(); }
                else if (Regex.IsMatch(date[1], "^[0-9]{4}-[0-9]{1,2}-[0-9]{1,2}") == false) { return new List<string>(); }
            }
            if (date[0] == date[1])
            {
                title = title + " ngày " + string_date1;
                if (getTime && date[0].Contains(" ") == false)
                {
                    return new List<string> { $"({field}>='{date[0]}' and {field}<='{date[0]} 23:59:59')", title.Trim() };
                }
                return new List<string> { $"{field}='{date[0]}'", title.Trim() };
            }
            if (string.IsNullOrEmpty(date[1]))
            {
                title = title + " từ ngày " + string_date1;
                return new List<string> { $"{field}>='{date[0]}'", title.Trim() };
            }
            if (string.IsNullOrEmpty(date[0]))
            {
                title = title + " đến ngày " + string_date2;
                if (getTime && date[1].Contains(" ") == false) { return new List<string> { $"{field}<='{date[1]} 23:59:59'", title.Trim() }; }
                return new List<string> { $"{field}<='{date[1]}'", title.Trim() };
            }
            title = $"{title} từ ngày {string_date1} đến {string_date2}".Trim();
            if (getTime && date[1].Contains(" ") == false) { return new List<string> { $"({field}>='{date[0]}' and {field}<='{date[1]} 23:59:59')", title }; }
            return new List<string> { $"({field}>='{date[0]}' and {field}<='{date[1]}')", title };
        }

        public static List<string> sqliteLikeDate_old(this string field, string date1, string date2, string title = "")
        {
            if (string.IsNullOrEmpty(date1)) { return new List<string>(); }
            var rs = new List<string>();
            if (regDate.IsMatch(date1))
            {
                if (date1 == date2 || regDate.IsMatch(date2) == false)
                {
                    rs.Add($"({field} >= '{date1}' and {field} <= '{date1} 23:59')");
                    if (title != "") { rs.Add($"{title} ngày {date1}"); }
                    return rs;
                }
                rs.Add($"({field} >= '{date1}' and {field} <= '{date2} 23:59')");
                if (title != "") { rs.Add($"{title} từ ngày {date1} đến {date2}"); }
                return rs;
            }
            var t1 = new DateTime();
            var t2 = new DateTime();
            t1 = date1.toDateTimeVN();
            if (!string.IsNullOrEmpty(date2)) { t2 = date2.toDateTimeVN(); }
            else { t2 = t1; };
            if (t1 == t2)
            {
                rs.Add($"({field} >= '{t1:yyyy-MM-dd}' and {field} <= '{t1:yyyy-MM-dd} 23:59')");
                if (title != "") { rs.Add($"{title} ngày {date1}"); }
                return rs;
            }
            rs.Add($"({field} >= '{t1:yyyy-MM-dd}' and {field} <= '{t2:yyyy-MM-dd} 23:59')");
            if (title != "") { rs.Add($"{title} từ ngày {date1} đến {date2}"); }
            return rs;
        }

        /// <summary>
        /// Execute update or innert data
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <param name="data"></param>
        /// <param name="where"></param>
        public static void Execute(this DbContext db, Dictionary<string, string> data, string tableName, string where = "")
        {
            if (string.IsNullOrEmpty(tableName)) { return; }
            where = where.Trim();
            var tsql = "";
            if (string.IsNullOrEmpty(where))
            {
                /* INNSERT DATA */
                var fields = new List<string>(); var vals = new List<string>();
                foreach (var item in data)
                {
                    if (item.Key.StartsWith("[")) { fields.Add(item.Key); } else { fields.Add($"[{item.Key}]"); }
                    vals.Add($"'{item.Value.sqliteGetValueField()}'");
                }
                tsql = $"insert into {tableName}({string.Join(",", fields)}) values ({string.Join(",", vals)})";
            }
            else
            {
                /* Update data */
                var vals = new List<string>(); 
                foreach (var item in data)
                {
                    if (item.Key.StartsWith("[")) { vals.Add($"{item.Key}='{sqliteGetValueField(item.Value)}'"); }
                    else { vals.Add($"[{item.Key}]='{sqliteGetValueField(item.Value)}'"); }
                }
                if (Regex.IsMatch(where, "^where", RegexOptions.IgnoreCase)) { where = where.Substring(5).Trim(); }
                tsql = $"update {tableName} set {string.Join(",", vals)} where " + where;
            }
            db.Database.ExecuteSqlCommand(tsql);
        }

        public static void Execute(this DbContext db, string tsql, params object[] param)
        {
            if (db.Database.Connection.State == ConnectionState.Closed) db.Database.Connection.Open();
            if (param == null) { db.Database.ExecuteSqlCommand(tsql); return; }
            if (param.Length == 0) { db.Database.ExecuteSqlCommand(tsql); return; }
            db.Database.ExecuteSqlCommand(tsql, param);
        }

        public static void Execute(this SQLiteConnection cn, string tsql, int commandTimeOut = 3600)
        {
            if (string.IsNullOrEmpty(tsql)) { return; }
            if (cn.State == ConnectionState.Closed) { cn.Open(); }
            var sqlcmd = cn.CreateCommand();
            sqlcmd.CommandTimeout = commandTimeOut;
            sqlcmd.CommandText = tsql;
            sqlcmd.ExecuteNonQuery();
        }

        public static void Execute(this SQLiteConnection cn, SQLiteCommand cmd, int commandTimeOut = 3600)
        {
            if (cn.State == ConnectionState.Closed) { cn.Open(); }
            cmd.Connection = cn;
            cmd.CommandTimeout = commandTimeOut;
            cmd.ExecuteNonQuery();
        }

        public static void delete(this DbContext db, string table) => db.Execute($"delete from [{table}];");

        public static void delete(this SQLiteConnection cn, string table, string where = "")
        {
            if (string.IsNullOrEmpty(where)) { cn.Execute("delete from " + table); return; }
            if (Regex.IsMatch(where, "^where", RegexOptions.IgnoreCase)) { where = where.Substring(5).Trim(); }
            cn.Execute($"delete from {table} where {where}");
        }

        public static void truncate(this DbContext db, string table) => db.Execute($"delete from [{table}]; UPDATE sqlite_sequence SET seq = 0 WHERE name = '{table}';");

        public static void truncate(this SQLiteConnection cn, string table) => cn.Execute($"delete from [{table}]; UPDATE sqlite_sequence SET seq = 0 WHERE name = '{table}';");

        public static string tsqlCreateTableSystemSqlite(List<string> tablesExists = null)
        {
            var tsql = new List<string>();
            List<string> tables = tablesExists == null ? new List<string>() : tablesExists;
            if (tables == null) { tables = new List<string>(); }
            /* sys-config */
            if (tables.Contains("sys-config") == false)
            {
                tsql.Add("CREATE TABLE IF NOT EXISTS [sys-config] ([name] nvarchar(50) NOT NULL PRIMARY KEY, [value] nvarchar(1000) NOT NULL DEFAULT '', [description] nvarchar(1000) NOT NULL DEFAULT '');");
            }
            if (tables.Contains("sys-userconfig") == false)
            {
                tsql.Add("CREATE TABLE IF NOT EXISTS [sys-userconfig] ([name] nvarchar(50) NOT NULL PRIMARY KEY, [value] nvarchar(1000) NOT NULL DEFAULT '', [description] nvarchar(1000) NOT NULL DEFAULT '');");
            }
            /* sys_history */
            if (tables.Contains("sys-history") == false)
            {
                tsql.Add("CREATE TABLE IF NOT EXISTS [sys-history] ([id] integer NOT NULL PRIMARY KEY AUTOINCREMENT, [iduser] nvarchar(50) NOT NULL DEFAULT '', [typeinfo] nvarchar(50) NOT NULL DEFAULT '', [pathorherf] nvarchar(1000) NOT NULL DEFAULT '', [action] nvarchar(255) NOT NULL DEFAULT '', [content] nvarchar(4000) NOT NULL, [times] datetime NOT NULL default (datetime('now', 'localtime')));");
                tsql.Add("create index IF NOT EXISTS sys-history-index on sys-history (user, typeinfo, pathorherf, action, times); ");
            }
            /* app_action */
            if (tables.Contains("app-action") == false)
            {
                tsql.Add("CREATE TABLE IF NOT EXISTS [app-action] ([id] integer NOT NULL PRIMARY KEY AUTOINCREMENT, [name] nvarchar(50) NOT NULL DEFAULT '', [path] nvarchar(1000) NOT NULL DEFAULT '', [execute] nvarchar(1000) NOT NULL DEFAULT '', [description] nvarchar(4000) NOT NULL, [times] datetime NOT NULL default (datetime('now', 'localtime')));");
            }
            /* app_menu */
            if (tables.Contains("app-menu") == false)
            {
                tsql.Add("CREATE TABLE IF NOT EXISTS [app-menu] ([id] integer NOT NULL PRIMARY KEY AUTOINCREMENT, [name] nvarchar(50) NOT NULL DEFAULT '', [description] nvarchar(4000) NOT NULL, [times] datetime NOT NULL default (datetime('now', 'localtime')));");
            }
            /* app_menu_list */
            if (tables.Contains("app-menu-list") == false)
            {
                tsql.Add("CREATE TABLE IF NOT EXISTS [app-menu-list] ( [id] integer NOT NULL PRIMARY KEY AUTOINCREMENT, [id0] integer NOT NULL DEFAULT 0, [idmenu] integer NOT NULL DEFAULT 0, [name] nvarchar(50) NOT NULL DEFAULT '', [path] nvarchar(1000) NOT NULL DEFAULT '', [action] nvarchar(1000) NOT NULL DEFAULT '', [style] nvarchar(1000) NOT NULL DEFAULT '', [description] nvarchar(4000) NOT NULL, [times] datetime NOT NULL default (datetime('now', 'localtime')));");
            }
            /* sys-import*/
            if (tables.Contains("sys-import") == false)
            {
                tsql.Add("CREATE TABLE IF NOT EXISTS [sys-import] ([id] integer NOT NULL PRIMARY KEY AUTOINCREMENT,[user] nvarchar(50) NOT NULL DEFAULT '',[action] nvarchar(255) NOT NULL DEFAULT '',[f0] nvarchar(1000),[f1] nvarchar(1000),[f2] nvarchar(1000),[f3] nvarchar(1000),[f4] nvarchar(1000),[f5] nvarchar(1000),[f6] nvarchar(1000),[f7] nvarchar(1000),[f8] nvarchar(1000),[f9] nvarchar(1000),[f10] nvarchar(1000),[f11] nvarchar(1000),[f12] nvarchar(1000),[f13] nvarchar(1000),[f14] nvarchar(1000),[f15] nvarchar(1000),[f16] nvarchar(1000),[f17] nvarchar(1000),[f18] nvarchar(1000),[f19] nvarchar(1000),[f20] nvarchar(1000),[f21] nvarchar(1000),[f22] nvarchar(1000),[f23] nvarchar(1000),[f24] nvarchar(1000),[f25] nvarchar(1000),[f26] nvarchar(1000),[f27] nvarchar(1000),[f28] nvarchar(1000),[f29] nvarchar(1000),[f30] nvarchar(1000),[f31] nvarchar(1000),[f32] nvarchar(1000),[f33] nvarchar(1000),[f34] nvarchar(1000),[f35] nvarchar(1000),[f36] nvarchar(1000),[f37] nvarchar(1000),[f38] nvarchar(1000),[f39] nvarchar(1000),[f40] nvarchar(1000),[f41] nvarchar(1000),[f42] nvarchar(1000),[f43] nvarchar(1000),[f44] nvarchar(1000),[f45] nvarchar(1000),[f46] nvarchar(1000),[f47] nvarchar(1000),[f48] nvarchar(1000),[f49] nvarchar(1000));");
                tsql.Add("create index IF NOT EXISTS [sys-import-index] on sys-import (user, action);");
            }
            return string.Join(" ", tsql);
        }

        /// <summary>
        /// Create Table sys-config, sys-userconfig, sys-history, app-action, app-menu, app-menu-list, sys-import(id (auto), user(50), action(255), f0->f49(1000))
        /// </summary>
        /// <param name="db"></param>
        public static void checkTableSystem(this DbContext db, List<string> tables = null) => (db.Database.Connection as SQLiteConnection).checkTableSystem(tables);

        public static void checkTableSystem(this SQLiteConnection cn, List<string> tables = null)
        {
            if (tables == null) { tables = new List<string>(); }
            if (tables.Count == 0) { tables = cn.getAllTables(); }
            var tsql = tsqlCreateTableSystemSqlite(tables).Trim();
            if (tsql != "") { cn.Execute(string.Join(" ", tsql)); }
        }

        public static object getValue(this DbContext db, string tsql) => (db.Database.Connection as SQLiteConnection).getValue(tsql);

        public static object getValue(this SQLiteConnection cn, string tsql)
        {
            if (cn.State == ConnectionState.Closed) { cn.Open(); }
            var dt = new DataTable("dt");
            var a = new SQLiteDataAdapter(tsql, cn);
            a.Fill(dt); a.Dispose();
            if (dt.Rows.Count == 0) { return null; }
            return dt.Rows[0][0];
        }

        public static string sqliteGetValueField(this string value) => value.Replace("'", "''");

        public static DataSet getDataSet(this DbContext db, string tsql) => (db.Database.Connection as SQLiteConnection).getDataSet(tsql);

        public static DataSet getDataSet(this SQLiteConnection cn, string tsql)
        {
            if (cn.State == ConnectionState.Closed) { cn.Open(); }
            var ds = new DataSet("dataset");
            var a = new SQLiteDataAdapter(tsql, cn);
            a.Fill(ds); a.Dispose();
            return ds;
        }

        public static bool tableExits(this DbContext db, string tablename) => (db.Database.Connection as SQLiteConnection).tableExits(tablename);

        public static bool tableExits(this SQLiteConnection cn, string tablename) => cn.getValue($"SELECT [name] FROM [sqlite_master] WHERE type='table' AND name <> '{tablename}'") == null ? false : true;

        public static List<string> getViewNames(this DbContext db) => (db.Database.Connection as SQLiteConnection).getViewNames();
        public static List<string> getViewNames(this SQLiteConnection cn)
        {
            var l = new List<string>();
            var dt = cn.getDataSet($"SELECT [name] FROM [sqlite_master] WHERE type= 'view' AND name not like 'sqlite_%'").Tables[0];
            foreach (DataRow r in dt.Rows) l.Add($"{r[0]}");
            return l;
        }
        public static List<string> getTableNames(this DbContext db) => (db.Database.Connection as SQLiteConnection).getTableNames();
        public static List<string> getTableNames(this SQLiteConnection cn)
        {
            var l = new List<string>();
            var dt = cn.getDataSet($"SELECT [name] FROM [sqlite_master] WHERE type= 'table' AND name not like 'sqlite_%'").Tables[0];
            foreach (DataRow r in dt.Rows) l.Add($"{r[0]}");
            return l;
        }
        public static List<string> getAllTables(this DbContext db, bool views = false) => (db.Database.Connection as SQLiteConnection).getAllTables(views);
        public static List<string> getAllTables(this SQLiteConnection cn, bool views = false)
        {
            var l = new List<string>();
            string type = "'table'";
            if (views) { type = "'table', 'view'"; }
            var dt = cn.getDataSet($"SELECT [name] FROM [sqlite_master] WHERE type IN ({type}) AND name not like 'sqlite_%'").Tables[0];
            foreach (DataRow r in dt.Rows) l.Add($"{r[0]}");
            return l;
        }

        public static List<DataColumn> getColumns(this DbContext db, string tablename) => (db.Database.Connection as SQLiteConnection).getColumns(tablename);

        public static List<DataColumn> getColumns(this SQLiteConnection cn, string tablename)
        {
            var l = new List<DataColumn>();
            var dt = cn.getDataSet($"SELECT * FROM {tablename} limit 1").Tables[0];
            foreach (DataColumn c in dt.Columns) l.Add(c);
            return l;
        }

        public static string sqliteLike(this string value, string fieldName)
        {
            if(string.IsNullOrEmpty(value)) { return ""; }
            if(Regex.IsMatch(value, "[*%_?]+") == false) { return $"{fieldName} = '{value.sqliteGetValueField()}'"; }
            if (value.Contains("*")) { value = value.Replace("*", "%"); }
            if (value.Contains("?")) { value = value.Replace("?", "_"); }
            value = Regex.Replace(value, "[%]+", "%");
            return $"{fieldName} like '{value.sqliteGetValueField()}'";
        }

        public static string sqliteLike(this string value, List<string> fieldNames)
        {
            if (string.IsNullOrEmpty(value)) { return ""; } 
            var ls = new List<string>();
            if (Regex.IsMatch(value, "[*%_?]+") == false) {
                value = value.sqliteGetValueField();
                foreach (var v in fieldNames) ls.Add($"{v}='{value}'");
                return string.Join(" or ", ls); 
            }
            if (value.Contains("*")) { value = value.Replace("*", "%"); }
            if (value.Contains("?")) { value = value.Replace("?", "_"); }
            value = (Regex.Replace(value, "[%]+", "%")).sqliteGetValueField();
            foreach (var v in fieldNames) ls.Add($"{v} like '{value}'");
            return string.Join(" or ", ls);
        }

        public static void backup(this SQLiteConnection cn, string pathsave)
        {
            using (var destination = new SQLiteConnection($"Data Source={pathsave};"))
            {
                bool closed = false;
                if (cn.State == ConnectionState.Closed) { cn.Open(); closed = true; }
                destination.Open();
                cn.BackupDatabase(destination, "main", "main", -1, null, 0);
                if (closed) { cn.Close(); }
            }
        }

        public static void backup(this DbContext db, string pathsave) => (db.Database.Connection as SQLiteConnection).backup(pathsave);

        public static SQLiteDataReader dataReader(this SQLiteConnection cn, string tsql, int commandTimeout = 3600)
        {
            if (cn.State == ConnectionState.Closed) { cn.Open(); }
            var cmd = cn.CreateCommand();
            cmd.CommandText = tsql;
            cmd.CommandTimeout = commandTimeout;
            return cmd.ExecuteReader();
        }

        public static SQLiteDataReader dataReader(this SQLiteConnection cn, SQLiteCommand cmd, int commandTimeout = 3600)
        {
            if (cn.State == ConnectionState.Closed) { cn.Open(); }
            cmd.Connection = cn;
            cmd.CommandTimeout = commandTimeout;
            return cmd.ExecuteReader();
        }

        public static SQLiteDataReader dataReader(this DbContext db, string tsql, int commandTimeout = 3600) => (db.Database.Connection as SQLiteConnection).dataReader(tsql, commandTimeout);

        public static SQLiteDataReader dataReader(this DbContext db, SQLiteCommand cmd, int commandTimeout = 3600) => (db.Database.Connection as SQLiteConnection).dataReader(cmd, commandTimeout);

        public static HSSFWorkbook exportXLS(this SQLiteDataReader reader, string FileTemplate = "", string PathSave = "", List<CellMerge> lsTieuDe = null, int RowIndex = 0, bool ShowHeader = true, bool addColumnAutoNumber = false, string formatDate = "dd/MM/yyyy HH:mm:ss")
        {
            /* Tạo mới */
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            if (FileTemplate != "")
            {
                var fs = new FileStream(FileTemplate, FileMode.Open, FileAccess.Read);
                hssfworkbook = new HSSFWorkbook(fs);
                fs.Close();
                fs.Dispose();
            }
            /* Mặc định từ dòng thứ 2 đi */
            int index = RowIndex <= 0 ? 1 : RowIndex;
            int pointIndex = index - 1;
            /* Tạo hoặc lấy Sheet */
            var sheet = FileTemplate == "" ? hssfworkbook.CreateSheet() : hssfworkbook.GetSheetAt(0);
            /* Tạo đường viền của ô */
            var cell = hssfworkbook.CreateCellStyleThin();
            /* tạo tiêu đề */
            var cellb = hssfworkbook.CreateCellStyleTitle();
            var fb = hssfworkbook.CreateFontTahomaBold();
            if (ShowHeader)
            {
                var cr = sheet.CreateRow(index - 1);
                int i = 0;
                if (addColumnAutoNumber)
                {
                    ICell stt = cr.CreateCell(0, CellType.String);
                    stt.SetCellValue("STT");
                    stt.CellStyle = cellb;
                    for (int j = 0; j < reader.FieldCount; j++) { var cc = cr.CreateCell(i + 1, CellType.String); cc.SetCellValue(reader.GetName(j)); cc.CellStyle = cellb; cc.CellStyle.SetFont(fb); i++; }
                }
                else { for (int j = 0; j < reader.FieldCount; j++) { var cc = cr.CreateCell(i, CellType.String); cc.SetCellValue(reader.GetName(j)); cc.CellStyle = cellb; i++; } }
            }
            /* Kiểm tra và đặt tiêu đề */
            if (lsTieuDe == null) { lsTieuDe = new List<CellMerge>(); }
            foreach (var item in lsTieuDe)
            {
                /* Lấy vị trí RowIndex dòng tiêu đề */
                IRow row = sheet.GetRow(item.RowIndex);
                if (row == null) row = sheet.CreateRow(item.RowIndex);
                /* Lấy vị trí ColumnIndex và đặt giá trị */
                var cTitle = row.GetCell(item.ColumnIndex);
                if (cTitle == null)
                {
                    var column = row.CreateCell(item.ColumnIndex);
                    column.SetCellValue(item.Value);
                    column.CellStyle = cellb;
                }
                else
                {
                    cTitle.SetCellValue(item.Value);
                    cTitle.CellStyle = cellb;
                }
                if (item.MergeColumnCount > 0 || item.MergeRowCount > 0)
                {
                    int LastRow = item.MergeRowCount > item.RowIndex ? item.MergeRowCount : item.RowIndex;
                    int LastColumn = item.MergeColumnCount > item.ColumnIndex ? item.MergeColumnCount : item.ColumnIndex;
                    var cellRange = new CellRangeAddress(item.RowIndex, LastRow, item.ColumnIndex, LastColumn);
                    sheet.AddMergedRegion(cellRange);
                }
            }
            /* Xuất dữ liệu */
            if (addColumnAutoNumber)
            {
                while (reader.Read())
                {
                    var cr = sheet.CreateRow(index);
                    ICell stt = cr.CreateCell(0, CellType.String);
                    stt.SetCellValue((index - pointIndex).ToString());
                    stt.CellStyle = cell;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Type t = reader[i].GetType();
                        if (XLS.typeNumber.Contains(t))
                        {
                            var cc = cr.CreateCell(i + 1, CellType.Numeric);
                            if (reader.GetValue(i) == DBNull.Value) cc.SetCellValue(0);
                            else cc.SetCellValue(double.Parse(reader.GetValue(i).ToString()));
                            cc.CellStyle = cell;
                        }
                        else
                        {
                            var cc = cr.CreateCell(i + 1, CellType.String);
                            if (XLS.typeDateTime.Contains(t))
                            {
                                if (reader.GetValue(i) == null) { cc.SetCellValue(""); }
                                else { cc.SetCellValue(((DateTime)reader.GetValue(i)).ToString(formatDate)); }
                            }
                            else
                            {
                                cc.SetCellValue(reader.GetValue(i).ToString());
                            }
                            cc.CellStyle = cell;
                        }
                    }
                    index++;
                }
            }
            else
            {
                while (reader.Read())
                {
                    var cr = sheet.CreateRow(index);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Type t = reader[i].GetType();
                        if (XLS.typeNumber.Contains(t))
                        {
                            var cc = cr.CreateCell(i, CellType.Numeric);
                            if (reader.GetValue(i) == DBNull.Value) cc.SetCellValue(0);
                            else cc.SetCellValue(double.Parse(reader.GetValue(i).ToString()));
                            cc.CellStyle = cell;
                        }
                        else
                        {
                            var cc = cr.CreateCell(i, CellType.String);
                            if (XLS.typeDateTime.Contains(t))
                            {
                                if (reader.GetValue(i) == null) { cc.SetCellValue(""); }
                                else { cc.SetCellValue(((DateTime)reader.GetValue(i)).ToString(formatDate)); }
                            }
                            else
                            {
                                cc.SetCellValue(reader.GetValue(i).ToString());
                            }
                            cc.CellStyle = cell;
                        }
                    }
                }
            }
            return hssfworkbook;
        }

        public static void updateTable(this SQLiteConnection cn, string table, Dictionary<string, string> data, string where = "")
        {
            if (string.IsNullOrEmpty(table)) { return; }
            if (data.Count == 0) { return; }
            if (cn.State == ConnectionState.Closed) { cn.Open(); }
            if (string.IsNullOrEmpty(where) == false)
            {
                /* Addnew*/
                var fields = new List<string>(); var values = new List<string>();
                foreach (var v in data) { fields.Add(v.Key); values.Add($"'{v.Value.sqliteGetValueField()}'"); }
                cn.Execute($"insert into {table}({string.Join(",", fields)}) values ({string.Join(",", values)});");
                return;
            }
            var modifies = new List<string>();
            foreach (var v in data) { modifies.Add($"{v.Key}='{v.Value.sqliteGetValueField()}'"); }
            where = where.Trim();
            if (Regex.IsMatch(where, "^where", RegexOptions.IgnoreCase)) { where = where.Substring(5).Trim(); }
            cn.Execute($"update {table} set {string.Join(",", modifies)} where {where}");
        }

        public static void updateTable(this DbContext db, string table, Dictionary<string, string> data, string where = "") => (db.Database.Connection as SQLiteConnection).updateTable(table, data, where);

        public static void saveXLS(this SQLiteDataReader reader, string FileTemplate = "", string PathSave = "", List<CellMerge> lsTieuDe = null, int RowIndex = 0, bool ShowHeader = true, bool addColumnAutoNumber = true)
        {
            var xls = reader.exportXLS(FileTemplate, PathSave, lsTieuDe, RowIndex, ShowHeader, addColumnAutoNumber);
            if (PathSave == "")
            {
                PathSave = Path.GetPathRoot(FileTemplate);
                PathSave = PathSave.EndsWith("\\") ? PathSave + "template.xls" : PathSave + "\\template.xls";
            }
            if (File.Exists(PathSave)) File.Delete(PathSave);
            using (var fs = new FileStream(PathSave, FileMode.Create, FileAccess.Write)) { xls.Write(fs); }
            xls.Clear();
            reader.Close();
        }

        public static void bulkCopy(this DataTable dt, DbContext db, string toTableName = "", int batchSize = 1000, int timeout = 3600) => (db.Database.Connection as SQLiteConnection).bulkCopy(dt, toTableName, batchSize, timeout);

        public static void bulkCopy(this SQLiteConnection cn, DataTable dt, string toTableName = "", int batchSize = 1000, int timeout = 3600)
        {
            if (batchSize > 1000 || batchSize < 10) { batchSize = 1000; }
            if (timeout < 10) { timeout = 3600; }
            if (string.IsNullOrEmpty(toTableName)) { toTableName = dt.TableName; }
            var vals = new List<string>();
            foreach (DataColumn c in dt.Columns) { vals.Add(c.ColumnName); }
            var tsql = $"insert into {toTableName} ({string.Join(",", vals)}) values";
            cn.DefaultTimeout = 3600;
            vals = new List<string>();
            foreach (DataRow r in dt.Rows)
            {
                if (vals.Count >= batchSize) { cn.Execute($"{tsql} {string.Join(",", vals)}"); vals = new List<string>(); }
                var v = new List<string>();
                for (int i = 0; i < dt.Columns.Count; i++) { v.Add($"{r[i]}".sqliteGetValueField()); }
                vals.Add($"('{string.Join("','", v)}')");
            }
            if (vals.Count > 0) { cn.Execute($"{tsql} {string.Join(",", vals)}"); }
        }
    }
}