using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyHoSoSucKhoe.Areas.bacsy.Controllers
{
    public class homeController : Controller
    {
        // GET: bacsy/home
        public ActionResult Index()
        {
            return View();
        }
    }
}