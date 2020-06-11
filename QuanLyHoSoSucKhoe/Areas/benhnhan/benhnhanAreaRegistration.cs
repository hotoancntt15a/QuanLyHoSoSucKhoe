using System.Web.Mvc;

namespace QuanLyHoSoSucKhoe.Areas.benhnhan
{
    public class benhnhanAreaRegistration : AreaRegistration 
    {
        public override string AreaName { get { return "benhnhan"; } }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "benhnhan_default",
                "benhnhan/{controller}/{action}/{id}",
                new { controller = "home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}