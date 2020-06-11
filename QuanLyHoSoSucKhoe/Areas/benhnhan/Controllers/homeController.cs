using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyHoSoSucKhoe.Areas.benhnhan.Controllers
{
    public class homeController : Controller
    {
        // GET: benhnhan/home
        public ActionResult Index()
        {
            return View();
        }
    }
}