﻿using Newtonsoft.Json; using NPOI.SS.Formula.Functions; using NPOI.XSSF.Extractor; using System; using System.Collections.Generic; using System.Data; using System.Linq; using System.Text.RegularExpressions; using System.Web; using System.Web.Mvc; using zModules; using zModules.AspNet; using zModules.Crypt; using zModules.MSSQLServer;  namespace QuanLyHoSoSucKhoe.Areas.sys.Controllers {     [Properties(Name = "Tài khoản", Note = "fa fa-users")]     public class taikhoanController : Controller     {         /* GET: hethong/taikhoan */         [Properties(Name = "Tài khoản", Note = "fa fa-users")]         public ActionResult index() => View();          public ActionResult search()         {             string table = "taikhoan"; string tmp = "";             var w = new List<string>();             try             {                 /* Get Request */                 tmp = Request.getValue("iduser"); if (!string.IsNullOrEmpty(tmp)) { w.Add(tmp.sqliteLike("iduser")); }                 tmp = Request.getValue("hoten"); if (!string.IsNullOrEmpty(tmp)) { w.Add(tmp.sqliteLike("hoten")); }                 tmp = Request.getValue("ngaysinh"); if (!string.IsNullOrEmpty(tmp)) { w.Add(tmp.sqliteLike("ngaysinh")); }                 tmp = Request.getValue("sdt"); if (!string.IsNullOrEmpty(tmp)) { w.Add(tmp.sqliteLike("sdt")); }                 tmp = Request.getValue("email"); if (!string.IsNullOrEmpty(tmp)) { w.Add(tmp.sqliteLike("email")); }                 /* Kiểm tra ngày có đúng định dạng hay không? */                 var tmp2 = "ngaytao".sqliteLikeDate(Request.getValue("ngay1"), Request.getValue("ngay2"));                 if (tmp2.Count > 0) { w.Add(tmp2[0]); }             }             catch (Exception ex) { return Content(ex.saveMessage()); }             using (var sqlite = local.getDataSQLite())             {                 try                 {                     tmp = w.Count > 0 ? " where " + string.Join(" and ", w) : "";                     tmp = $"select * from {table} {tmp}";                     var dt = sqlite.getDataSet(tmp).Tables[0];                     dt.getTableDescription(Folders.App_Data + "\\" + table + ".xml");                     ViewBag.data = dt;                 }                 catch (Exception ex) { return Content(ex.saveMessage(tmp)); }             }             return View();         }          public ActionResult viewupdate()         {             using (var sqlite = local.getDataSQLite())             {                 try                 {                     string id = Request.getValue("id");                     string mode = Request.getValue("mode");                     if (string.IsNullOrEmpty(id)) { mode = "0"; } else { mode = "1"; }                     var data = new Dictionary<string, object>();                     data["mode"] = mode;                     ViewBag.mode = $"{mode}|{DateTime.Now:dd/MM/yyyy HH:mm:ss}".encryptMD5(AppConfig.Key); ;                     var dt = sqlite.getDataSet($"select * from taikhoan where iduser='{id}'").Tables[0];                     if (string.IsNullOrEmpty(id) == false)                     {                         if (dt.Rows.Count == 0) { return Content($"Bản ghi tài khoản '{id}' không tồn tại hoặc đã bị xóa khỏi hệ thống"); }                         foreach (DataColumn c in dt.Columns)                         {                             if (c.DataType != typeof(DateTime)) { data[c.ColumnName] = dt.Rows[0][c.ColumnName]; continue; }                             data[c.ColumnName] = $"{dt.Rows[0][c.ColumnName]:dd/MM/yyyy HH:mm:ss}";                          }                     }                     else { foreach (DataColumn c in dt.Columns) { data[c.ColumnName] = ""; } }                     ViewBag.data = data;                 }                 catch (Exception ex) { return Content(ex.saveMessage()); }             }             return View();         }          public ActionResult update()         {             /* Cập nhật dữ liệu */             using (var sqlite = local.getDataSQLite())             {                 var data = new Dictionary<string, string>();                 try                 {                     var tmplist = sqlite.getColumnNamnes("taikhoan");                     foreach (var v in tmplist) { data.Add(v, Request.getValue(v)); }                     data.Remove("lancuoi"); data.Remove("ngaytao"); data.Remove("block"); data.Remove("block_lydo");                     /* Kiểm tra dữ liệu đầu vào */                     if (Regex.IsMatch(data["iduser"], "^[a-zA-Z0-9_]{3,49}$") == false) { throw new Exception("Tên tài khoản không hợp lệ"); }                     if (string.IsNullOrEmpty(data["hoten"])) { throw new Exception("Họ tên để trống"); }                     var mode = Request.getValue("mode");                     if (string.IsNullOrEmpty(mode)) { throw new Exception(messageKey.thamSoKhongDung); }                     mode = mode.decryptMD5(AppConfig.Key);                     if (Regex.IsMatch(mode, "[0-9]{1,2}/[0-9]{1,2}/[0-9]{4}") == false) { throw new Exception(messageKey.thamSoKhongDung); }                     if (string.IsNullOrEmpty(data["pass"])) { data.Remove("pass"); }                     else if (data["pass"].Length < 6) { throw new Exception("mật khẩu tối thiểu 6 ký tự; tối đa 50 ký tự"); }                     else { data["pass"] = data["pass"].encryptMD5(AppConfig.Key).catChuoi(45); }                     if (string.IsNullOrEmpty(data["idgroup"])) { data.Remove("idgroup"); }                     /* Xác định thêm mới hay chỉnh lý dữ liệu */                     var where = "";                     if (mode.StartsWith("1")) { where = $"iduser = '{data["iduser"].getValueField()}'"; data.Remove("iduser");  } /* Cập nhật dữ liệu */                     else                     {                          /* Thêm bản ghi mới */                         if (data["iduser"].ToLower() == "administrator") { throw new Exception("Tài khoản Administrator không thể thao tác"); }                         data["ngaytao"] = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";                     }                      sqlite.Execute(data, "taikhoan", where);                 }                 catch (Exception ex) { return Content(ex.saveMessage()); }             }             return Content(messageKey.actionSuccess);         }          public ActionResult blockuser()         {             string tsql = "";             try             {                 string iduser = Request.getValue("iduser");                 if (string.IsNullOrEmpty(iduser)) { throw new Exception(messageKey.thamSoKhongDung); }                 string block = Request.getValue("block");                 string lydo = Request.getValue("lydo");                 if(block == "1") { if(string.IsNullOrEmpty(lydo)) { throw new Exception("lý do khóa tài khoản để trống"); } }                 else if(block == "0") { lydo = ""; }                 else { throw new Exception(messageKey.thamSoKhongDung + " block") ; }                 tsql = $"update taikhoan set block='{block}', block_lydo='{lydo.sqliteGetValueField()}' where iduser='{iduser.sqliteGetValueField()}'";              }             catch (Exception ex) { return Content(ex.saveMessage()); }             using (var sqlite = local.getDataSQLite())             {                 try { sqlite.Execute(tsql); }                 catch (Exception ex) { return Content(ex.saveMessage()); }             }             return Content(messageKey.actionSuccess);         }     } }