using NPOI.XSSF.Extractor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.UI.WebControls;
using zModules;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Controllers
{
    public class HoSoController : Controller
    {
        // GET: HoSo
        [Properties(Name = "Bác sỹ", Note = "fa fa-list")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult update()
        {
            return View();
        }

        public ActionResult save()
        {
            string tmp = "", id = "";
            var item = new Dictionary<string, string>();
            try
            { 
                var fields = "id,pic,hoten,gioitinh,ngaysinh,cmndhochieu,capngay,noicap,hokhauthuongtru,noiohientai,nghenghiep,noicongtac,ngaycongtachientai,tiensubenhcuagiadinh,times,iduser,iduser2".Split(',');
                foreach (var v in fields) { item[v] = Request.getValue(v); }
                id = item["id"];
                tmp = Request.getValue("pic");
                if (Regex.IsMatch(item["pic"], "^/temp/img", RegexOptions.IgnoreCase) == false) { item.Remove("pic"); }
                /* Kiêm tra trường trống */
                if (item["hoten"] == "") { throw new Exception("1. Họ tên bỏ trống"); }
                if (item["ngaysinh"] == "") { throw new Exception("2.2 Ngày sinh bỏ trống"); }
                if (item["hokhauthuongtru"] == "") { throw new Exception("4. Hộ khẩu thường trú bỏ trống"); }
                if (item["noiohientai"] == "") { throw new Exception("5. Nơi ở hiện tại bỏ trống"); }
                /* Kiểm tra quy tắc */
                var regDate = new Regex("^[0-3][0-9]/[0-1][0-9]/[1-9][0-9]{3}$");
                if(regDate.IsMatch(item["ngaysinh"]) == false) { throw new Exception($"2.2 Ngày sinh không đúng định dạng {item["ngaysinh"]}"); }
                if (item["capngay"] != "") {
                    if (regDate.IsMatch(item["capngay"]) == false) { throw new Exception($"3.2 Ngày cấp không đúng định dạng {item["capngay"]}"); }
                }
                if (item["ngaycongtachientai"] != "")
                {
                    if (regDate.IsMatch(item["ngaycongtachientai"]) == false) { throw new Exception($"8.Ngày bắt đầu vào học/làm việc tại đơn vị hiện nay không đúng định dạng {item["ngaycongtachientai"]}"); }
                } 
                if (item["id"] != "") { id = item["id"]; } else { item["id"] = Tools.getTimeSpanCurrent().ToString(); }
                /* Soát danh sách */ 
                item.Remove("times");
                if (id != "") { item["iduser2"] = $"{Session["iduser"]}"; item.Remove("iduser"); } 
                else { item["iduser"] = $"{Session["iduser"]}"; }
            }
            catch (Exception ex) { return Content($"<div class=\"alert alert-danger\">{ex.saveMessage()}</div>"); }
            tmp = "";
            using (var db = local.getDataObject())
            {
                try
                {
                    db.Execute(item, "hoso", id == "" ? "" : $"id=N'{id.getValueField()}'");
                }
                catch (Exception ex) { tmp = ex.saveMessage(); }
            }
            if (tmp == "") { return Content($"{item.getTsqlSQLServerUpdate("hoso", id == "" ? "" : $"id=N'{id}'")} <br /> {Request.showRequest()}"); }
            return Content($"<div class=\"alert alert-danger\">{tmp} <br />{item.getTsqlSQLServerUpdate("hoso", id == "" ? "" : $"id=N'{id}'")} <br /> {Request.showRequest()}</div>");
        }
    }
}