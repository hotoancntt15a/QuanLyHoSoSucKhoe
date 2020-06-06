using Newtonsoft.Json; using NPOI.SS.Formula.Functions; using NPOI.XSSF.Extractor;
using System; using System.Collections.Generic; using System.Data; using System.Linq; using System.Text.RegularExpressions; using System.Web; using System.Web.Mvc; using zModules; using zModules.AspNet; using zModules.Crypt;
using zModules.MSSQLServer;  namespace QuanLyHoSoSucKhoe.Areas.sys.Controllers {     [Properties(Name = "Tài khoản", Note = "fa fa-users")]     public class taikhoanController : Controller     {
        /* GET: hethong/taikhoan */
        [CheckLogin]         [Properties(Name = "Tài khoản", Note = "fa fa-users")]         public ActionResult index() => View();          [CheckLogin]         public ActionResult search()         {             string table = "taikhoan"; string tmp = "";             var w = new List<string>();             try             {
                /* Get Request */
                tmp = Request.getValue("iduser"); if (!string.IsNullOrEmpty(tmp)) { w.Add(tmp.sqliteLike("iduser")); }                 tmp = Request.getValue("hoten"); if (!string.IsNullOrEmpty(tmp)) { w.Add(tmp.sqliteLike("hoten")); }                 tmp = Request.getValue("ngaysinh"); if (!string.IsNullOrEmpty(tmp)) { w.Add(tmp.sqliteLike("ngaysinh")); }                 tmp = Request.getValue("sdt"); if (!string.IsNullOrEmpty(tmp)) { w.Add(tmp.sqliteLike("sdt")); }                 tmp = Request.getValue("email"); if (!string.IsNullOrEmpty(tmp)) { w.Add(tmp.sqliteLike("email")); }
                /* Kiểm tra ngày có đúng định dạng hay không? */
                var tmp2 = "ngaytao".sqliteLikeDate(Request.getValue("ngay1"), Request.getValue("ngay2"));                 if (tmp2.Count > 0) { w.Add(tmp2[0]); }             }             catch (Exception ex) { return Content(ex.saveMessage()); }             using (var db = local.getDataSQLite())             {                 try
                {                     tmp = w.Count > 0 ? " where " + string.Join(" and ", w) : "";                     var dt = db.getDataSet($"select * from {table} {w}").Tables[0];                     dt.getTableDescription(Folders.App_Data + "\\" + table + ".xml");                     ViewBag.data = dt;                 }                 catch (Exception ex) { return Content(ex.saveMessage()); }             }             return View();         }          [CheckLogin]         public ActionResult viewupdate() {
            return View();
        }          [CheckLogin]         public ActionResult update()         {
            /* Cập nhật dữ liệu */
            string tmp = "";             using (var sqlite = local.getDataSQLite())             {                 try
                {                     var tmplist = sqlite.getColumnNamnes("taikhoan");                     var data = new Dictionary<string, string>();                     foreach (var v in tmplist) { data.Add(v, Request.getValue(v)); }
                    data.Remove("lancuoi"); data.Remove("block"); data.Remove("block_lydo");
                    /* Kiểm tra dữ liệu đầu vào */
                    if (Regex.IsMatch(data["iduser"], "^[a-zA-Z0-9_]{3,49}$") == false) { throw new Exception("Tên tài khoản không hợp lệ"); }
                    if (string.IsNullOrEmpty(data["hoten"])) { throw new Exception("Họ tên để trống"); }
                    tmp = Request.getValue("mode");
                    if (string.IsNullOrEmpty(tmp)) { throw new Exception(messageKey.thamSoKhongDung); }
                    tmp = tmp.decryptMD5(AppConfig.Key);
                    if (Regex.IsMatch(tmp, "[0-9]{1,2}/[0-9]{1,2}/[0-9]{4}") == false) { throw new Exception(messageKey.thamSoKhongDung); }
                    if (string.IsNullOrEmpty(data["pass"])) { data.Remove("pass"); }
                    else if (data["pass"].Length < 6) { throw new Exception("mật khẩu tối thiểu 6 ký tự; tối đa 50 ký tự"); }
                    else { data["pass"] = data["pass"].encryptMD5(AppConfig.Key).catChuoi(45); }
                    if (string.IsNullOrEmpty(data["idgroup"])) { data.Remove("idgroup"); }
                    /* Xác định thêm mới hay chỉnh lý dữ liệu */
                    if (tmp.StartsWith("1") == false) { tmp = ""; } /* Cập nhật dữ liệu */
                    else { tmp = $"iduser = '{data["iduser"].getValueField()}'"; data.Remove("iduser"); } /* Thêm bản ghi mới */
                    sqlite.Execute(data, "taikhoan", tmp);
                }                 catch (Exception ex) { return Content(ex.saveMessage()); }             }
            return View();         }     } }