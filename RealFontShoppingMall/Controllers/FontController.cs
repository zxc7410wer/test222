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
    public class FontController : Controller
    {
        RealFontShoppingmallEntities db = new RealFontShoppingmallEntities();

        //
        // GET: /Font/
        public ActionResult Index(int font_seq, int flag, int use)
        {
            ViewBag.font_seq = font_seq;
            ViewBag.flag = flag;
            ViewBag.use = use;
            return View();
        }

        // #0000 폰트 리스트 가져오기
        [HttpGet]
        public JsonResult List(int page, int flag, int use)
        {
            try
            {
                var query = db.Font
                    .Where(a => !a.deleted_at.HasValue);

                if (flag == FontType.brand)
                {
                    query = query
                        .Where(q => q.brand_seq.HasValue);
                }

                if(flag == FontType.free)
                {
                    if (use == FontUse.commercial)
                    {
                        query = query
                            .Where(q => q.price_commercial == 0)
                            .Where(q => q.is_personal == 0 || q.is_personal == 2);
                    }

                    if (use == FontUse.personal)
                    {
                        query = query
                            .Where(q => q.price == 0)
                            .Where(q => q.is_personal == 1 || q.is_personal == 2);
                    }
                }

                query = query
                    .OrderByDescending(q => q.important_point)
                    .ThenByDescending(q => q.created_at);

                var fonts = query
                    //.Skip((page - 1) * 30)
                    //.Take(30)
                    .ToList();

                var data = new List<FontViewModel>(); 
                foreach(var f in fonts)
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
                        price_commercial = f.price_commercial.HasValue ? f.price_commercial.Value : 0,
                        seller_name = f.seller_name,
                        file_url = f.file_url,
                        link_url = f.link_url,
                    };

                    if(f.brand_seq.HasValue)
                    {
                        model.brand_info = new BrandViewModel
                        {
                            brand_seq = f.brand_seq.HasValue ? f.brand_seq.Value : 0,
                            brand_url = f.Brand != null ? f.Brand.brand_url : null,
                            created_at = (f.Brand != null && f.Brand.created_at.HasValue )? f.Brand.created_at.Value.ToString("yyyy-MM-dd") : null,
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
                
        // #0000 폰트 상세 정보 가져오기
        [HttpGet]
        public JsonResult Detail(int font_seq, int flag, int use)
        {
            try
            {   
                var f = db.Font
                    .Where(a => !a.deleted_at.HasValue && a.seq == font_seq)
                    .FirstOrDefault();

                if (f == null)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, "no font");
                }

                // 선택한 폰트 이전,다음 데이터 가져오기
                var font_query = db.Font
                    .Where(a => !a.deleted_at.HasValue);
                var prev_font = new SimpleFontViewModel();
                var next_font = new SimpleFontViewModel();

                if (flag == FontType.free)
                {
                    font_query = font_query
                        .Where(q => q.price == 0);
                }

                if (flag == FontType.brand)
                {
                    font_query = font_query
                        .Where(q => q.brand_seq == f.brand_seq);
                }

                if (use == FontUse.commercial)
                {
                    font_query = font_query
                        .Where(q => q.is_personal == 0 || q.is_personal == 2);
                }

                if (use == FontUse.personal)
                {
                    font_query = font_query
                        .Where(q => q.is_personal == 1 || q.is_personal == 2);
                }

                var prev_info = font_query
                    .Where(a => (a.important_point < f.important_point) && a.seq != f.seq)
                    .OrderByDescending(a => a.important_point)
                    .ThenByDescending(a => a.created_at)
                    .FirstOrDefault();

                if(prev_info != null)
                {
                    prev_font.created_at = prev_info.created_at.HasValue ? prev_info.created_at.Value.ToString("yyyy-MM-dd") : null;
                    prev_font.font_seq = prev_info.seq;
                    prev_font.name = prev_info.name;
                    prev_font.is_personal = use;
                }

                var next_info = font_query
                    .Where(a => (a.important_point >= f.important_point) && a.seq != f.seq)
                    .OrderBy(a => a.important_point)
                    .ThenBy(a => a.created_at)
                    .FirstOrDefault();

                if(next_info != null)
                {
                    next_font.created_at = next_info.created_at.HasValue ? next_info.created_at.Value.ToString("yyyy-MM-dd") : null;
                    next_font.font_seq = next_info.seq;
                    next_font.name = next_info.name;
                    next_font.is_personal = use;
                }

                f.important_point += 100;
                db.SaveChanges();

                // 폰트 정보
                var font = new FontViewModel
                {   
                    created_at = f.created_at.HasValue ? f.created_at.Value.ToString("yyyy-MM-dd"): null,
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
                    price_commercial = f.price_commercial.HasValue ? f.price_commercial.Value : 0,
                    //seller_name = f.Brand != null ? f.Brand.name : null,
                    file_url = f.file_url,
                    link_url = f.link_url,
                    prev_font = prev_font,
                    next_font = next_font
                };

                if (f.brand_seq.HasValue)
                {
                    font.brand_info = new BrandViewModel
                    {
                        brand_seq = f.brand_seq,
                        brand_url = f.Brand.brand_url,
                        created_at = (f.Brand != null && f.Brand.created_at.HasValue) ? f.Brand.created_at.Value.ToString("yyyy-MM-dd") : null,
                        info = f.Brand.info,
                        name = f.Brand.name,
                        font_count = db.Font
                            .Count(t => !t.deleted_at.HasValue && t.brand_seq == f.brand_seq)
                    };
                }

                return Json(font, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }

        // #0000 폰트 seq로 리스트 가져오기
        [HttpGet]
        public JsonResult ListBySeq(string font_seqs)
        {
            try
            {
                var font_seqs_int = new List<int>();
                var splited = font_seqs.Split(',');
                foreach(var item in splited)
                {
                    if (item == "" || item == null)
                    {
                        continue;
                    }
                    var item_int = Convert.ToInt32(item);
                    font_seqs_int.Add(item_int);
                }
                var query = db.Font
                    .Where(a => !a.deleted_at.HasValue);

                var fonts = query
                    .Where(q => font_seqs_int.Contains(q.seq))
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
                        price_commercial = f.price_commercial.HasValue ? f.price_commercial.Value :0,
                        seller_name = f.seller_name,
                        file_url = f.file_url,
                        link_url = f.link_url
                    };

                    if (f.brand_seq.HasValue)
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

        // #0000 구매한 폰트 목록 가져오기
        [HttpGet]
        public JsonResult PurchasedFont(int user_seq, int page = 1)
        {
            try
            {
                var today = DateTime.Now;
                var purchased_font_list = new List<MyFontViewModel>();

                var font_query = db.Font
                    .Where(f => !f.deleted_at.HasValue);

                var query = db.Payment
                    .Where(mf => !mf.deleted_at.HasValue && mf.user_seq == user_seq && mf.ok == 1);

                foreach(var q in query)
                {
                    var font_ids = q.fontids.Split('/');

                    foreach(var font_id in font_ids)
                    {
                        if(font_id == "")
                        {
                            continue;
                        }
                        var font_seq = Convert.ToInt32(font_id);
                        var font = font_query
                            .Where(fq => fq.seq == font_seq)
                            .FirstOrDefault();

                        if(font == null)
                        {
                            continue;
                        }
                        if (q.is_personal == 0)
                        {
                            if (q.paid_at.Value.AddMonths(font.period.Value) < today && font.period != 0)
                            {
                                continue;
                            }
                        }                        

                        var model = new MyFontViewModel
                        {
                            my_font_seq = q.seq,
                            font_info = new FontViewModel
                            {
                                created_at = q.created_at.HasValue ? q.created_at.Value.ToString("yyyy-MM-dd") : null,
                                font_seq = font.seq,
                                format = font.format,
                                info = font.info,
                                is_free = font.is_free.HasValue ? font.is_free.Value : Free.no,
                                is_personal = q.is_personal.HasValue ? q.is_personal.Value : 1,
                                name = font.name,
                                os = font.os,
                                period = font.period.HasValue ? font.period.Value : 0,
                                photos = db.FontPhoto
                                    .Where(fp => !fp.deleted_at.HasValue && fp.font_seq == font.seq)
                                    .Select(fp => fp.photo_url)
                                    .ToList(),
                                price = font.price.HasValue ? font.price.Value : 0,
                                price_commercial = font.price_commercial.HasValue ? font.price_commercial.Value : 0,
                                seller_name = font.Brand.name,
                                file_url = font.file_url,
                                link_url = font.link_url
                            },
                            is_personal = q.is_personal.HasValue ? q.is_personal.Value : 1,
                            purchased_at = q.paid_at.HasValue ? q.paid_at.Value.ToString("yyyy-MM-dd") : null,
                            user_seq = user_seq
                        };

                        if (q.is_personal == 1)
                        {
                            model.font_info.period = 0;
                        }

                        if (font.brand_seq.HasValue)
                        {
                            model.font_info.brand_info = new BrandViewModel
                            {
                                brand_seq = font.brand_seq,
                                name = db.Brand
                                    .Where(b => !b.deleted_at.HasValue && b.seq == font.brand_seq)
                                    .FirstOrDefault() != null ? db.Brand
                                        .Where(b => !b.deleted_at.HasValue && b.seq == font.brand_seq)
                                        .FirstOrDefault().name : null
                            };
                        }
                        purchased_font_list.Add(model);
                    }
                }

                purchased_font_list = purchased_font_list
                    .OrderByDescending(q => q.font_info.created_at)
                    .Skip((page - 1) * 30)
                    .Take(30)
                    .ToList();

                var result = new
                {
                    state = "ok",
                    data = purchased_font_list,
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

        // #0000 장바구니에 담은 폰트 목록 가져오기
        [HttpGet]
        public JsonResult CartFont(int user_seq, int page = 1)
        {
            try
            {
                var purchased_font_list = new List<MyFontViewModel>();

                var payment_query = db.Payment
                    .Where(p => !p.deleted_at.HasValue && p.user_seq == user_seq)
                    .ToList();

                var query = db.MyFont
                    .Where(mf => !mf.deleted_at.HasValue && mf.user_seq == user_seq && !mf.purchased_at.HasValue);

                query = query
                    .OrderByDescending(q => q.created_at)
                    .Skip((page - 1) * 30)
                    .Take(30);

                foreach(var q in query)
                {
                    var font_seq_string = q.font_seq.ToString();
                    var payment_record = payment_query
                        .Where(pq => pq.fontids != null && pq.fontids.Split('/').Count(s => s == font_seq_string) > 0 && pq.is_personal == q.is_personal)
                        .OrderByDescending(pq => pq.paid_at)
                        .FirstOrDefault();
                    if (payment_record != null)
                    {
                        q.purchased_at = payment_record.paid_at;
                        continue;
                    }

                    var model = new MyFontViewModel
                    {
                        my_font_seq = q.seq,
                        font_info = new FontViewModel
                        {
                            created_at = q.created_at.HasValue ? q.created_at.Value.ToString("yyyy-MM-dd") : null,
                            font_seq = q.font_seq.HasValue ? q.font_seq.Value : 0,
                            format = q.Font.format,
                            info = q.Font.info,
                            is_free = q.Font.is_free.HasValue ? q.Font.is_free.Value : Free.no,
                            is_personal = q.is_personal.HasValue ? q.is_personal.Value : 1,
                            name = q.Font.name,
                            os = q.Font.os,
                            period = q.Font.period.HasValue ? q.Font.period.Value : 0,
                            photos = db.FontPhoto
                                .Where(fp => !fp.deleted_at.HasValue && fp.font_seq == q.font_seq)
                                .Select(fp => fp.photo_url)
                                .ToList(),
                            price = q.Font.price.HasValue ? q.Font.price.Value : 0,
                            price_commercial = q.Font.price_commercial.HasValue ? q.Font.price_commercial.Value : 0,
                            seller_name = q.Font.seller_name,
                            file_url = q.Font.file_url,
                            link_url = q.Font.link_url
                        },
                        is_personal = q.is_personal,
                        purchased_at = q.purchased_at.HasValue ? q.purchased_at.Value.ToString("yyyy-MM-dd") : null,
                        user_seq = user_seq
                    };

                    if(model.font_info.is_personal == 1)
                    {
                        model.font_info.period = 0;
                    }
                    
                    if (q.Font.brand_seq.HasValue)
                    {
                        model.font_info.brand_info = new BrandViewModel
                        {
                            brand_seq = q.Font.brand_seq,
                            name = db.Brand
                                .Where(b => !b.deleted_at.HasValue && b.seq == q.Font.brand_seq)
                                .FirstOrDefault() != null ? db.Brand
                                    .Where(b => !b.deleted_at.HasValue && b.seq == q.Font.brand_seq)
                                    .FirstOrDefault().name : null
                        };
                    }
                    purchased_font_list.Add(model);
                }

                db.SaveChanges();

                var result = new
                {
                    state = "ok",
                    data = purchased_font_list,
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

        // #0000 장바구니에 담기
        [HttpPost]
        public JsonResult AddCart(int user_seq, int font_seq, int is_personal)
        {
            try
            {
                var user = db.User
                    .Where(u => !u.deleted_at.HasValue && u.seq == user_seq)
                    .FirstOrDefault();

                if (user == null)
                {
                    return Json(new { state = "no user" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }

                var my_font = db.MyFont
                    .Where(mf => !mf.deleted_at.HasValue && mf.user_seq == user_seq && mf.font_seq == font_seq && mf.is_personal == is_personal)
                    .FirstOrDefault();

                if (my_font != null)
                {
                    if(my_font.purchased_at.HasValue)
                    {
                        return Json(new { state = "purchased" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { state = "duplicated" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    
                }

                var new_my_font = new MyFont
                {
                    created_at = DateTime.Now,
                    font_seq = font_seq,
                    updated_at = DateTime.Now,
                    user_seq = user_seq,
                    is_personal = is_personal
                };

                db.MyFont.Add(new_my_font);
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

        // #0000 장바구니에 삭제
        [HttpPost]
        public JsonResult RemoveCart(int user_seq, int my_font_seq)
        {
            try
            {
                var user = db.User
                    .Where(u => !u.deleted_at.HasValue && u.seq == user_seq)
                    .FirstOrDefault();

                if (user == null)
                {
                    return Json(new { state = "no user" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }

                var my_font = db.MyFont
                    .Where(mf => !mf.deleted_at.HasValue && mf.user_seq == user_seq && mf.seq == my_font_seq)
                    .FirstOrDefault();

                if (my_font == null)
                {
                    return Json(new { state = "no font" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }

                my_font.deleted_at = DateTime.Now;
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

        // #0000 폰트 구매하기
        [HttpPost]
        public JsonResult Purchase(int user_seq, int font_seq)
        {
            try
            {
                var user = db.User
                    .Where(u => !u.deleted_at.HasValue && u.seq == user_seq)
                    .FirstOrDefault();

                if (user == null)
                {
                    return Json(new { state = "no user" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }

                var my_font = db.MyFont
                    .Where(mf => !mf.deleted_at.HasValue && mf.user_seq == user_seq && mf.font_seq == font_seq && !mf.purchased_at.HasValue)
                    .FirstOrDefault();

                if (my_font == null)
                {
                    var new_my_font = new MyFont
                    {
                        created_at = DateTime.Now,
                        font_seq = font_seq,
                        purchased_at = DateTime.Now,
                        updated_at = DateTime.Now,
                        user_seq = user_seq
                    };
                    db.MyFont.Add(new_my_font);
                }
                else
                {
                    my_font.purchased_at = DateTime.Now;
                    my_font.updated_at = DateTime.Now;
                }

                var font = db.Font
                    .Where(f => !f.deleted_at.HasValue && f.seq == font_seq)
                    .FirstOrDefault();

                if(font != null)
                {
                    font.important_point += 1000;
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

        // #0000 무료폰트 개인용,상업용 개수 구하기
        [HttpGet]
        public JsonResult GetFreeFontCount()
        {
            try
            {
                var query = db.Font
                    .Where(a => !a.deleted_at.HasValue);

                var personal_count = query
                    .Where(q => q.price == 0)
                    .Where(q => q.is_personal == 1 || q.is_personal == 2)
                    .Count();

                var commercial_count = query
                    .Where(q => q.price_commercial == 0)
                    .Where(q => q.is_personal == 0 || q.is_personal == 2)
                    .Count();


                var result = new
                {
                    personal_count = personal_count,
                    commercial_count = commercial_count
                };

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }

        // #0000 새로 등록된 폰트 있는지 확인
        [HttpGet]
        public JsonResult CheckNewFont()
        {
            try
            {
                var time_now = DateTime.Now;
                var before_5minute = time_now.AddMinutes(-5);
                var query = db.Font
                    .Where(a => !a.deleted_at.HasValue)
                    .Where(a => a.created_at >= before_5minute);

                var free_commercial_count = query
                    .Where(q => q.price_commercial == 0)
                    .Where(q => q.is_personal == 0 || q.is_personal == 2)
                    .Count();

                var free_personal_count = query
                    .Where(q => q.price == 0)
                    .Where(q => q.is_personal == 1 || q.is_personal == 2)
                    .Count();

                var free_count = free_commercial_count + free_personal_count;

                var brand_count = query
                    .Where(a => (a.price > 0 && a.is_personal == 1) || (a.price_commercial > 0 && a.is_personal == 0) || (a.price > 0 && a.price > 0 && a.is_personal == 2))
                    .Count();


                var result = new
                {
                    state = "ok",
                    free_count = free_count,
                    brand_count = brand_count
                };

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }



        // #0000 관리자 폰트 리스트 가져오기
        [HttpGet]
        public JsonResult AdminList()
        {
            try
            {
                var fonts = db.Font
                    .Where(a => !a.deleted_at.HasValue)
                    .OrderByDescending(a => a.created_at)
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
                        price_commercial = f.price_commercial.HasValue ? f.price_commercial.Value : 0,
                        seller_name = f.seller_name,
                        click_count = f.click_count,
                        important_point = f.important_point,
                        file_url = f.file_url,
                        link_url = f.link_url,
                    };

                    if (f.brand_seq.HasValue)
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
                    total_count = fonts.Count()
                };

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }

        // #0000 폰트 상세 정보 가져오기
        [HttpGet]
        public JsonResult DetailBySeq(int font_seq)
        {
            try
            {
                var font = db.Font
                    .Where(f => !f.deleted_at.HasValue && f.seq == font_seq)
                    .FirstOrDefault();

                if (font == null)
                {
                    return Json(new { state = "no font" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }

                var model = new FontViewModel
                {
                    created_at = font.created_at.HasValue ? font.created_at.Value.ToString("yyyy-MM-dd") : null,
                    font_seq = font.seq,
                    format = font.format,
                    info = font.info,
                    intro = font.intro,
                    is_free = font.is_free.HasValue ? font.is_free.Value : Free.no,
                    is_personal = font.is_personal.HasValue ? font.is_personal.Value : 1,
                    name = font.name,
                    os = font.os,
                    period = font.period.HasValue ? font.period.Value : 0,
                    photos = font.FontPhoto
                        .Where(fp => !fp.deleted_at.HasValue)
                        .Select(fp => fp.photo_url)
                        .ToList(),
                    price = font.price.HasValue ? font.price.Value : 0,
                    price_commercial = font.price_commercial.HasValue ? font.price_commercial.Value : 0,
                    seller_name = font.seller_name,
                    file_url = font.file_url,
                    link_url = font.link_url,
                    click_count = font.click_count,
                    important_point = font.important_point,
                };

                if (font.brand_seq.HasValue)
                {
                    model.brand_info = new BrandViewModel
                    {
                        brand_seq = font.brand_seq,
                        brand_url = font.Brand.brand_url,
                        created_at = (font.Brand != null && font.Brand.created_at.HasValue) ? font.Brand.created_at.Value.ToString("yyyy-MM-dd") : null,
                        info = font.Brand.info,
                        name = font.Brand.name,
                        font_count = db.Font
                            .Count(t => !t.deleted_at.HasValue && t.brand_seq == font.brand_seq)
                    };
                }

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

        // #0000 폰트 등록하기
        [HttpPost]
        public JsonResult Add(string name, int? brand_seq, List<string> photos, int is_personal, int price, int? price_commercial, string os, string format, int period, string seller_name, string file_url, string info, string intro, string link_url, int important_point)
        {
            try
            {
                var new_font = new Font
                {
                    brand_seq = brand_seq,
                    click_count = 0,
                    created_at = DateTime.Now,
                    file_url = file_url,
                    format = format,
                    important_point = important_point,
                    info = info,
                    intro = intro,
                    is_personal = is_personal,
                    link_url = link_url,
                    name = name,
                    os = os,
                    period = period,
                    price = price,
                    price_commercial = price_commercial,
                    seller_name = seller_name,
                    updated_at = DateTime.Now
                };
                //if (price == 0)
                //{
                //    new_font.is_free = 1;
                //}
                //else
                //{
                //    new_font.is_free = 0;
                //}

                db.Font.Add(new_font);
                db.SaveChanges();

                if (photos != null)
                {
                    if (photos.Count > 0)
                    {
                        foreach (var photo in photos)
                        {
                            var model = new FontPhoto
                            {
                                created_at = DateTime.Now,
                                font_seq = new_font.seq,
                                photo_url = photo,
                                updated_at = DateTime.Now
                            };
                            db.FontPhoto.Add(model);
                        }
                    }
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

        // #0000 폰트 수정하기
        [HttpPost]
        public JsonResult Edit(int font_seq, string name, int? brand_seq, List<string> photos, int is_personal, int price, int? price_commercial, string os, string format, int period, string seller_name, string file_url, string info, string intro, string link_url, int important_point)
        {
            try
            {
                var font = db.Font
                    .Where(f => !f.deleted_at.HasValue && f.seq == font_seq)
                    .FirstOrDefault();

                if (font == null)
                {
                    return Json(new { state = "no font" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }

                font.name = name;
                font.brand_seq = brand_seq;
                //if (price == 0)
                //{
                //    font.is_free = 1;
                //}
                //else
                //{
                //    font.is_free = 0;
                //}
                font.is_personal = is_personal;
                font.link_url = link_url;
                font.file_url = file_url;
                font.format = format;
                font.important_point = important_point;
                font.info = info;
                font.intro = intro;
                font.os = os;
                font.name = name;
                font.period = period;
                font.price = price;
                font.price_commercial = price_commercial;
                font.seller_name = seller_name;
                font.updated_at = DateTime.Now;

                if(photos != null)
                {
                    if (photos.Count > 0)
                    {
                        var origin_photos = db.FontPhoto
                            .Where(f => !f.deleted_at.HasValue && f.font_seq == font_seq);

                        foreach (var photo in origin_photos)
                        {
                            photo.deleted_at = DateTime.Now;
                        }

                        foreach (var photo in photos)
                        {
                            var model = new FontPhoto
                            {
                                created_at = DateTime.Now,
                                font_seq = font_seq,
                                photo_url = photo,
                                updated_at = DateTime.Now
                            };
                            db.FontPhoto.Add(model);
                        }
                    }
                }
                else
                {
                    var origin_photos = db.FontPhoto
                            .Where(f => !f.deleted_at.HasValue && f.font_seq == font_seq);

                    foreach (var photo in origin_photos)
                    {
                        photo.deleted_at = DateTime.Now;
                    }
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

        // #0000 폰트 삭제하기
        [HttpPost]
        public JsonResult Delete(int font_seq)
        {
            try
            {
                var font = db.Font
                    .Where(f => !f.deleted_at.HasValue && f.seq == font_seq)
                    .FirstOrDefault();

                if (font == null)
                {
                    return Json(new { state = "no font" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
                font.deleted_at = DateTime.Now;
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