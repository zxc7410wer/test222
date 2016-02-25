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
    public class BrandController : Controller
    {
        RealFontShoppingmallEntities db = new RealFontShoppingmallEntities();

        //
        // GET: /Brand/
        public ActionResult Index(int brand_seq)
        {
            @ViewBag.brand_seq = brand_seq;
            return View();
        }

        // #0000 브랜드 리스트 가져오기
        [HttpGet]
        public JsonResult List()
        {
            try
            {
                var brand_list = new List<BrandViewModel>();
                var query = db.Brand
                    .Where(f => !f.deleted_at.HasValue);

                query = query
                    .OrderByDescending(q => q.important_point);

                foreach(var q in query)
                {
                    var model = new BrandViewModel
                    {
                        brand_seq = q.seq,
                        brand_url = q.brand_url,
                        created_at = q.created_at.HasValue ? q.created_at.Value.ToString("yyyy-MM-dd") : null,
                        font_count = q.Font
                            .Where(a => !a.deleted_at.HasValue)
                            .Where(a => (a.price > 0 && a.is_personal == 1) || (a.price_commercial > 0 && a.is_personal == 0) || (a.price > 0 && a.price > 0 && a.is_personal == 2))
                            .Count(),
                        info = q.info,
                        name = q.name,
                        important_point = q.important_point,
                        link_click_count = q.link_click_count
                    };
                    brand_list.Add(model);
                }

                //brand_list = query
                //    .Select(q => new BrandViewModel
                //    {
                //        brand_seq = q.seq,
                //        brand_url = q.brand_url,
                //        created_at = q.created_at,
                //        font_count = q.Font.Count(f => !f.deleted_at.HasValue),
                //        info = q.info,
                //        name = q.name,
                //        important_point = q.important_point,
                //        link_click_count = q.link_click_count
                //    })
                //    .ToList();

                var result = new
                {
                    state = "ok",
                    total_count = query.Count(),
                    data = brand_list
                };

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }

        // #0000 브랜드 별 폰트 리스트 가져오기
        [HttpGet]
        public JsonResult Fonts(int brand_seq, int page = 1)
        {
            try
            {
                var query = db.Font
                    .Where(a => !a.deleted_at.HasValue)
                    .Where(a => (a.price > 0 && a.is_personal == 1) || (a.price_commercial > 0 && a.is_personal == 0) || (a.price > 0 && a.price > 0 && a.is_personal == 2));

                if(brand_seq == 0)
                {
                    query = query
                        .Where(q => q.brand_seq.HasValue);
                }
                else
                {
                    query = query
                        .Where(q => q.brand_seq == brand_seq);

                    var brand = db.Brand
                        .Where(b => !b.deleted_at.HasValue && b.seq == brand_seq)
                        .FirstOrDefault();

                    if(brand != null){
                        brand.important_point += 100;
                        db.SaveChanges();
                    }
                }

                query = query
                    .OrderByDescending(q => q.important_point);

                var fonts = query
                    .Skip((page - 1) * 30)
                    .Take(30)
                    .ToList();

                var data = new List<FontViewModel>();
                foreach (var f in fonts)
                {
                    var model = new FontViewModel
                    {
                        created_at = f.created_at.HasValue ? f.created_at.Value.ToString("yyyy-MM-dd") : null,
                        font_seq = f.seq,
                        format = f.format,
                        info = f.info,
                        intro = f.intro,
                        is_free = f.is_free.HasValue ? f.is_free.Value : Free.no,
                        is_personal = f.is_personal.HasValue ? f.is_personal.Value : 1,
                        name = f.name,
                        os = f.os,
                        period = f.period.HasValue ? f.period.Value : 0,
                        photos = f.FontPhoto
                            .Where(fp => !fp.deleted_at.HasValue)
                            .Select(fp => fp.photo_url)
                            .ToList(),
                        price = f.price.HasValue ? f.price.Value : 0,
                        seller_name = f.seller_name,
                        file_url = f.file_url,
                        link_url = f.link_url
                    };

                    if(f.brand_seq.HasValue)
                    {
                        model.brand_info = new BrandViewModel
                        {
                            brand_seq = f.brand_seq.HasValue ? f.brand_seq.Value : 0,
                            brand_url = f.Brand != null ? f.Brand.brand_url : null,
                            created_at = (f.Brand != null && f.Brand.created_at.HasValue) ? f.Brand.created_at.Value.ToString("yyyy-MM-dd") : null,
                            info = f.Brand != null ? f.Brand.info : null,
                            name = f.Brand != null ? f.Brand.name : null,
                            font_count = db.Font
                                .Count(t => !t.deleted_at.HasValue && t.brand_seq == f.brand_seq)
                        };
                    }
                    data.Add(model);
                }

                var result = new
                {
                    state = "ok",
                    data = data,
                    total_count = query.Count()
                };

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }

        // #0000 브랜드 등록하기
        [HttpPost]
        public JsonResult Add(string name, string info, string brand_url, string photo, int? important_point)
        {
            try
            {
                var new_brand = new Brand
                {
                    brand_url = brand_url,
                    created_at = DateTime.Now,
                    important_point = important_point.HasValue ? important_point.Value : 0,
                    info = info,
                    link_click_count = 0,
                    name = name,
                    photo = photo,
                    updated_at = DateTime.Now
                };

                db.Brand.Add(new_brand);
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
        
        // #0000 브랜드 수정하기
        [HttpPost]
        public JsonResult Edit(int brand_seq, string name, string info, string brand_url, string photo, int? important_point)
        {
            try
            {
                var brand = db.Brand
                    .Where(b => !b.deleted_at.HasValue && b.seq == brand_seq)
                    .FirstOrDefault();

                if (brand == null)
                {
                    return Json(new { state = "no brand" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }

                brand.name = name;
                brand.info = info;
                brand.brand_url = brand_url;
                brand.photo = photo;
                if(important_point.HasValue)
                {
                    brand.important_point = important_point.Value;
                }
                brand.updated_at = DateTime.Now;
                
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

        // #0000 브랜드 상세
        [HttpGet]
        public JsonResult Detail(int brand_seq)
        {
            try
            {
                var brand = db.Brand
                    .Where(b => !b.deleted_at.HasValue && b.seq == brand_seq)
                    .FirstOrDefault();

                if (brand == null)
                {
                    return Json(new { state = "no brand" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }

                var model = new BrandViewModel
                {
                    brand_seq = brand.seq,
                    brand_url = brand.brand_url,
                    created_at = brand.created_at.HasValue ? brand.created_at.Value.ToString("yyyy-MM-dd") : null,
                    font_count = brand.Font.Count(f => !f.deleted_at.HasValue),
                    important_point = brand.important_point,
                    info = brand.info,
                    link_click_count = brand.link_click_count,
                    name = brand.name,
                    photo = brand.photo,
                };

                var result = new
                {
                    state = "ok",
                    data = model
                };

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }

        // #0000 브랜드 삭제
        [HttpPost]
        public JsonResult Delete(int brand_seq)
        {
            try
            {
                var brand = db.Brand
                    .Where(b => !b.deleted_at.HasValue && b.seq == brand_seq)
                    .FirstOrDefault();

                if (brand == null)
                {
                    return Json(new { state = "no brand" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
                
                brand.deleted_at = DateTime.Now;

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