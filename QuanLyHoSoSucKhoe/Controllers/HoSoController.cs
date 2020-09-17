using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using zModules;

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
            var tmp = Request.showRequest();
            return Content(tmp);
        }
    }
}