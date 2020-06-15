using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using zModules;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe
{
    public class LoginInfo
    {
        public LoginInfo(string iduser = "", string password = "", bool remmember = false)
        {
            idUser = iduser;
            this.password = password;
            this.remmember = remmember;
        }

        public string idUser = "";
        public string password = "";
        public bool remmember = false;
    }
    public static class buildDatabase
    {
        public static void saveTaiKhoan(this Models.SQLServerDataContext db, Dictionary<string, string> data, string iduser = "")
        {
            if (data.ContainsKey("sdt")) { if (data["sdt"].ToLower() == "administrator") { throw new Exception("Tài khoản 'administrator' không thể thao tác"); } }
            if (data.ContainsKey("tendangnhap"))
            {
                if (data["tendangnhap"].ToLower() == "administrator") { throw new Exception("Tài khoản 'administrator' không thể thao tác"); }
                if (Regex.IsMatch(data["tendangnhap"], "[/\\:*?\"<>|]+")) { throw new Exception("Tên đăng nhập không hợp lệ (Không chứa các ký tự /\\:*?\"<>|);"); }
            }
            if (iduser.ToLower() == "administrator") { throw new Exception("Tài khoản 'administrator' không thể thao tác"); }

            var val1 = new List<string>();
            var tsql = "";
            if (string.IsNullOrEmpty(iduser))
            {
                /* Add New */
                foreach (var v in data) { val1.Add(v.Value.getValueField()); }
                tsql = $"insert into nguoidung ({string.Join(",", data.Keys)}) values (N'{string.Join("',N'", val1)}')";
            }
            else
            {
                /* Edit */
                foreach (var v in data) { val1.Add($"{v.Key}=N'{v.Value.getValueField()}'"); }
                tsql = $"update nguoidung set {string.Join(",", val1)} where tendangnhap=N'{iduser.getValueField()}'";
            }
            db.Execute(tsql);
        }
        public static void createThongBao(this Models.SQLServerDataContext db, string fromIdUser, string toIdUser, string noidung, string chitiet, DateTime? hansudung = null)
        {
            try
            {
                var item = new Models.thongbao();
                item.chitiet = chitiet;
                item.dadoc = 0;
                item.den = toIdUser;
                item.hansudung = hansudung.HasValue ? hansudung.Value : DateTime.Now;
                item.noidung = noidung;
                item.times = DateTime.Now;
                item.tu = fromIdUser;
                db.thongbaos.InsertOnSubmit(item);
                db.SubmitChanges();
            }
            catch (Exception ex) { ex.save(); }
        }
        public static string createDatabase()
        {
            var timestring = $"{DateTime.Now:yyMMddHHmmss}";
            var sqlite = local.getDataSQLite();
            var db = local.getDataObject();
            var s = new List<string>(); 
            var tsql = new List<string>();
            var msgerr = "";
            try
            {
                /* Microsfot SQL Server 
                 - TABLLE: Select TABLE_NAME from INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'
                 - VIEW: Select TABLE_NAME from INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='VIEW'
                 */
                var tables = db.ExecuteQuery<string>("select TABLE_NAME from INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'").ToList();
                db.checkTableSystem(tables); 
                /* Danh mục */
                if (tables.Contains("dmdantoc") == false) { tsql.Add("CREATE TABLE [dmdantoc] ([id] nvarchar(5) NOT NULL PRIMARY KEY, [ten] nvarchar(50) NOT NULL DEFAULT '', [tenkhac] nvarchar(255) not null DEFAULT '');"); }
                if (tables.Contains("dmtinh") == false) { tsql.Add("create table dmtinh (id nvarchar(10) not null primary key, ten nvarchar(255) not null default '');"); }
                if (tables.Contains("dmhuyen") == false) { tsql.Add("create table dmhuyen (id nvarchar(10) not null primary key, ten nvarchar(255) not null default '', idtinh nvarchar(10) not null default '');"); }
                if (tables.Contains("dmxa") == false) { tsql.Add($"create table dmxa (id nvarchar(10) not null primary key, ten nvarchar(255) not null default '', idhuyen nvarchar(10) not null default '');"); } 
                if (tables.Contains("dmcqbh") == false) { tsql.Add("create table dmcqbh (id nvarchar(10) not null primary key, ten nvarchar(255) not null default '', idtinh nvarchar(10) not null default '');"); }
                if (tables.Contains("dmbenhvien") == false) { tsql.Add("create table dmbenhvien (id nvarchar(20) not null primary key, matinh nvarchar(10) not null default '', mabv nvarchar(10) not null default '', ten nvarchar(255) not null default '');"); }
                if (tables.Contains("dmmoiquanhe") == false) { tsql.Add("create table dmmoiquanhe (id nvarchar(10) not null primary key, quanhe nvarchar(50) not null default '', viettat nvarchar(50) not null default '');"); }
                /* Dữ liệu hệ thống */
                if (tables.Contains("thongbao") == false)
                {
                    /* Từ ai, đến ai, thời gian đăng, hạn sử dụng đến hết, đã đọc chưa, nội dung thông báo, chi tiết thông báo*/
                    tsql.Add("create table thongbao (id integer NOT NULL PRIMARY KEY IDENTITY(1,1), tu nvarchar(50) not null default '', den nvarchar(50) not null default '', times datetime not null, hansudung datetime, dadoc int not null default 0, noidung nvarchar(255) not null default '', chitiet nvarchar(max) not null default '');");
                    tsql.Add($" create index thongbao{timestring} on thongbao(tu,den,dadoc,times,hansudung,noidung);");
                }
                /* Danh mục khoa */
                if (tables.Contains("dmkhoa") == false)
                {
                    tsql.Add("create table dmkhoa (id nvarchar(20) not null primary key, khoa nvarchar(50) not null, ghichu nvarchar(255) not null default '');");
                }
                /* Bác sĩ, nhân viên */
                if (tables.Contains("bacsy") == false)
                {
                    tsql.Add("create table bacsy (id nvarchar(20) not null primary key, " +
                        "hoten nvarchar(100) not null, ngaysinh varchar(10) not null default '', gioitinh nvarchar(10) not null default '', idkhoa nvarchar(20) not null default '', ghichu nvarchar(255) not null default '');");
                }
                /* Bệnh nhân */
                if (tables.Contains("benhnhan") == false)
                {
                    tsql.Add("create table benhnhan (id nvarchar(20) not null primary key, " +
                        "hoten nvarchar(100) not null, ngaysinh varchar(10) not null default '', gioitinh nvarchar(10) not null default '', sobhxh nvarchar(20) not null default '', diachi nvarchar(255) not null default '', "+
                        "socmnd nvarchar(20) not null default '', ngaycap nvarchar(10) not null default '', noicap nvarchar(255) not null default '', ngaytao datetime not null);");
                }
                /* Đơn vị, cơ quan */
                if (tables.Contains("donvi") == false)
                {
                    tsql.Add("create table donvi (id nvarchar(20) not null primary key, " +
                        "ten nvarchar(100) not null, mst varchar(20) not null default '', diachi nvarchar(255) not null default '', sdt nvarchar(50) not null default '', " +
                        "daidien nvarchar(50) not null default '', ghichu nvarchar(255) not null default '');");
                }
                /* Hồ sơ khám */
                if (tables.Contains("hosokham") == false)
                {
                    tsql.Add("create table hosokham(id nvarchar(20) not null primary key, ngaytao datetime not null, " +
                        "thuongtru nvarchar(255) not null default '', diachi nvarchar(255) not null default '', dienthoai nvarchar(50) not null default '', " +
                        "nghenghiep nvarchar(255) not null default '', noicongtac nvarchar(255) not null, ngaylamviec datetime not null, " +
                        "tiensubenhgiadinh nvarchar(500) not null default '', tieusubenh nvarchar(500) not null default '', " +
                        "chieucao float not null default 0, cannang float not null default 0, bmi float not null default 0, " +
                        "mach float not null default 0, huyetap1 float not null default 0, huyetap2 float not null default 0, " +
                        "pltheluc nvarchar(50) not null default '');");
                }
                if (tables.Contains("dotkham") == false)
                {
                    tsql.Add("create table dotkham(id nvarchar(20) not null primary key, noidung nvarchar(255) not null, donvi nvarchar(255) not null default '', thoigian datetime not null, muckham nvarchar(1000) not null default '');");
                }
                if (tables.Contains("ketquakham") == false)
                {
                    tsql.Add("create table ketquakham(id nvarchar(20) not null primary key, idhoso nvarchar(50) not null, iddot nvarchar(50) not null default '', thoigian datetime not null, chieucao float not null default 0, " +
                        "cannang float not null default 0, tinhtrang nvarchar(50) null default '', phanloai nvarchar(50) not null default '', ketluan nvarchar(500) not null default '');");
                }
                if (tables.Contains("khammat") == false)
                {
                    tsql.Add("create table khammat(id nvarchar(20) not null primary key, idhoso nvarchar(50) not null, iddot nvarchar(50) not null default '', thoigian datetime not null, " +
                        "phai nvarchar(50) not null default '',trai nvarchar(50) not null default '',phaikinh nvarchar(50) not null default '',traikinh nvarchar(50) not null default '', " +
                        "benh nvarchar(255) not null default '', phanloai nvarchar(50) not null default '');");
                }
                if (tables.Contains("khamtaimuimat") == false)
                {
                    tsql.Add("create table khamtaimuimat(id nvarchar(20) not null primary key, idhoso nvarchar(50) not null, iddot nvarchar(50) not null default '', thoigian datetime not null, " +
                        "taiphai nvarchar(50) not null default '',taitrai nvarchar(50) not null default '',taiphaitham nvarchar(50) not null default '',taitraitham nvarchar(50) not null default '', " +
                        "benhvemui nvarchar(255) not null default '', phanloai nvarchar(50) not null default '');");
                }
                if (tables.Contains("khamrangmieng") == false)
                {
                    tsql.Add("create table khamrangmieng(id nvarchar(20) not null primary key, idhoso nvarchar(50) not null, iddot nvarchar(50) not null default '', thoigian datetime not null, " +
                        "hamtren nvarchar(255) not null default '',hamduoi nvarchar(255) not null default ''," +
                        "benh nvarchar(255) not null default '', phanloai nvarchar(50) not null default '');");
                }
                if (tables.Contains("khamdalieu") == false)
                {
                    tsql.Add("create table khamdalieu(id nvarchar(20) not null primary key, idhoso nvarchar(50) not null, iddot nvarchar(50) not null default '', thoigian datetime not null, " +
                        "benh nvarchar(255) not null default '', phanloai nvarchar(50) not null default '');");
                }
                if (tables.Contains("khamnoikhoa") == false)
                {
                    tsql.Add("create table khamnoikhoa(id nvarchar(20) not null primary key, idhoso nvarchar(50) not null, iddot nvarchar(50) not null default '', thoigian datetime not null, " +
                        "tuanhoan nvarchar(255) not null default '',pltuanhoan nvarchar(50) not null default ''," +
                        "hohap nvarchar(255) not null default '',plhohap nvarchar(50) not null default ''," +
                        "tieuhoa nvarchar(255) not null default '',pltieuhoa nvarchar(50) not null default ''," +
                        "thantietnieu nvarchar(255) not null default '',plthantietnieu nvarchar(50) not null default ''," +
                        "coxuongkhop nvarchar(255) not null default '',plcoxuongkhop nvarchar(50) not null default ''," +
                        "thankinh nvarchar(255) not null default '',plthankinh nvarchar(50) not null default ''," +
                        "tamthan nvarchar(255) not null default '',pltamthan nvarchar(50) not null default '');");
                }
                if (tables.Contains("khamngoaikhoa") == false)
                {
                    tsql.Add("create table khamngoaikhoa(id nvarchar(20) not null primary key, idhoso nvarchar(50) not null, iddot nvarchar(50) not null default '', thoigian datetime not null);");
                }
                if (tables.Contains("khamkhac") == false)
                {
                    tsql.Add("create table khamkhac(id nvarchar(20) not null primary key, idhoso nvarchar(50) not null, iddot nvarchar(50) not null default '', thoigian datetime not null);");
                }
                if (tsql.Count > 0) { db.Execute(string.Join(" ", tsql)); }
                tsql = new List<string>();
                /* Check Product */
                /* Check Function */
                /* Check Data Default */
                /* Drop table temp */
                var tmptable = tables.Where(p => p.EndsWith("temp")).Select(p => p).ToList();
                if (tmptable.Count > 0)
                {
                    tsql = new List<string>();
                    foreach (var v in tmptable) { tsql.Add($"drop table {v}; "); }
                    db.Execute(string.Join(" ", tsql));
                }
                db.truncate(keyTable.importData); 
                /* SQLite */
                tables = sqlite.getTableNames(); tsql = new List<string>();
                sqlite.checkTableSystem(tables);
                if(!tables.Contains("taikhoan"))
                {
                    tsql.Add("CREATE TABLE taikhoan (iduser nvarchar(50) NOT NULL PRIMARY KEY, pass nvarchar(50) NOT NULL, hoten nvarchar(255) NOT NULL default '', ngaysinh nvarchar(10) NOT NULL default '', gioitinh nvarchar(10) NOT NULL default '', sdt nvarchar(50) NOT NULL default '', email nvarchar(50) NOT NULL default '', ngaytao datetime NOT NULL, lancuoi datetime, ghichu nvarchar(255) NOT NULL DEFAULT '', idgroup int not null default 0, block int not null default 0, block_lydo nvarchar(255) not null default '');");
                }
                if (tsql.Count > 0) { sqlite.Execute(string.Join(" ", tsql)); } 
                /* Check Data Default */
                if ($"{sqlite.getValue("select * from taikhoan where iduser='admin'")}" == "")
                {
                    /* Create User Default */
                    tsql.Add($"insert into taikhoan (iduser,pass,hoten,sdt,idgroup,ngaytao) values ('admin','{"admin".toPassword()}','Administrator','0914272795',-1,'{DateTime.Now:yyyy-MM-dd}'); ");
                }
                if (tsql.Count > 0) { sqlite.Execute(string.Join(" ", tsql)); }
            }
            catch (Exception ex) { msgerr = ex.saveMessage(string.Join(" ", tsql)); }
            db.Dispose();
            sqlite.Dispose();
            return msgerr;
        }
        private static string getTypeNvarchar(this string listField, int len, string def = "''")
        {
            if (string.IsNullOrEmpty(listField)) { return ""; }
            var obj = listField.Replace(" ", "").Split(',').ToList();
            return string.Join($" nvarchar({len}) not null default {def},", obj) + $" nvarchar({len}) not null default {def}";
        }
        public static string getTypeNvarchar(this List<string> listField, Dictionary<string, int> regexPatternSetLenght)
        {
            if (listField == null) { return ""; }
            if (listField.Count == 0) { return ""; }
            var d = new Dictionary<string, string>();
            foreach (var v in listField) { d.Add(v, $"[{v}] nvarchar(255) not null default ''"); }
            foreach (var v in regexPatternSetLenght)
            {
                var objs = d.Keys.Where(p => Regex.IsMatch(p, v.Key, RegexOptions.IgnoreCase)).ToList();
                if (objs.Count == 0) { continue; }
                foreach (var k in objs) { d[k] = $"[{k}] nvarchar({v.Value}) not null default ''"; }
            }
            return string.Join(",", d.Values.ToList()).ToLower();
        }
    }
}