using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyHoSoSucKhoe.Areas.danhmuc.Controllers
{
    public class homeController : Controller
    {
        // GET: danhmuc/home
        public ActionResult Index()
        {
            return View();
        }
    }
}