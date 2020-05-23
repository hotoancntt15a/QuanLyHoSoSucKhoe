﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using zModules;
using zModules.AspNet;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Areas.sys.Controllers
{
    [Properties(Name = "Tài khoản", Note = "fa fa-users")]
    public class taikhoanController : Controller
    { 
        /* GET: hethong/taikhoan */
        [CheckLogin]
        [Properties(Name = "Tài khoản", Note = "fa fa-users")]
        public ActionResult index()
        {
            ViewBag.tabledescription = "nguoidung";
            using (var db = local.getDataObject())
            {
                try { ViewBag.dmtinh = db.dmtinhs.OrderBy(p => p.ten).ToList(); }
                catch (Exception ex) {
                    Session.saveError(ex.saveMessage());
                    ViewBag.dmtinh = new List<Models.dmtinh>();
                }
            }
            return View();
        }

        [CheckLogin]
        public ActionResult search()
        {
            string table = "nguoidung";
            var w = new List<string>();
            try
            {
                /* Get Request */
                string idtinh = Request.getValue("tinh");
                string ngay1 = Request.getValue("ngay1");
                string ngay2 = Request.getValue("ngay2");
                string tendangnhap = Request.getValue("taikhoan");
                string sdt = Request.getValue("sdt");
                /* Buil TSQL */
                if (string.IsNullOrEmpty(idtinh) == false) { w.Add($"idtinh=N'{idtinh.getValueField()}'"); }
                if (string.IsNullOrEmpty(ngay1) == false)
                {
                    var obj = "ngaytao".buildLikeSqlDate(ngay1, ngay2);
                    w.Add(obj[0]);
                }
                if (string.IsNullOrEmpty(sdt) == false) { w.Add(sdt.buildLikeSql("sdt")); }
                if (string.IsNullOrEmpty(tendangnhap) == false) { w.Add(tendangnhap.buildLikeSql("tendangnhap")); }
            }
            catch (Exception ex) { return Content(ex.saveMessage()); }
            using(var db = local.getDataObject())
            {
                try {
                    string tsql = $"select top (100) * from {table} " + (w.Count > 0 ? "where " + string.Join(" or ", w) : "");
                    var dt = db.getDataSet(tsql).Tables[0];
                    dt.getTableDescription(Folders.App_Data + "\\" + table + ".xml");
                    ViewBag.data = dt;
                }
                catch(Exception ex) { return Content(ex.saveMessage()); }
            }
            return View();
        }

        [CheckLogin]
        public ActionResult update()
        {
            var t = Request.isTimeout();
            if (string.IsNullOrEmpty(t) == false)
            {
                Session[keyS.Error] = t;
                return RedirectToAction("index");
            }
            /* update */
            string id = Request.getValue("id");
            string iduser = Request.getValue("taikhoan");
            var user = new Dictionary<string, string>();
            var db = local.getDataObject();
            if (Request["taikhoan"] != null)
            {
                while (true)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(iduser)) { throw new Exception("Tài khoản để trống"); }
                        string idtinh = Request.getValue("idtinh");
                        var reg = new Regex("^[0-9]+$");
                        if(reg.IsMatch(idtinh) == false) { throw new Exception("Chưa chọn tỉnh"); }
                        string pass = Request.getValue("matkhau");
                        if (string.IsNullOrEmpty(id))
                        {
                            /* Add New */
                            user.Add("tendangnhap", iduser);
                            if (string.IsNullOrEmpty(pass)) { throw new Exception("Mật khẩu để trống"); }
                            user.Add("ngaytao", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else
                        {
                            /* Edit */
                            if (iduser != id) { throw new Exception("Cập nhật thông tin có mã không chính xác"); }
                            if (db.nguoidungs.Any(p => p.tendangnhap == iduser) == false) { throw new Exception(messageKey.notFoundRecord); }
                        }
                        /* [tendangnhap] ,[matkhau] ,[sdt] ,[capdo] ,[kichhoat] ,[ngaytao] ,[lancuoi] ,[ghichu] ,[idhuyen] ,[idxa] ,[bhhuyen] */
                        if (string.IsNullOrEmpty(pass) == false) { user.Add("matkhau", pass.toPassword()); }
                        string sdt = Request.getValue("sdt");
                        if (string.IsNullOrEmpty(sdt) == false)
                        {
                            /* Kiểm tra số điện thoại đã sử dụng chưa */
                            if (db.nguoidungs.Any(p => p.sdt == sdt && p.tendangnhap != iduser))
                            {
                                throw new Exception("Số điện thoại đã được sử dụng. Vui lòng sử dụng số điện thoại khác hoặc liên hệ với quản trị viên");
                            }
                            if (db.nguoidungs.Any(p => p.tendangnhap == sdt && p.tendangnhap != iduser))
                            {
                                throw new Exception("Tên đăng nhập trùng với số điện thoại của tài khoản khác.");
                            }
                        }
                        user.Add("sdt", sdt);
                        string capdo = Request.getValue("capdo");
                        user.Add("capdo", capdo);
                        user.Add("kichhoat", Request.getValue("kichhoat"));
                        user.Add("ghichu", Request.getValue("ghichu"));
                        string idxa = Request.getValue("idxa");
                        if (capdo == "3" && string.IsNullOrEmpty(idxa)) { throw new Exception("Chưa chọn xã"); }
                        user.Add("idxa", idxa);
                        string idhuyen = Request.getValue("idhuyen");
                        if (capdo == "2" && string.IsNullOrEmpty(idhuyen)) { throw new Exception("Chưa chọn Huyện"); }
                        user.Add("idhuyen", idhuyen);
                        string bhhuyen = Request.getValue("bhhuyen");
                        if (string.IsNullOrEmpty(bhhuyen)) { throw new Exception("Chưa chọn bảo hiểm thuộc huyện"); }
                        user.Add("bhhuyen", bhhuyen);
                        user.Add("idtinh", idtinh);
                    }
                    catch (Exception ex) { ViewBag.Error = ex.Message; break; }
                    try
                    {
                        db.saveTaiKhoan(user, id);
                        db.setHistory();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Cannot insert duplicate key") == false) { ViewBag.Error = ex.saveMessage(); }
                        else { ViewBag.Error = $"Tên đăng nhập '{iduser}' đã tồn tại trên hệ thống"; }
                    }
                    if (ViewBag.Error == null)
                    {
                        db.Dispose();
                        Session.saveMessage(messageKey.actionSuccess);
                        return RedirectToAction("index");
                    }
                    break;
                }
            }
            var item = new Models.nguoidung();
            try
            {
                if (string.IsNullOrEmpty(id) == false)
                {
                    item = db.nguoidungs.FirstOrDefault(p => p.tendangnhap == id);
                    if (item == null) { throw new Exception(messageKey.notFoundRecord); }
                    ViewBag.flagedit = "readonly=\"readonly\"";
                }
                else
                {
                    item.bhhuyen = "";
                    item.idxa = "";
                    item.idhuyen = "";
                }
                ViewBag.optiontinh = db.getOptions("dmtinh", "id", "ten", $"{item.idtinh}", defValueSelect: "select"); 
                ViewBag.optionhuyen = db.getOptions("dmhuyen", "id", "ten", $"{item.idhuyen}", $"idtinh='{item.idtinh}'", defValueSelect: "select");
                ViewBag.optionxa = db.getOptions("dmxa", "id", "ten", $"{item.idxa}", $"idhuyen='{item.idhuyen.getValueField()}'");
                ViewBag.optionbhhuyen = db.getOptions("dmcqbh", "id", "ten", $"{item.bhhuyen}", $"idtinh='{item.idtinh}'", all: false);
            }
            catch (Exception ex) { ViewBag.Error = ex.saveMessage(); }
            ViewBag.data = item;
            db.Dispose();
            return View();
        }
    }
}