using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using System.Xml;
using zModules;
using zModules.AspNet;
using zModules.Config;
using zModules.Crypt;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe
{

    public static class local
    {
        public static MD5 md5 = new MD5();
        public static fonts zfont = new fonts();
        public static string connectionstring = "";
        public static string projectName = "";
        private static Regex regNumber = new Regex(@"^[0-9]+$");
        /* Other */
        public static Dictionary<string, string> LYesNo = new Dictionary<string, string>() { { "", "" }, { "có", "có" }, { "không", "không" } };
        public static Dictionary<string, string> LAddRemove = new Dictionary<string, string>() { { "", "" }, { "+", "+" }, { "-", "-" } };
        public static List<link> listLinkMVC = new List<link>();
        /* Thống kê danh sách ID đăng nhập */
        public static Dictionary<string, string> IpConnect = new Dictionary<string, string>();
        /* User - Permission */
        public static Dictionary<string, int> SessionUser = new Dictionary<string, int>();
        /* Tiến trình đang chạy */
        public static TaskList taskList = new TaskList();

        public static string setFormatOnilne(this HttpRequest r, HttpSessionState s) => $"{s[keyS.idUser]}|{r.getIPClient()}:{(r.Browser.IsMobileDevice ? "Mobile" : "PC")}[{r.getBrowerName()}]|{s[keyS.timeStart]}";

        public static string setFormatOnilne(this HttpRequestBase r, HttpSessionStateBase s) => $"{s[keyS.idUser]}|{r.getIPClient()}:{(r.Browser.IsMobileDevice ? "Mobile" : "PC")}[{r.getBrowerName()}]|{s[keyS.timeStart]}";

        public static string setFormatOnilne(this HttpContext w) => w.Request.setFormatOnilne(w.Session);

        public static string setFormatOnilne(this HttpContextBase w) => w.Request.setFormatOnilne(w.Session); public static Models.SQLServerDataContext getDataObject() => new Models.SQLServerDataContext(connectionstring) { CommandTimeout = 3600 };
        public static Models.DBSQLite.dbEntities getDataSQLite(string pathFile = "")
        {
            string connectionstring = "";
            if (string.IsNullOrEmpty(pathFile)) { pathFile = Folders.App_Data + "\\db.db3"; }
            connectionstring = Tools.getConnectionStringSQLiteEF6(pathFile, "Models.DBSQLite.SQLiteFile");
            return new Models.DBSQLite.dbEntities(connectionstring);
        }
        public static string isSoCMND(string socmnd, string gioitinh, string namsinh, List<string> dmhanhchinh)
        {
            if (Regex.IsMatch(socmnd, "^[0-9]{9}$"))
            {
                if (int.Parse(socmnd.Substring(0, 2)) > 38) { return "Mã đơn vị hành chính không nằm trong danh sách"; }
                return "";
            }
            if (Regex.IsMatch(socmnd, "^[0-9]{12}$") == false) { return "01. Không đúng định dạng [(9|12) ký tự số]"; }
            if (dmhanhchinh.Count > 0) { if (dmhanhchinh.Contains(socmnd.Substring(0, 3)) == false) { return "Mã đơn vị hành chính không nằm trong danh sách"; } }
            if (Regex.IsMatch(namsinh, "^[0-9]{4}$") == false) { return "04. Năm sinh không đúng"; }
            if (int.Parse(namsinh) < 1900 || int.Parse(namsinh) > 2399) { return "05. Năm sinh nằm ngoài thế kỷ 20-24"; }
            if (socmnd.Substring(4, 2) != namsinh.Substring(2, 2)) { return "07. Mã năm sinh không đúng"; }
            /* Xác định thế kỷ - 1; Xác định giới hạn magt */
            var magt = int.Parse(socmnd.Substring(3, 1));
            /* Nam */
            var gt = (int.Parse(namsinh.Substring(0, 2)) - 19) * 2;
            /* Nữ */
            if (gioitinh.Contains("a")) { gt = gt + 1; }
            if (magt != gt) { return "06. Mã giới tính không đúng"; }
            return "";
        }
        public static bool isLogin(this HttpSessionState s)
        {
            if (s[keyS.isLogin] == null) return false;
            if (s[keyS.isLogin].ToString() != "1") return false;
            if (s[keyS.idUser] == null) return false;
            return s[keyS.idUser].ToString().Trim() != "";
        }

        public static bool isLogin(this HttpSessionStateBase s)
        {
            if (s[keyS.isLogin] == null) return false;
            if (s[keyS.isLogin].ToString() != "1") return false;
            if (s[keyS.idUser] == null) return false;
            return s[keyS.idUser].ToString().Trim() != "";
        }

        public static string getNameController(this RouteData r) => $"{r.Values["controller"]}";

        public static string getNameAction(this RouteData r) => $"{r.Values["action"]}";

        public static string getNameArea(this RouteData r) => $"{r.DataTokens["area"]}";

        public static string herf(RouteValueDictionary rt)
        {
            try
            {
                var s = "controller/action/id".Split('/');
                if (s.Count() == 0) return "";
                var l = new List<string>();
                foreach (var v in s) { if (rt.ContainsKey(v)) l.Add(rt[v].ToString().ToLower()); }
                return "/" + l.ToJoin("/");
            }
            catch (Exception ex) { ex.save(); return ""; }
        }

        public static bool setLogin(LoginInfo sender = null)
        {
            var w = HttpContext.Current;
            /* Kiểm tra đăng nhập Remmenber */
            if (sender == null)
            {
                sender = new LoginInfo();
                /* Check Session */
                sender.password = w.Session[keyS.isLogin] == null ? "0" : w.Session[keyS.isLogin].ToString();
                sender.idUser = w.Session[keyS.idUser] == null ? "" : w.Session[keyS.idUser].ToString();
                if (sender.password == "1")
                {
                    if (string.IsNullOrEmpty(sender.idUser) == false)
                    {
                        IpConnect[w.Session.SessionID] = w.Request.setFormatOnilne(w.Session);
                        return true;
                    }
                }
                /* Check cookie */
                /* Lấy thông tin ghi nhớ đăng nhập */
                if (w.Request.Cookies[KeyCookie.auth] == null) { return false; }
                string cryt = w.Request.Cookies[KeyCookie.auth].Value.Trim();
                if (cryt == "") { return false; }
                /* Giải mã */
                cryt = md5.GiaiMa(cryt, AppConfig.Key);
                var info = cryt.Split('\n');
                /* Sai định dạng */
                if (info.Count() < 3) { return false; }
                /* Sai định dạng thời hạn hoặc đã bị quá hạn */
                DateTime time_rem = new DateTime();
                if (info[2].isDateTimeVN() == false) { return false; }
                else { time_rem = info[2].toDateTimeVN(); }
                if (time_rem < DateTime.Now) { return false; }
                /* Gắn thông tin */
                sender = new LoginInfo(info[0], info[1], true);
            }
            /* LoginMulti */
            if (AppConfig.loginMulti != 1)
            {
                /* Kiểm tra đã đăng nhập chưa. Thời gian đăng nhập đã quá hạn chưa */
                if (IpConnect.ContainsValue("|" + sender.idUser)) { w.Session.saveError("Bạn đang đăng nhập ở nơi khác!"); return false; }
            }
            /* Tài khoản system - administrator - config system - auto */
            try
            {
                if (sender.idUser.ToLower() == "administrator")
                {
                    /* create + get systemconfig.xml */
                    var cfg = new zModules.Config.Config(Folders.App_Data + "\\systemconfig.xml");
                    var pass = cfg.getValue("password");
                    if (string.IsNullOrEmpty(pass))
                    {
                        pass = "maianh094@".toPassword();
                        cfg.ModifiItemAppSetting("password", pass);
                    }
                    if (cfg.getValue("password") != sender.password) { throw new Exception("Mật khẩu không đúng"); }
                    /* Đăng nhập thành công */
                    w.setLoginSuccess(sender, "0");
                    return true;
                }
            }
            catch (Exception ex) { w.Session.saveError(ex.Message); return false; }
            if (AppConfig.value("app_offline", "") == "1")
            {
                var msg = AppConfig.value("app_messenger", "").Trim();
                if (string.IsNullOrEmpty(msg)) { msg = "Website đang bảo trì. Vui lòng quay lại sau."; }
                w.Session.saveError(msg); return false;
            }
            var rs = false;
            string capdo = "";
            var urlLogin = AppConfig.value("app.urlserver", "");
            if (string.IsNullOrEmpty(urlLogin) == false)
            {
                /* Login from Server */
                try
                {
                    var key = HttpUtility.UrlEncode(md5.MaHoa($"{DateTime.Now:yyMMddHH}|{sender.idUser}|{sender.password}", AppConfig.Key));
                    var stream = new XmlTextReader($"{urlLogin}/api/login?key={key}");
                    var dt = new DataTable();
                    dt.ReadXml(stream);
                    if (dt.TableName == "error") { throw new Exception($"{dt.Rows[0][0]}"); }
                    if (dt.Rows.Count == 1)
                    {
                        using (var db = getDataObject())
                        {
                            try
                            {
                                capdo = $"{dt.Rows[0]["capdo"]}";
                                db.Execute($"delete from nguoidung where tendangnhap=N'{sender.idUser.getValueField()}';");
                                dt.bulkCopy(db.Connection.ConnectionString, "nguoidung");
                                rs = true;
                            }
                            catch (Exception ex) { throw new Exception(ex.Message); }
                        }
                    }
                    else { throw new Exception("Lỗi không xác định"); }
                }
                catch (Exception ex) { w.Session.saveError(ex.Message); }
            }
            else
            {
                var db = getDataObject();
                try
                {
                    /* Kiểm tra có bị block không */
                    var userinfo = db.nguoidungs.FirstOrDefault(p => p.tendangnhap == sender.idUser);
                    if (userinfo == null)
                    {
                        /* Trường hơp đăng nhập bằng số điện thoại */
                        if (regNumber.IsMatch(sender.idUser) == false) { throw new Exception($"Tài khoản '{sender.idUser}' không tồn tại trên hệ thống"); }
                        userinfo = db.nguoidungs.FirstOrDefault(p => p.sdt == sender.idUser);
                        if (userinfo == null) { throw new Exception($"Tài khoản '{sender.idUser}' không tồn tại trên hệ thống"); }
                        sender.idUser = userinfo.tendangnhap;
                    }
                    if (userinfo.kichhoat != 1) { throw new Exception($"Tài khoản '{sender.idUser}' chưa kích hoạt hoặc đã bị khóa"); }
                    /* Kiểm tra bản ghi so với dữ liệu */
                    /* Đăng nhập thất bại: xóa thông tin đăng nhập trình duyệt client */
                    if (userinfo.matkhau != sender.password) { throw new Exception("Mật khẩu không đúng"); }
                    rs = true;
                    capdo = userinfo.capdo.ToString();
                    /* Lưu lại lần đăng nhập cuối cùng */
                    db.Execute($"update nguoidung set lancuoi='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' where tendangnhap='{userinfo.tendangnhap.getValueField()}';");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Cannot open database") || ex.Message.Contains("The server was not found"))
                    { w.Session.saveError(ex.Message + " <a href=\"/login?redirect=/sysconfig\"> Cấu hình kết nối </a>"); }
                    else { w.Session.saveError(ex.Message); }
                }
                db.Dispose();
            }
            if (rs) { w.setLoginSuccess(sender, capdo); }
            return rs;
        }

        public static void setLoginSuccess(this HttpContext w, LoginInfo sender, string capdo = "4")
        {
            w.Session[keyS.idUser] = sender.idUser;
            w.Session[keyS.isLogin] = "1";
            w.Session[keyS.capdo] = capdo;
            if (sender.remmember)
            {
                /* Ghi thông tin đăng nhập vào trình duyệt thiết bị kết nối
                // XML IdUser[enter]Password[enter]DateExpiries
                // Thời hạn tự động đăng nhập được lấy từ config - default: 60 ngày
                */
                w.setCookie(KeyCookie.auth, $"{sender.idUser}\n{sender.password}\n{DateTime.Now.AddDays(AppConfig.lifeTimeRemmenberLogin):dd/MM/yyyy HH:mm}", AppConfig.lifeTimeRemmenberLogin, AppConfig.Key);
            }
            else { w.Response.Cookies[KeyCookie.auth].Value = null; }
            /* Thêm vao danh sách IDOnline */
            IpConnect[w.Session.SessionID] = w.Request.setFormatOnilne(w.Session);
        }

        public static SelectList DropDown(Dictionary<string, string> Source, object ValueSelect)
        {
            if (ValueSelect == null) { return new SelectList(Source, "Key", "Value"); }
            return new SelectList(Source, "Key", "Value", ValueSelect);
        }

        public static string toPassword(this string s) => (s + "trabaohiem.local").ToMD5();

        /// <summary>
        /// catChuoi: s: chuỗi cần cắt; len: độ dài của chuỗi sau khi cắt; endstring: những ký tự kết thúc chuỗi sau khi cắt
        /// </summary>
        /// <param name="s"></param>
        /// <param name="len"></param>
        /// <param name="endstring"></param>
        /// <returns></returns>
        public static string catChuoi(this string s, int len, string endstring = "..") => s.CatChuoi(len, endstring);

        public static string getValue(this HttpRequestBase rq, string key, string defaultValue = "") => HttpHelper.getValue(rq, key, defaultValue);

        public static string showRequest(this HttpRequestBase rq, string notStartWith = "", string element = "div") => HttpHelper.ShowRequest(rq, notStartWith, element);

        public static string getBrowerName(this HttpRequest rq) => HttpHelper.getBrowerName(rq);

        public static string getBrowerName(this HttpRequestBase rq) => HttpHelper.getBrowerName(rq);

        public static string xoaDauTV(this string s) => Tools.xoaDauTV(s);

        public static string clearUnicodeSpace(this string s, string charsNotReplace = "") => Tools.clearUnicodeSpace(s, charsNotReplace);

        public static string remove2Space(this string s) => Tools.remove2Space(s);

        public static string getDVT1024(this long _bytes)
        {
            if (_bytes <= ((long)1032)) { return $"{_bytes:#,0}b"; }
            if (_bytes <= (long)1049600) { return $"{Math.Round((double)_bytes / (double)1024, 1):#,0.0}Kb"; }
            /* ‭‭1,073,741,824‬ + ‬ ‭1,048,576‬ = 1,074,790,400*/
            if (_bytes <= (long)1074790400) { return $"{Math.Round((double)_bytes / (double)1048576, 1):#,0.0}Mb"; }
            return $"{Math.Round((double)_bytes / (double)1073741824, 1):#,0.0}Gb";
        } 
        public static string getKey(this List<itemKeyValue> keyValues, string key, string defValue = "")
        {
            if (keyValues == null) { return defValue; }
            if (keyValues.Count == 0) { return defValue; }
            if (string.IsNullOrEmpty(key)) { return defValue; }
            key = key.Trim();
            var obj = keyValues.FirstOrDefault(p=>string.Compare(key, p.Value, true) == 0);
            if(obj == null) { return defValue; }
            return obj.Key;
        }
        public static string getValue(this List<itemKeyValue> keyValues, string value, string defValue = "")
        {
            if (keyValues == null) { return defValue; }
            if (keyValues.Count == 0) { return defValue; }
            if (string.IsNullOrEmpty(value)) { return defValue; }
            value = value.Trim();
            var obj = keyValues.FirstOrDefault(p => string.Compare(value, p.Value, true) == 0);
            if (obj == null) { return defValue; }
            return obj.Value;
        }
        public static string getTenThon(this string thon, string diachi)
        {
            if (string.IsNullOrEmpty(thon) == false) { return thon; }
            if (string.IsNullOrEmpty(diachi)) { return thon; }
            var dc = diachi.Split(',').ToList();
            if (dc.Count < 4) { return ""; }
            return dc[dc.Count - 4];
        }
        public static List<string> getMaTinhHuyenXa(this string diachi, List<Models.dmtinh> dmTinh, List<Models.dmhuyen> dmHuyen, List<Models.dmxa> dmXa)
        {
            var rs = new List<string>() { "", "", "" };
            if (string.IsNullOrEmpty(diachi)) { return rs; }
            var tmp = diachi.Split(new char[] { ',', '-' }, StringSplitOptions.RemoveEmptyEntries).Reverse().ToList();
            var id = dmTinh.Where(p => string.Compare(tmp[0].Trim(), p.ten, true) == 0).Select(p => p.id).FirstOrDefault();
            if (id == null) { return rs; } rs[0] = id;
            if (tmp.Count < 2) { return rs; }
            id = dmHuyen.Where(p => p.idtinh == id && string.Compare(tmp[1].Trim(), p.ten, true) == 0).Select(p => p.id).FirstOrDefault();
            if (id == null) { return rs; } rs[1] = id;
            if (tmp.Count < 3) { return rs; }
            id = dmXa.Where(p => p.idhuyen == id && string.Compare(tmp[2].Trim(), p.ten, true) == 0).Select(p => p.id).FirstOrDefault();
            if (id == null) { return rs; } rs[2] = id;
            return rs;
        } 

        public static string vietHoaDauTu(this string name) => Tools.vietHoaDauTu(name);

        public static int databaseSize(this Models.SQLServerDataContext db) => db.DatabaseSize();

        public static object getConnection(this Models.SQLServerDataContext db) => db.Connection;

        public static string getTenDanToc(this List<Models.dmdantoc> ds, string madantoc, string defValue = "Kinh")
        {
            if (string.IsNullOrEmpty(madantoc)) { return defValue; }
            var item = ds.FirstOrDefault(p => p.id == madantoc);
            if (item == null) { return madantoc; }
            return item.ten;
        }

        internal static Models.nguoidung getUserCurrent(this Models.SQLServerDataContext db, string iduser) => db.nguoidungs.FirstOrDefault(p => p.tendangnhap == iduser);

        internal static Models.nguoidung getUserCurrent(string iduser)
        {
            using (var db = getDataObject()) { return db.getUserCurrent(iduser); }
        }

        internal static List<string> AddFirst(this List<string> s, string item)
        {
            var ls = new List<string>() { item };
            ls.AddRange(s);
            return ls;
        }

        public static string getPathConfig(string pathfile = "")
        {
            if (string.IsNullOrEmpty(pathfile)) { return $"{Folders.App_Data}\\global.config"; } 
            return pathfile;
        } 

        public static void Start()
        {
            /* Check Folder Exists */
            Folders.pathApp = HttpContext.Current.Server.MapPath($"~");
            var s = new List<string>() { "/App_Data", "/App_Data/Images", "/App_Data/Files", "/temp", "/temp/Backup" };
            foreach (var v in s)
            {
                try
                {
                    var d = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~{v}"));
                    if (d.Exists == false) { d.Create(); }
                }
                catch (Exception ex) { ex.save(); }
            }

            listLinkMVC = Assembly.GetExecutingAssembly().getLinkMVC();
            /* Set variable */
            projectName = (typeof(local)).Namespace;
            /* Load Config */
            try { AppConfig.LoadConfig(); }
            catch (Exception ex) { ex.save(); }
            if (string.IsNullOrEmpty(AppConfig.FolderShare)) { AppConfig.FolderShare = "D:\\"; }
            if (AppConfig.pageSize < 1) { AppConfig.pageSize = 100; }
            loadConnectionData();
        }

        public static void loadConnectionData()
        {
            /* Check database */
            connectionstring = AppConfig.dbMssql;
            if (string.IsNullOrEmpty(connectionstring))
            {
                connectionstring = SQLServerHelper.getConnectionString("(local)", "QLHSSK");
                AppConfig.dbMssql = connectionstring;
            }
            /* Check table */
            buildDatabase.createDatabase();
            /* Khai báo kết nối dữ liệu chạy ngầm */
            using (var db = getDataObject())
            {
                try
                {
                    db.Execute("select 1 as x");
                    //taskScheduler.reCallTaskScheduler();
                }
                catch { }
            }
        }

        /// <summary>
        /// Lưu lại thông tin lỗi vào tập tin error.txt; description: ghi chú đi kèm theo lỗi
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="description"></param>
        public static void save(this Exception ex, string description = "") => ex.getLine().saveError(description);

        public static void saveError(this string message, string description = "")
        {
            try
            {
                var sw = new StreamWriter(Folders.pathApp + "error.txt", true, Encoding.Unicode);
                sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                sw.WriteLine(message.Replace(@"D:\Private\bhxhlaocai\trabaohiem_mssqlserver\trabaohiem_mssqlserver", "") + " " + description);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            catch { }
        }

        /// <summary>
        /// Lưu lại thông tin lỗi vào tập tin error.txt; description: ghi chú đi kèm theo lỗi
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="description"></param>
        /// <returns>Trả lại thông báo lỗi</returns>
        public static string saveMessage(this Exception ex, string description = "")
        {
            ex.getLine().saveError(description);
            return ex.Message;
        }

        public static string fixListToHtml(string s, char options)
        {
            if (s.IndexOf(options) >= 0)
            {
                var v = s.Split(options);
                return string.Join("<br />", v);
            }
            return s;
        }

        public static string valSession(string key)
        {
            var s = HttpContext.Current.Session;
            if (s[key] == null) return "";
            return s[key].ToString();
        }

        public static List<string> valSession(string key, char separator)
        {
            var s = HttpContext.Current.Session;
            if (s[key] == null) return new List<string>();
            var v = s[key].ToString().Trim();
            if (v == "") return new List<string>();
            v = v.Replace(" ", "").Replace(separator.ToString(), " ").Trim();
            return v.Split(separator).ToList();
        }

        public static List<string> val_Session(HttpContextBase w, string key, char separator)
        {
            if (w.Session[key] == null) return new List<string>();
            var v = w.Session[key].ToString().Trim();
            if (v == "") return new List<string>();
            v = v.Replace(" ", "").Replace(separator.ToString(), " ").Trim();
            return v.Split(separator).ToList();
        }

        public static string getAlert(object v1, string v2 = null, string alert = "info")
        {
            var s = v1 == null ? "" : $"{v1}";
            var s1 = v2 == null ? "" : $"{v2}";
            if (string.IsNullOrEmpty(s) && string.IsNullOrEmpty(s1)) { return ""; }
            if (string.IsNullOrEmpty(s)) { return $"<div class=\"alert alert-{alert}\" role=\"alert\">{s1}</div>"; }
            return $"<div class=\"alert alert-{alert}\" role=\"alert\">{s} <br /> {s1}</div>";
        }

        public static string getMessage(this HttpSessionState s)
        {
            var msg = $"{s[keyS.Message]}";
            s[keyS.Message] = "";
            return msg;
        }

        public static string getMessage(this HttpSessionStateBase s)
        {
            var msg = $"{s[keyS.Message]}";
            s[keyS.Message] = "";
            return msg;
        }

        public static string getError(this HttpSessionState s)
        {
            var msg = $"{s[keyS.Error]}";
            s[keyS.Error] = "";
            return msg;
        }

        public static string getError(this HttpSessionStateBase s)
        {
            var msg = $"{s[keyS.Error]}";
            s[keyS.Error] = "";
            return msg;
        }

        public static string getValueConfig(this HttpSessionStateBase s, string key, string defaultValue = "")
        {
            using (var db = getDataObject())
            {
                try
                {
                    string pathConfig = getPathConfig();
                    var cfg = new Config(pathConfig);
                    return cfg.getValue(key, defaultValue: defaultValue);
                }
                catch { return defaultValue; }
            }
        }

        public static string getIdUser(this HttpSessionStateBase s) => $"{s[keyS.idUser]}";

        public static void saveError(this HttpSessionState s, string msg) => s[keyS.Error] = msg;

        public static void saveError(this HttpSessionStateBase s, string msg) => s[keyS.Error] = msg;

        public static void saveError(this HttpSessionStateBase s, Exception ex) => s[keyS.Error] = ex.Message;

        public static void saveMessage(this HttpSessionState s, string msg) => s[keyS.Message] = msg;

        public static void saveMessage(this HttpSessionStateBase s, string msg) => s[keyS.Message] = msg;

        public static void HandleUnknownAction(this HttpContextBase w, string actionName = "")
        {
            string v = "";
            if (w.Session[keyS.Error] != null) { v = $"{w.Session[keyS.Error]}"; }
            if (v == "") { v = $"{w.Request.Url.AbsolutePath} {actionName}: Phương thức này đã bị ẩn hoặc không tồn tại trên hệ thống"; }
            if ($"{w.Request.Url.AbsolutePath}{actionName}" != "/Index") { w.Session.saveError(v); }
            /* Khởi tạo từ IIS bỏ qua */
            try { w.Response.Redirect("/error"); } catch { }
        }

        /* Ghi lại lịch sử khi tạo thread */

        public static string getTsqlInsertHistory(string content, string iduser, string action = "")
        {
            var values = new Dictionary<string, string>();
            values.Add("iduser", $"'{iduser.getValueField()}'");
            values.Add("[action]", $"'{action.getValueField()}'");
            values.Add("content", $"'{content.getValueField()}'");
            values.Add("times", $"'{DateTime.Now:yyyy-MM-dd HH:mm:ss}'");
            return $"insert into sys_history ({string.Join(",", values.Keys)}) values ({string.Join(",", values.Values)})";
        }

        public static string getTsqlInsertHistory(this HttpContext w, string content = "", string action = "", string iduser = "")
        {
            var r = w.Request;
            var values = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(iduser)) { iduser = $"{w.Session[keyS.idUser]}"; }
            values.Add("iduser", $"'{iduser.getValueField()}'");
            if (string.IsNullOrEmpty(action)) { action = r.Url.PathAndQuery; }
            var path = r.Url.PathAndQuery;
            values.Add("[action]", $"'{action.getValueField()}'");
            if (string.IsNullOrEmpty(content))
            {
                if (path.ToLower().StartsWith("/login")) { content = "Đăng nhập"; }
                else
                {
                    var s = new List<string>();
                    foreach (var v in r.Form.AllKeys)
                    {
                        if (v.StartsWith("__")) { continue; }
                        s.Add($"{v}: {r[v]};");
                    }
                    foreach (var v in r.Cookies.AllKeys)
                    {
                        if (v.StartsWith("__")) { continue; }
                        if (v.StartsWith("nv3c_")) { continue; }
                        if (v == ".ASPXAUTH") { continue; }
                        if (v == "ASP.NET_SessionId") { continue; }
                        s.Add($"{v}: {r.Cookies[v].Value};");
                    }
                    if (r.Files.Count > 0) { for (int i = 0; i < r.Files.Count; i++) { s.Add($"{r.Files[i].FileName}: {r.Files[i].ContentLength};"); } }
                    s.Add($"[{r.Url.PathAndQuery}]");
                    content = string.Join(" ", s);
                }
            }
            values.Add("content", $"'{content.getValueField()}'");
            values.Add("times", $"'{DateTime.Now:yyyy-MM-dd HH:mm:ss}'");
            return $"insert into sys_history ({string.Join(",", values.Keys)}) values ({string.Join(",", values.Values)})";
        }

        public static void setHistory(string content = "", string iduser = "", string action = "")
        {
            string tsql;
            if (!string.IsNullOrEmpty(content)) { tsql = getTsqlInsertHistory(content, iduser, action); }
            else { tsql = HttpContext.Current.getTsqlInsertHistory(content, action, iduser); }
            using (var db = getDataSQLite())
            {
                try { db.Execute(tsql); }
                catch (Exception ex) { ex.save(tsql); }
            }
        } 

        public static void setHistoryTaskList(Progressbar p, string iduser = "", string content = "", string requeststring = "")
        {
            string tsql = "";
            var values = new Dictionary<string, string>();
            values.Add("iduser", $"'{iduser}'");
            values.Add("typeinfo", "'tasklist'");
            values.Add("[action]", $"'{p.actionName}.{p.name}'");
            if (string.IsNullOrEmpty(content)) { values.Add("content", $"'Start: {p.timeStart:dd/MM/yyyy HH:mm:ss}; Min: {p.valueMin}; Max: {p.valueMax}; request: {requeststring.getValueField()}'"); }
            else { values.Add("content", $"'{content.getValueField()}'"); }
            values.Add("times", $"'{DateTime.Now:yyyy-MM-dd HH:mm:ss}'");
            tsql = $"insert into sys_history ({string.Join(",", values.Keys)}) values ({string.Join(",", values.Values)})";
            var sqlite = getDataSQLite();
            try { sqlite.Execute(tsql); } 
            catch (Exception ex) { ex.save(tsql); }
            sqlite.Dispose();
        }

        public static void SetSession_Delete()
        {
            var w = HttpContext.Current;
            w.Session[keyS.Message] = "Xóa thành công vào lúc " + DateTime.Now.toVN();
            setHistory();
        } 

        public static void getMenuOptions(this List<string> s, int id, string level)
        {
            level = level.Trim() + "- ";
            //foreach (var v in local.lsMenu.Where(p => p.id2 == id).OrderBy(p => p.postion).ToList())
            //{
            //    s.Add("<option value=\"{0}\">{1}</option>", v.id, level + v.name);
            //    getMenuOptions(s, v.id, level);
            //}
        }

        public static string bootstrap_alert_warning(string msg) => $"<div class=\"alert alert-warning\">{msg}</div>";

        public static string bootstrap_alert_info(string msg) => $"<div class=\"alert alert-info\">{msg}</div>";

        public static List<string> getList(this string s, char space = '\n') => s.Split(space).ToList();

        /// <summary>
        /// Thời gian quá hạn: 3 giờ; True = ""; False = Error Message
        /// </summary>
        /// <param name="r"></param>
        /// <param name="RequestKey"></param>
        /// <param name="HoursOut"></param>
        /// <returns></returns>
        public static string isTimeout(this HttpRequestBase r, string RequestKey = "t", int HoursOut = 3)
        {
            try
            {
                string t = r[RequestKey] == null ? "N/A" : r[RequestKey].Trim();
                DateTime timeclient = (DateTime.FromOADate(double.Parse(t))).AddHours(HoursOut);
                if (timeclient >= DateTime.Now && timeclient <= DateTime.Now.AddHours(21)) { return ""; }
            }
            catch { return messageKey.thamSoKhongDung; }
            return "Thời gian thao tác quá hạn. Vui lòng nhấn F5 để làm mới trang";
        }

        public static string getOptions(string table, string fieldvalue, string fielddisplay, string value = "", string where = "", bool all = true, string defselect = "")
        {
            using (var db = local.getDataObject())
            {
                return db.getOptions(table, fieldvalue, fielddisplay, value, where, all, defselect);
            }
        }
        public static string getOptionsTinh(this Models.SQLServerDataContext db, string value = "", string where = "", bool all = true, string defValueSelect = "", bool dangKyTaiKhoan = false)
        {
            var Server = HttpContext.Current.Server;
            var op = new List<string>();
            if (all) { op.Add($"<option value=\"{defValueSelect}\"> -- Chọn -- </option>"); }
            string tsql = dangKyTaiKhoan ? $"select id,ten from dmtinh where id in (select idtinh from nguoidung group by idtinh) {(where == "" ? "" : $" and {where}")} order by ten" : $"select id,ten from dmtinh {(where == "" ? "" : $" where {where}")} order by ten";
            try
            {
                var dt = db.getDataSet(tsql).Tables[0];
                if (string.IsNullOrEmpty(value))
                {
                    foreach (DataRow r in dt.Rows) { op.Add($"<option value=\"{Server.HtmlEncode($"{r[0]}")}\"> {Server.HtmlEncode($"{r[1]}")} </option>"); }
                }
                else
                {
                    foreach (DataRow r in dt.Rows) { op.Add($"<option value=\"{Server.HtmlEncode($"{r[0]}")}\" {($"{r[0]}" == value ? "selected=\"selected\"" : "")}> {Server.HtmlEncode($"{r[1]}")} </option>"); }
                }
            }
            catch { }
            return string.Join("", op);
        }
        public static string getOptions(this Models.SQLServerDataContext db, string table, string fieldvalue, string fielddisplay, string value = "", string where = "", bool all = true, string defValueSelect = "")
        {
            var Server = HttpContext.Current.Server;
            var op = new List<string>();
            if (all) { op.Add($"<option value=\"{defValueSelect}\"> -- Chọn -- </option>"); }
            string tsql = $"select {fieldvalue},{fielddisplay} from {table} {(where == "" ? "" : $" where {where}")} order by {fielddisplay}";
            try
            {
                var dt = db.getDataSet(tsql).Tables[0];
                if (string.IsNullOrEmpty(value))
                {
                    foreach (DataRow r in dt.Rows) { op.Add($"<option value=\"{Server.HtmlEncode($"{r[0]}")}\"> {Server.HtmlEncode($"{r[1]}")} </option>"); }
                }
                else
                {
                    foreach (DataRow r in dt.Rows) { op.Add($"<option value=\"{Server.HtmlEncode($"{r[0]}")}\" {($"{r[0]}" == value ? "selected=\"selected\"" : "")}> {Server.HtmlEncode($"{r[1]}")} </option>"); }
                }
            }
            catch { }
            return string.Join("", op);
        }

        public static string thongbaoMarquee(HttpContextBase w)
        {
            var s = new List<string>();
            var iduser = w.Session.getIdUser();
            using (var db = getDataObject())
            {
                try
                {
                    var time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    var q = db.thongbaos.Where(p => p.hansudung >= time && (p.den == iduser || p.den == ""))
                        .Select(p => new { p.id, p.tu, p.times, p.noidung, dodai = p.chitiet.Length }).OrderByDescending(p => p.times).Take(20);
                    if ($"{w.Session[keyS.capdo]}" == "0")
                    {
                        q = db.thongbaos.Where(p => p.hansudung >= time)
                          .Select(p => new { p.id, p.tu, p.times, p.noidung, dodai = p.chitiet.Length }).OrderByDescending(p => p.times).Take(20);
                    }
                    var d = q.ToList();
                    foreach (var v in d)
                    {
                        s.Add($"<a href=\"javascript:showgeturl('/home/thongbao/{v.id}?layout=null');\"> {((v.dodai > 30 && v.noidung.StartsWith("Import ")) ? "<i class=\"fa fa-warning\"></i>" : "")} <i>{v.times:dd/MM/yyyy}-{v.tu}</i> {v.noidung}</a>; ");
                    }
                }
                catch (Exception ex) { ex.save(); }
            }
            if (s.Count == 0) { return ""; }
            return $"<div class=\"form-group\"> <div class=\"alert alert-dark\"> <marquee id=\"newstatus\" scrollamount=\"3\" onmouseover=\"this.stop();\" onmouseout=\"this.start();\"> {string.Join("", s)} </marquee> </div></div>";
        } 
    } 
    public class itemStringInt
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }
    public class itemKeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}