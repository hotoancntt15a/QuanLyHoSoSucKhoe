using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyHoSoSucKhoe.Controllers
{
    public class loginController : Controller
    {
        public ActionResult Index()
        {
            var redirect = Request.getValue("redirect");
            if (Request["username"] == null) { return View(); }
            var user = Request.getValue("username");
            var pass = Request.getValue("password").toPassword();
            var rm = Request.getValue("remmember");
            if (user == "" && pass == "") { return View(); }
            if (user == "" || pass == "")
            {
                ViewBag.Error = "Thiếu thông tin đăng nhập";
                return View();
            }
            var rs = local.setLogin(new LoginInfo(user, pass, rm == "1"));
            if (rs == false)
            {
                ViewBag.Error = Session.getError();
                if (Request["w"] != null) { return RedirectToAction("index"); }
                return View();
            }
            if (Request["w"] != null) { return RedirectToAction("index"); }
            if (string.IsNullOrEmpty(redirect) == true) { return RedirectToAction("Index", "Home", new { area = "" }); }
            return Redirect(redirect);
        }

        public ActionResult quenmatkhau() => Content("Vui lòng liên hệ quản trị viên => để cấp lại mật khẩu");
    }
}