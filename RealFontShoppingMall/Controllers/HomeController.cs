using RealFontShoppingMall.Entities;
using RealFontShoppingMall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RealFontShoppingMall.Controllers
{
    public class HomeController : Controller
    {
        RealFontShoppingmallEntities db = new RealFontShoppingmallEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult Index3()
        {
            return View();
        }

        // #0000 전체 폰트 리스트 불러오기
        //[HttpGet]
        //public JsonResult List(int page = 1)
        //{
        //    try
        //    {
        //        var font_list = new List<FontViewModel>();
        //        var query = db.Font
        //            .Where(f => !f.deleted_at.HasValue);

        //        query = query
        //            .OrderByDescending(q => q.created_at)
        //            .Skip((page - 1) * 30)
        //            .Take(30);

        //        font_list = query
        //            .Select(q => new FontViewModel
        //            {
        //                brand_info = new BrandViewModel
        //                {
        //                    brand_seq = q.brand_seq,
        //                    brand_url = q.Brand.brand_url,
        //                    created_at = q.Brand.created_at,
        //                    info = q.Brand.info,
        //                    name = q.Brand.name,
        //                    font_count = db.Font
        //                        .Count(f => !f.deleted_at.HasValue && f.brand_seq == q.brand_seq)
        //                },
        //                created_at = q.created_at,
        //                font_seq = q.seq,
        //                format = q.format,
        //                info = q.info,
        //                is_free = q.is_free.HasValue ? q.is_free.Value : Free.no,
        //                name = q.name,
        //                os = q.os,
        //                period = q.period.HasValue ? q.period.Value : 0,
        //                photos = q.FontPhoto
        //                    .Where(fp => !fp.deleted_at.HasValue)
        //                    .Select(fp => fp.photo_url)
        //                    .ToList(),
        //                price = q.price.HasValue ? q.price.Value : 0,
        //                seller_name = q.seller_name
        //            })
        //            .ToList();

        //        var result = new
        //        {
        //            state = "ok",
        //            total_count = query.Count(),
        //            data = font_list
        //        };

        //        return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (HttpException e)
        //    {
        //        Response.StatusCode = e.GetHttpCode();
        //        return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}