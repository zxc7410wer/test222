using RealFontShoppingMall.Entities;
using RealFontShoppingMall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RealFontShoppingMall.Controllers
{
    public class MyPageController : Controller
    {
        RealFontShoppingmallEntities db = new RealFontShoppingmallEntities();

        //
        // GET: /MyPage/
        public ActionResult Index(int flag)
        {
            ViewBag.flag = flag;
            return View();
        }
	}
}