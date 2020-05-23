using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using zModules;
using zModules.Config;
using zModules.Crypt;

namespace QuanLyHoSoSucKhoe
{

    public static class AppConfig
    {
        public static string PathFileConfig { get; set; } = "";
        public static Dictionary<string, string> Config = new Dictionary<string, string>();
        public static void setValue(string key, string value) { PathFileConfig.setValue(key, value); }
        public static void setValueAppConfig(this string key, string value) => PathFileConfig.setValue(key, value);
        public static void removeAppConfig(this string key) => PathFileConfig.Remove(key);
        public static void LoadConfig()
        {
            PathFileConfig = HttpContext.Current.Server.MapPath("~/App_Data/global.config");
            Config = new Dictionary<string, string>();
            Config.GetFile(PathFileConfig);
        }

        public static void LoadConfig(string pathFileConfig)
        {
            PathFileConfig = pathFileConfig;
            Config = new Dictionary<string, string>();
            Config.GetFile(PathFileConfig);
        }

        public static void rebuildConfig()
        {
            if (string.IsNullOrEmpty(PathFileConfig)) return;
            if (Config != null) { Config.RebuildFile(PathFileConfig); }
            else { if (File.Exists(PathFileConfig)) { Config.GetFile(PathFileConfig); } }
        }

        public static void setValue(string key, object value)
        {
            try
            {
                PathFileConfig.Modify(key, value.ToString());
                LoadConfig();
            }
            catch { }
        }

        public static void remove(string key)
        {
            try
            {
                zModules.Config.Config.RemoveItemKeyValue(PathFileConfig, "", key);
                LoadConfig();
            }
            catch { }
        }

        public static string value(string key) => PathFileConfig.getValue(key);

        public static string value(string key, string valueDefault) => PathFileConfig.getValue(key, valueDefault: valueDefault);

        /*
        public static string this[string key] {
            get { return value(key, "") }
            set { setValue(key, ""); }
        }
        */

        public static int? valueInt(string key)
        {
            try
            {
                var s = PathFileConfig.getValue(key);
                if (s.isNumberInt()) { return int.Parse(s); }
                return null;
            }
            catch (Exception ex) { ex.save(); return null; }
        }

        public static int valueInt(string key, int valueDefault)
        {
            try
            {
                var s = PathFileConfig.getValue(key);
                if (s.isNumberInt()) { return int.Parse(s); }
                return valueDefault;
            }
            catch (Exception ex) { ex.save(); return valueDefault; }
        }

        public static int pageSize
        {
            get { return valueInt("page.size", 25); }
            set { setValue("page.size", value); }
        }
        public static string Title
        {
            get { return value("app.title", "Hồ Sơ Khám Sức Khỏe"); }
            set { setValue("app.title", value); }
        }
        public static string FolderShare
        {
            get { return value("folder.share"); }
            set { setValue("folder.share", value); }
        }
        public static string siteShared
        {
            get { return value("site.shared"); }
            set { setValue("site.shared", value); }
        }

        public static string Description
        {
            get { return value("app.description"); }
            set { setValue("app.description", value); }
        }

        public static string dbMssql
        {
            get { return value("db.mssql", "").decryptMD5(local.projectName); }
            set
            {
                setValue("db.mssql", value.encryptMD5(local.projectName));
                Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                ConnectionStringsSection section = config.GetSection("SQLServerConnectionString") as ConnectionStringsSection;
                if (section != null)
                {
                    section.ConnectionStrings["SQLServerConnectionString"].ConnectionString = value;
                    config.Save();
                }
            }
        }

        public static string Key
        {
            get { return value("app.key", "07654a26-aed2-41db-8575-2bca09d01f16"); }
            set { setValue("app.key", value); }
        }

        /* Thông tin người tạo */
        public static string Author
        {
            get { return value("app.author"); }
            set { setValue("app.author", value); }
        }

        /* từ khóa tìm kiếm từ các trang tìm kiếm */
        public static string Keywords
        {
            get { return value("app.keyword"); }
            set { setValue("app.keyword", value); }
        }
        /* Thông tin bản quyền */
        public static string Copyright
        {
            get { return value("app.copyright", "Ma Kiem (hotoancntt15a@gmail.com)"); }
            set { setValue("app.copyright", value); }
        }
        /* Tạo mã - serial tự động */
        public static string Generator
        {
            get { return value("app.generator"); }
            set { setValue("app.generator", value); }
        }
        /* Cho phép 1 tài khoản có thể đăng nhập từ nhiều thiết bị cùng 1 thời điểm */
        public static int loginMulti
        {
            get { return valueInt("login.multi", 1); }
            set { setValue("login.multi", value); }
        }
        /* Sử dụng phân quyền theo nhóm quyền */
        public static int permissionUserGroup
        {
            get { return valueInt("permission.usergroup", 1); }
            set { setValue("permission.usergroup", value); }
        }

        public static int permission
        {
            get { return valueInt("permission", 0); }
            set { setValue("permission", value); }
        }
        /* Serial key */
        public static string serialkey
        {
            get { return value("app.serialkey"); }
            set { setValue("app.serialkey", value); }
        }
        /* Bảo mật */
        public static string security
        {
            get { return value("app.security"); }
            set { setValue("app.security", value); }
        }
        /* Thời gian sống của nhớ đăng nhập; đơn vị tính: ngày */
        public static int lifeTimeRemmenberLogin
        {
            get { return valueInt("app.lifetimeRemmenberLogin", 60); }
            set { setValue("app.lifetimeRemmenberLogin", value); }
        }
    }
}