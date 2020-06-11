using System.Web.Mvc;

namespace QuanLyHoSoSucKhoe.Areas.danhmuc
{
    public class danhmucAreaRegistration : AreaRegistration {
        public override string AreaName { get { return "danhmuc"; } } 
        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "danhmuc_default",
                "danhmuc/{controller}/{action}/{id}",
                new { controller = "home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}