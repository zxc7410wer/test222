using RealFontShoppingMall.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RealFontShoppingMall.Controllers
{
    public class CsController : Controller
    {
        RealFontShoppingmallEntities db = new RealFontShoppingmallEntities();

        //
        // GET: /Cs/
        public ActionResult Index()
        {
            return View();
        }

        // #0000 문의하기
        [HttpPost]
        public JsonResult Request(string name, string email, string text)
        {
            try
            {
                var new_cs = new Cs
                {
                    created_at = DateTime.Now,
                    email = email,
                    name = name,
                    text = text,
                    updated_at = DateTime.Now
                };
                db.Cs.Add(new_cs);
                db.SaveChanges();

                var result = new
                {
                    state = "ok"
                };

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
	}
}