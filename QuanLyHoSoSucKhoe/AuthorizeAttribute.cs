using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using zModules;

namespace QuanLyHoSoSucKhoe
{
    public class CheckLoginAttribute : ActionMethodSelectorAttribute
    {
        private int idgroup = -1;
        private bool autoLogin = false;
        private string redirect = "";
        private bool everyone = false; 

        public CheckLoginAttribute(int idgroup = -1, bool autoLogin = false, string redirect = "", bool EveryOne = false)
        {
            this.idgroup = idgroup;
            this.autoLogin = autoLogin;
            this.redirect = redirect;
            everyone = EveryOne; 
        }
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var w = controllerContext.HttpContext; 
            /* Kiểm tra ghi nhớ đăng nhập trên trình duyệt */
            var rs = local.setLogin();
            var reg = new Regex("^[0-9]$");
            if (rs == true)
            {
                if (everyone) { return true; }
                string temp = $"{w.Session["capdo"]}";
                if (temp == "0") { return true; }
                var area = $"{controllerContext.RouteData.DataTokens["area"]}";
                string msg = "";
                while (true)
                { 
                    if(area == "") { break; }
                    var access = getConfigPermission("permission.0", "sys"); 
                    if(access.Contains(area)) { msg = "Giới hạn quyền truy cập chức năng. Vui lòng liên hệ quản trị viên"; break; }
                    /* Cấp tỉnh */ 
                    access = getConfigPermission("permission.1", "hethong");
                    if (access.Contains(area)) { if(temp != "1") { msg = "Giới hạn quyền truy cập chức năng. Vui lòng liên hệ quản trị viên"; break; } }
                    /* Cấp huyện không cho phép truy cập chức năng điều chỉnh hệ thống */
                    /* Cấp xã, không cấp không có quyền truy cập chức năng quản lý */
                    int capdo = reg.IsMatch(temp) ? int.Parse(temp) : 4;
                    if (capdo > 3)
                    {
                        msg = "Giới hạn quyền truy cập chức năng. Vui lòng liên hệ quản trị viên";
                        break;
                    }
                    break;
                }
                if (msg != "")
                { 
                    w.Session[keyS.Error] = msg;
                    w.Response.Redirect($"/error?url={w.Server.HtmlEncode(w.Request.Url.AbsolutePath)}&message=" + w.Server.HtmlEncode(msg));
                    return false;
                }
                return true;
            }
            /* if (autoLogin) return true; */
            if (string.IsNullOrEmpty(redirect)) { w.Response.Redirect("/login?redirect=" + w.Server.UrlEncode(w.Request.Url.PathAndQuery)); }
            else { w.Response.Redirect("/login?redirect=" + w.Server.UrlEncode(redirect)); }
            return false;
        }

        private List<string> getConfigPermission(string key, string defValue)
        {
            var access = AppConfig.value(key, defValue).Replace(" ", "");
            var m = Regex.Matches(access, "[a-zA-Z0-9,]+"); 
            if (m.Count == 1) { return access.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
            return new List<string>() { defValue };

        }
    }
}