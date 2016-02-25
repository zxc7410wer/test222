using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealFontShoppingMall.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index(int flag)
        {
            @ViewBag.flag = flag;
            return View();
        }
    }
}