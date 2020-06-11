using System.Web.Mvc;

namespace QuanLyHoSoSucKhoe.Areas.bacsy
{
    public class bacsyAreaRegistration : AreaRegistration 
    {
        public override string AreaName { get { return "bacsy"; } }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "bacsy_default",
                "bacsy/{controller}/{action}/{id}",
                new { controller = "home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}