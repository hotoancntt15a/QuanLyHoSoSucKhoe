using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
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
            var regDate = new Regex("^[0-3][0-9]/[0-1][0-9]/[1-9][0-9]{3}$");
            var item = new Dictionary<string, string>();
            var tsqlNgheCV = new List<string>();
            var tsqlBenh = new List<string>(); 
            try
            {
                var fields = "id,ngay,pic,hoten,gioitinh,ngaysinh,cmndhochieu,capngay,noicap,hokhauthuongtru,noiohientai,nghenghiep,noicongtac,ngaycongtachientai,tiensubenhcuagiadinh,times,iduser,iduser2".Split(',');
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
                if(item["ngay"] == "") { item["ngay"] = $"{DateTime.Now:yyyy-MM-dd}"; }
                else {
                    var time = item["ngay"].vnToDateTime();
                    if(time == null) { throw new Exception($"0 Ngày lập sổ không đúng định dạng {item["ngay"]}"); }
                    if(time.HasValue == false) { throw new Exception($"0 Ngày lập sổ không đúng định dạng {item["ngay"]}"); }
                    item["ngay"] = time.Value.ToString("yyyy-MM-dd");
                }
                if (regDate.IsMatch(item["ngaysinh"]) == false) { throw new Exception($"2.2 Ngày sinh không đúng định dạng {item["ngaysinh"]}"); }
                if (item["capngay"] != "")
                {
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

                /* Lấy danh sách nghề, công việc */
                var listNgay1 = Request.Form.GetValues("cvngay1");
                var listNgay2 = Request.Form.GetValues("cvngay1");
                var listCVBenh = Request.Form.GetValues("cvngay1");
                for (var i = 0; i < listNgay1.Count(); i++)
                {
                    if (string.IsNullOrEmpty(listNgay1[i])) { break; }
                    if (regDate.IsMatch(listNgay1[i]) == false) { throw new Exception($"9. Nghề, công việc trước đây - Từ ngày không đúng: {listNgay1[i]} - {listNgay2[i]} - {listCVBenh[i]}"); }
                    if (string.IsNullOrEmpty(listNgay2[i]) == false) { throw new Exception($"9. Nghề, công việc trước đây - đến ngày không đúng: {listNgay1[i]} - {listNgay2[i]} - {listCVBenh[i]}"); }
                    if (string.IsNullOrEmpty(listCVBenh[i])) { throw new Exception($"9. Nghề, công việc trước đây - Nghề, Công việc bỏ trống: {listNgay1[i]} - {listNgay2[i]} - {listCVBenh[i]}"); }
                    tsqlNgheCV.Add($"(N'{item["id"]}-{i}',N'{item["id"]}',N'{listNgay1[i].getValueField()}', N'{listNgay2[i].getValueField()}', N'{listCVBenh[i].getValueField()}')");
                }
                /* Lấy tiểu sử bệnh */
                listNgay1 = Request.Form.GetValues("tsnambenh");
                listCVBenh = Request.Form.GetValues("tsbenh");
                for (var i = 0; i < listNgay1.Count(); i++)
                {
                    if (string.IsNullOrEmpty(listNgay1[i])) { break; }
                    if (Regex.IsMatch(listNgay1[i], "^[1-9][0-9]{3}$") == false) { throw new Exception($"11.1 Tiền sử bệnh - Năm không đúng: {listNgay1[i]} - {listCVBenh[i]}"); }
                    if (string.IsNullOrEmpty(listCVBenh[i])) { throw new Exception($"11.1 Tiền sử bệnh - Bệnh bỏ trống: {listNgay1[i]} - {listCVBenh[i]}"); }
                    /* Loai: 1 - Tiểu sử bệnh; 2- Tiểu sử bệnh nghề nghiệp */
                    tsqlBenh.Add($"(N'{item["id"]}-1{i}',N'{item["id"]}',1,N'{listNgay1[i].getValueField()}', N'{listCVBenh[i].getValueField()}')");
                }
                /* Lấy tiểu sử bệnh nghề nghiệp */
                listNgay1 = Request.Form.GetValues("tsnambenhnghe");
                listCVBenh = Request.Form.GetValues("tsbenhnghe");
                for (var i = 0; i < listNgay1.Count(); i++)
                {
                    if (string.IsNullOrEmpty(listNgay1[i])) { break; }
                    if (Regex.IsMatch(listNgay1[i], "^[1-9][0-9]{3}$") == false) { throw new Exception($"11.2 Tiền sử bệnh nghề nghiệp - Năm không đúng: {listNgay1[i]} - {listCVBenh[i]}"); }
                    if (string.IsNullOrEmpty(listCVBenh[i])) { throw new Exception($"11.2 Tiền sử bệnh nghề nghiệp - Bệnh bỏ trống: {listNgay1[i]} - {listCVBenh[i]}"); }
                    /* Loai: 1 - Tiểu sử bệnh; 2- Tiểu sử bệnh nghề nghiệp */
                    tsqlBenh.Add($"(N'{item["id"]}-2{i}',N'{item["id"]}',2,N'{listNgay1[i].getValueField()}', N'{listCVBenh[i].getValueField()}')");
                }
            }
            catch (Exception ex) { return Content($"<div class=\"alert alert-danger\">{ex.saveMessage()}</div>"); }
            tmp = "";
            using (var db = local.getDataObject())
            {
                try
                {  
                    db.Execute(item, "hoso", id == "" ? "" : $"id=N'{id.getValueField()}'");
                    db.ExecuteCommand($"delete from hosotsbenh where idhoso=N'{item["id"]}'; delete from hosotscongviec where idhoso=N'{item["id"]}'; ");
                    var tsql = new List<string>();
                    if (tsqlNgheCV.Count > 0) { tsql.Add($"insert into hosotscongviec (id,idhoso,ngay1,ngay2,congviec) values {tsqlNgheCV.ToJoin(",")}; "); }
                    if (tsqlBenh.Count > 0) { tsql.Add($"insert into hosotsbenh (id,idhoso,loai,nam,tenbenh) values {tsqlBenh.ToJoin(",")}; "); }
                    if (tsql.Count > 0) { db.ExecuteCommand(tsql.ToJoin(" ")); }
                }
                catch (Exception ex) { tmp = ex.saveMessage(); }
            }
            if (tmp == "") { return Content($"{item.getTsqlSQLServerUpdate("hoso", id == "" ? "" : $"id=N'{id}'")} <br /> {Request.showRequest()}"); }
            return Content($"<div class=\"alert alert-danger\">{tmp} <br />{item.getTsqlSQLServerUpdate("hoso", id == "" ? "" : $"id=N'{id}'")} <br /> {Request.showRequest()}</div>");
        }
    }
}