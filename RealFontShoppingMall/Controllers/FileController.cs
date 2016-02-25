using RealFontShoppingMall.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RealFontShoppingMall.Controllers
{
    public class FileController : Controller
    {
        // GET: File
        public ActionResult Index()
        {
            return View();
        }

        // #0000 파일 업로드
        [HttpPost]
        public JsonResult Upload()
        {
            try
            {
                Guid gu = Guid.NewGuid();
                if (Request.Files.Count != 0)
                {
                    var extension = "";
                    var final_name = "";
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = Request.Files[i];
                        int fileSize = file.ContentLength;
                        string fileName = file.FileName;
                        var splited = file.FileName.Split('.');
                        var splited_length = splited.Length;

                        extension = "." + splited[splited_length - 1];
                        final_name = fileName;
                        //final_name = fileName.Replace(extension, "") + "_" + gu.ToString() + extension;
                        file.SaveAs(@"C:\Files\REALFONT\FontFile\" + final_name);
                    }
                    return Json(new { url = SERVER_URL.file + final_name }, "application/json", JsonRequestBehavior.AllowGet);
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