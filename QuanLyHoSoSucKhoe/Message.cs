using System;

namespace QuanLyHoSoSucKhoe
{
    public class msgView
    {
        public static string notFoundRecord { get; } = "Bản ghi đã bị xóa hoặc không tồn tại trên hệ thống";
        public static string notFoundFile { get; } = "Tập tin đã bị xóa hoặc không tồn tại trên hệ thống";
        public static string notFoundUrlQuery { get; } = "Đường dẫn Url sai";
        public static string actionSuccess { get { return $"Thao tác thành công {DateTime.Now: dd/MM/yyyy HH:mm:ss}"; } }
        public static string keyRedirect { get; } = "redirect";
    }

    public static class keyTable
    {
        public static string importData { get; } = "importdata";
        public static string config { get; } = "sys-config";
        public static string history { get; } = "sys-history";
    }

    public static class keyS
    {
        public static string Message { get; } = "M00x00000";
        public static string Error { get; } = "E00x00000";
        public static string Per_Blocked { get; } = "per.blocked";
        public static string Per_Read { get; } = "per.read";
        public static string Per_Write { get; } = "per.write";
        public static string Per_AddNew { get; } = "per.addnew";
        public static string Per_Edit { get; } = "per.edit";
        public static string Per_Delete { get; } = "per.delete";
        public static string Per_Down { get; } = "per.down";
        public static string isLogin { get; } = "islogin";
        public static string idUser { get; } = "iduser";
        public static string idSession { get; } = "idSession";
        public static string remmenber { get; } = "remmenber";
        public static string timeStart { get; } = "session_timestart";
        public static string redirect { get; } = "redirect";
        public static string capdo { get; } = "capdo";
    }

    public static class KeyCookie
    {
        public static string auth { get; } = "__" + (typeof(KeyCookie)).FullName + "_auth";
        public static string iduser { get; } = "__" + (typeof(KeyCookie)).FullName + "_iduser";
    }

    public static class Folders
    {
        public static string pathApp { set; get; } = "";

        /// <summary>
        /// Path: Server.MathPath("~")
        /// </summary>
        public static string DataImage { get { return pathApp + "App_Data\\Images"; } }

        /// <summary>
        /// Path: Server.MathPath("~")\App_Data
        /// </summary>
        public static string App_Data { get { return pathApp + "App_Data"; } }

        /// <summary>
        /// Path: Server.MathPath("~")\App_Data\Files
        /// </summary>
        public static string DataFile { get { return pathApp + "App_Data\\Files"; } }

        /// <summary>
        /// Path: Server.MathPath("~")\App_Data\Files
        /// </summary>
        public static string Images { get { return pathApp + "images"; } }

        /// <summary>
        /// Path: Server.MathPath("~")\Scripts
        /// </summary>
        public static string Scripts { get { return pathApp + "Scripts"; } }

        /// <summary>
        /// Path: Server.MathPath("~")\Content
        /// </summary>
        public static string Content { get { return pathApp + "Content"; } }

        /// <summary>
        /// Path: Server.MathPath("~")\Backup
        /// </summary>
        public static string Backup { get { return pathApp + "temp\\Backup"; } }

        public static string temp { get { return pathApp + "temp"; } }
    }
}