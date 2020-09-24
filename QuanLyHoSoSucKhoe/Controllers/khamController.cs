using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using zModules;
using zModules.MSSQLServer;

namespace QuanLyHoSoSucKhoe.Controllers
{
    public class khamController : Controller
    {
        // GET: kham 
        [Properties(Name = "Khám", Note = "fa fa-list")]
        public ActionResult Index()
        {
            return View();
        } 

        public ActionResult dotkham()
        {
            return View();
        }
         
        [HttpGet]
        [ActionName("khamtheluc")]
        public ActionResult khamtheluc_get()
        {
            return View();
        }
        [HttpPost]
        [ActionName("khamtheluc")]
        public ActionResult khamtheluc_post()
        {
            return View();
        }
    }
}