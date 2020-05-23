using System.Web.Mvc;

namespace QuanLyHoSoSucKhoe.Areas.sys
{
    public class sysAreaRegistration : AreaRegistration 
    {
        public override string AreaName { get { return "sys"; } }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "sys_default",
                "sys/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}