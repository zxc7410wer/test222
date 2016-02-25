using RealFontShoppingMall.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RealFontShoppingMall.Controllers
{
    public class PhotoController : Controller
    {
        // GET: Photo
        public ActionResult Index()
        {
            return View();
        }

        // #0000 사진 업로드
        [HttpPost]
        public JsonResult Upload()
        {
            try
            {
                Guid gu = Guid.NewGuid();
                if (Request.Files.Count != 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = Request.Files[i];
                        int fileSize = file.ContentLength;
                        string fileName = file.FileName;
                        file.SaveAs(@"C:\Files\REALFONT\Image\" + gu.ToString() + ".png");
                    }
                    return Json(new { url = SERVER_URL.image + gu.ToString() + ".png" }, "application/json", JsonRequestBehavior.AllowGet);
                }
                return Json("no posted file", "application/json", JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
    }
}