using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyHoSoSucKhoe.Controllers
{
    public class keepliveController : Controller
    {
        // GET: keeplive  
        public ActionResult Index()
        {
            local.IpConnect[Session.SessionID] = Request.setFormatOnilne(Session);
            return Content(local.IpConnect.Count.ToString());
        }

        public ActionResult ViewConnect() { return View(); }
    }
}