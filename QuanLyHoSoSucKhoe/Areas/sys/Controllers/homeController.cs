using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace QuanLyHoSoSucKhoe.Areas.sys.Controllers
{
    public class homeController : Controller
    { 
        // GET: sys/home
        [CheckLogin]
        public ActionResult index()
        { 
            return View();
        }
    }
}