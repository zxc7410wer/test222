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
    public class BlogController : Controller
    {
        RealFontShoppingmallEntities db = new RealFontShoppingmallEntities();

        // GET: Blog
        public ActionResult Index()
        {
            return View();
        }

        // #0000 블로그 리스트 가져오기
        [HttpGet]
        public JsonResult List()
        {
            try
            {
                var blog_list = new List<BlogViewModel>();
                var query = db.Blog
                    .Where(f => !f.deleted_at.HasValue);

                query = query
                    .OrderByDescending(q => q.important_point);

                foreach (var q in query)
                {   
                    var model = new BlogViewModel
                    {
                        blog_seq = q.seq,
                        link_url = q.link_url,
                        created_at = q.created_at.HasValue ? q.created_at.Value.ToString("yyyy-MM-dd") : null,
                        photos = q.BlogPhoto
                            .Where(bp => !bp.deleted_at.HasValue)
                            .Select(bp => bp.photo_url)
                            .ToList(),
                        tags = q.tag,
                        info = q.info,
                        name = q.name,
                        important_point = q.important_point,
                        color = q.color
                    };
                    blog_list.Add(model);
                }

                var result = new
                {
                    state = "ok",
                    total_count = query.Count(),
                    data = blog_list
                };

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }

        // #0000 블로그 상세
        [HttpGet]
        public JsonResult Detail(int blog_seq)
        {
            try
            {   
                var blog = db.Blog
                    .Where(f => !f.deleted_at.HasValue && f.seq == blog_seq)
                    .FirstOrDefault();

                if(blog == null)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, "no blog");
                }

                var model = new BlogViewModel
                {
                    blog_seq = blog.seq,
                    link_url = blog.link_url,
                    created_at = blog.created_at.HasValue ? blog.created_at.Value.ToString("yyyy-MM-dd") : null,
                    photo = blog.BlogPhoto
                        .Where(bp => !bp.deleted_at.HasValue)
                        .OrderByDescending(bp => bp.created_at)
                        .Select(bp => bp.photo_url)
                        .FirstOrDefault(),
                    info = blog.info,
                    name = blog.name,
                    tags = blog.tag,
                    important_point = blog.important_point,
                    color = blog.color
                };

                //var tags = new List<string>();
                //if (blog.tag != null)
                //{
                //    tags = blog.tag.Split('#')
                //        .Where(s => s != "").ToList();
                //}
                //model.tags = tags;

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

        // #0000 블로그 추가하기
        [HttpPost]
        public JsonResult Add(string name, string link_url, string info, string photo, string tags, int? important_point, string color)
        {
            try
            {
                var new_blog = new Blog
                {
                    created_at = DateTime.Now,
                    info = info,
                    link_url = link_url,
                    name = name,
                    tag = tags,
                    important_point = important_point.HasValue ? important_point.Value : 0,
                    updated_at = DateTime.Now,
                    color = color
                };

                db.Blog.Add(new_blog);
                db.SaveChanges();

                if(photo != null)
                {
                    var new_blog_photo = new BlogPhoto
                    {
                        blog_seq = new_blog.seq,
                        created_at = DateTime.Now,
                        photo_url = photo,
                        updated_at = DateTime.Now
                    };
                    db.BlogPhoto.Add(new_blog_photo);
                }

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

        // #0000 블로그 수정하기
        [HttpPost]
        public JsonResult Edit(int blog_seq, string name, string link_url, string info, string photo, string tags, int? important_point, string color)
        {
            try
            {
                var blog = db.Blog
                    .Where(f => !f.deleted_at.HasValue && f.seq == blog_seq)
                    .FirstOrDefault();

                if (blog == null)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, "no blog");
                }

                blog.name = name;
                blog.link_url = link_url;
                blog.info = info;
                blog.tag = tags;
                blog.important_point = important_point;
                blog.color = color;
                blog.updated_at = DateTime.Now;

                var blog_photos = db.BlogPhoto
                    .Where(bp => !bp.deleted_at.HasValue && bp.blog_seq == blog_seq)
                    .ToList();

                

                // 새로운 사진 추가
                if (photo != null)
                {
                    // 기존 사진 삭제
                    foreach (var item in blog_photos)
                    {
                        item.deleted_at = DateTime.Now;
                    }
                    var photo_model = new BlogPhoto
                    {
                        blog_seq = blog_seq,
                        created_at = DateTime.Now,
                        photo_url = photo,
                        updated_at = DateTime.Now
                    };
                    db.BlogPhoto.Add(photo_model);
                }

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

        // #0000 블로그 삭제하기
        [HttpPost]
        public JsonResult Delete(int blog_seq)
        {
            try
            {
                var blog = db.Blog
                    .Where(f => !f.deleted_at.HasValue && f.seq == blog_seq)
                    .FirstOrDefault();

                if (blog == null)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, "no blog");
                }

                blog.deleted_at = DateTime.Now;
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