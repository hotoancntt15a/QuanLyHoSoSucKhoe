using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace QuanLyHoSoSucKhoe
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            local.Start();
        }
        protected void Application_Error()
        {
            var httpException = (Server.GetLastError()) as HttpException;
            var statusCode = httpException.GetHttpCode();
            if (statusCode == 404) { Response.Redirect($"~/error?{keyS.redirect}={Server.UrlEncode(Request.Url.PathAndQuery)}&message={Server.UrlEncode("Đường dẫn không tồn tại hoặc bị giới hạn truy cập")}"); }
        }
        private void Session_Start(object sender, EventArgs e)
        {
            string time = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            Session[keyS.timeStart] = time;
            try { local.IpConnect[Session.SessionID] = Request.setFormatOnilne(Session); } catch { }
        }

        private void Session_End(object sender, EventArgs e)
        {
            try { local.IpConnect.Remove(Session.SessionID); } catch { }
        }
    }
}
