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
    public class PaymentController : Controller
    {
        RealFontShoppingmallEntities db = new RealFontShoppingmallEntities();

        // 개인용 구폰트 구매 팝업
        //
        // GET: /Payment/
        public ActionResult Index(string font_seqs)
        {
            ViewBag.font_seqs = font_seqs;
            return View();
        }

        // 상업용 폰트 구매 팝업
        //
        // GET: /Payment/
        public ActionResult Index2(string font_seqs)
        {
            ViewBag.font_seqs = font_seqs;
            return View();
        }

        // 무료 상업용 폰트 다운로드시 팝업
        //
        // GET: /Payment/
        public ActionResult Index3(int font_seq)
        {
            ViewBag.font_seq = font_seq;
            return View();
        }

        //// #0000 구매완료
        //[HttpPost]
        //public JsonResult RequestPayment(int user_seq, string font_seqs, string method, int price, int use)
        //{
        //    try
        //    {
        //        var font_seqs_int = new List<int>();
        //        var fontids = font_seqs.Replace(",", "/");
        //        var splited = font_seqs.Split(',');
        //        foreach(var item in splited)
        //        {
        //            if(item == null || item == "")
        //            {
        //                continue;
        //            }

        //            var item_int = Convert.ToInt32(item);
        //            font_seqs_int.Add(item_int);
        //        }
        //        var product = "";
        //        var query = db.MyFont
        //            .Where(mf => !mf.deleted_at.HasValue && mf.user_seq == user_seq && !mf.purchased_at.HasValue);

        //        foreach (var item in font_seqs_int)
        //        {
        //            if(query.Count(q => q.font_seq == item) > 0)
        //            {
        //                var my_font = query
        //                    .Where(q => q.font_seq == item)
        //                    .FirstOrDefault();
        //                if(my_font == null)
        //                {
        //                    continue;
        //                }
        //                my_font.purchased_at = DateTime.Now;
        //                my_font.updated_at = DateTime.Now;
        //            }
        //            else 
        //            {
        //                var new_my_font = new MyFont
        //                {
        //                    created_at = DateTime.Now,
        //                    font_seq = item,
        //                    purchased_at = DateTime.Now,
        //                    updated_at = DateTime.Now,
        //                    user_seq = user_seq
        //                };
        //                db.MyFont.Add(new_my_font);
        //            }
        //        }

        //        var font_query = db.Font
        //            .Where(q => !q.deleted_at.HasValue);

        //        foreach (var item in font_seqs_int)
        //        {
        //            var font = font_query
        //                .Where(q => q.seq == item)
        //                .FirstOrDefault();

        //            if (font == null)
        //            {
        //                continue;
        //            }
        //            product += (font.name + "/");
        //        }

        //        product = product.TrimEnd('/');

        //        var new_payment = new Payment
        //        {
        //            created_at = DateTime.Now,
        //            method = method,
        //            price = price,
        //            product = product,
        //            updated_at = DateTime.Now,
        //            user_seq = user_seq,
        //            is_personal = use,
        //            ok = 0,
        //            fontids = fontids
        //        };
        //        db.Payment.Add(new_payment);
        //        db.SaveChanges();

        //        var result = new
        //        {
        //            state = "ok"
        //        };

        //        return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (HttpException e)
        //    {
        //        Response.StatusCode = e.GetHttpCode();
        //        return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //// #0000 상업용 구매
        //[HttpPost]
        //public JsonResult completecommercial(int user_seq, int font_seq, string method, string company_number, string email, string name, string phone, string purpose, int price)
        //{
        //    try
        //    {
        //        var font = db.Font
        //            .Where(f => !f.deleted_at.HasValue && f.seq == font_seq)
        //            .FirstOrDefault();

        //        if(font == null)
        //        {
        //            throw new HttpException((int)HttpStatusCode.BadRequest, "no font");
        //        }

        //        var new_payment = new Payment
        //        {
        //            company_email = email,
        //            company_name = name,
        //            company_number = company_number,
        //            company_phone = phone,
        //            created_at = DateTime.Now,
        //            method = method,
        //            price = price,
        //            product = font.name,
        //            purpose = purpose,
        //            updated_at = DateTime.Now,
        //            user_seq = user_seq,
        //            font_seq = font_seq,
        //            is_personal = 0
        //        };
        //        db.Payment.Add(new_payment);

        //        var my_font = new MyFont
        //        {
        //            created_at = DateTime.Now,
        //            font_seq = font_seq,
        //            purchased_at = DateTime.Now,
        //            updated_at = DateTime.Now,
        //            user_seq = user_seq
        //        };
        //        db.MyFont.Add(my_font);
        //        db.SaveChanges();

        //        var result = new
        //        {
        //            state = "ok"
        //        };

        //        return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (HttpException e)
        //    {
        //        Response.StatusCode = e.GetHttpCode();
        //        return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        //    }
        //}

        // #0000 개인용 무료 다운로드 저장 
        [HttpPost]
        public JsonResult completefreepersonal(int user_seq, int font_seq)
        {
            try
            {
                var font = db.Font
                    .Where(f => !f.deleted_at.HasValue && f.seq == font_seq)
                    .FirstOrDefault();

                if (font == null)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, "no font");
                }

                var font_seq_string = font_seq.ToString();
                var new_payment = new Payment
                {  
                    created_at = DateTime.Now,
                    method = "FREE",
                    price = 0,
                    product = font.name,
                    updated_at = DateTime.Now,
                    user_seq = user_seq,
                    font_seq = font_seq,
                    fontids = font_seq_string,
                    is_personal = 1,
                    paid_at = DateTime.Now,
                    ok = 1
                };
                db.Payment.Add(new_payment);
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

        // #0000 상업용 무료 다운로드 저장 
        [HttpPost]
        public JsonResult completefreecommercial(int user_seq, int font_seq, string company_number, string email, string name, string phone, string purpose)
        {
            try
            {
                var font = db.Font
                    .Where(f => !f.deleted_at.HasValue && f.seq == font_seq)
                    .FirstOrDefault();

                if (font == null)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, "no font");
                }

                var font_seq_string = font_seq.ToString();

                var payment_record = db.Payment
                    .Where(p => !p.deleted_at.HasValue && p.user_seq == user_seq && p.fontids == font_seq_string)
                    .FirstOrDefault();

                if(payment_record != null)
                {
                    return Json(new { state = "duplicated"  }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }

                var new_payment = new Payment
                {
                    created_at = DateTime.Now,
                    method = "FREE",
                    price = 0,
                    product = font.name,
                    updated_at = DateTime.Now,
                    user_seq = user_seq,
                    font_seq = font_seq,
                    fontids = font_seq_string,
                    is_personal = 0,
                    paid_at = DateTime.Now,
                    company_number = company_number,
                    company_email = email,
                    company_name = name,
                    company_phone = phone,
                    purpose = purpose,
                    ok = 1
                };
                db.Payment.Add(new_payment);
                db.SaveChanges();

                var result = new
                {
                    state = "ok",
                    file_url = font.file_url
                };

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }

        // #0000 구매내역 목록 보기
        [HttpGet]
        public JsonResult List()
        {
            try
            {
                var model_list = new List<PaymentViewModel>();
                var payments = db.Payment
                    .Where(p => !p.deleted_at.HasValue && p.ok.HasValue && p.ok.Value == 1)
                    .OrderByDescending(p => p.created_at)
                    .ToList();

                var font_query = db.Font;

                foreach(var payment in payments)
                {
                    var new_model = new PaymentViewModel
                    {
                        company_mail = payment.company_email,
                        company_name = payment.company_name,
                        company_number = payment.company_number,
                        company_phone = payment.company_phone,
                        created_at_date = payment.created_at.HasValue ? payment.created_at.Value.ToString("yyyy-MM-dd") : null,
                        //font_name = payment.Font != null ? payment.Font.name : null,
                        payment_seq = payment.seq,
                        purpose = payment.purpose,
                        user_login_id = payment.User != null ? payment.User.login_id : null,
                        user_phone = payment.User != null ? payment.User.phone : null,
                        is_personal = payment.is_personal
                    };

                    

                    if (payment.fontids.Contains("/"))
                    {
                        var font_count = 0;
                        foreach(var id in payment.fontids.Split('/'))
                        {
                            if (id == "")
                            {
                                continue;
                            }
                            font_count++;
                        }

                        var first_font_id = payment.fontids.Split('/')[0];
                        var font_id_int = Convert.ToInt32(first_font_id);
                        var first_font = font_query
                            .Where(fq => fq.seq == font_id_int)
                            .FirstOrDefault();

                        if (first_font != null)
                        {
                            new_model.font_name = first_font.name + " 외 " + font_count + "개";
                        }
                    }
                    // font 1개
                    else
                    {
                        var font_id_int = Convert.ToInt32(payment.fontids);
                        var font = font_query
                            .Where(fq => fq.seq == font_id_int)
                            .FirstOrDefault();

                        if(font != null)
                        {
                            new_model.font_name = font.name;
                        }
                    }

                    if(payment.is_personal == 0)
                    {
                        new_model.font_use = "상업";
                    }
                    if(payment.is_personal == 1)
                    {
                        new_model.font_use = "개인";
                    }

                    model_list.Add(new_model);
                }

                var result = new
                {
                    state = "ok",
                    data = model_list
                };

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }

        // #0000 구매내역 상세
        [HttpGet]
        public JsonResult Detail(int payment_seq)
        {
            try
            {
                var payment = db.Payment
                    .Where(p => !p.deleted_at.HasValue && p.seq == payment_seq)
                    .FirstOrDefault();

                if(payment == null)
                {
                    return Json(new { state = "no payment" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }

                var new_model = new PaymentViewModel
                {
                    company_mail = payment.company_email,
                    company_name = payment.company_name,
                    company_number = payment.company_number,
                    company_phone = payment.company_phone,
                    created_at_date = payment.created_at.HasValue ? payment.created_at.Value.ToString("yyyy-MM-dd") : null,
                    font_name = payment.Font != null ? payment.Font.name : null,
                    payment_seq = payment.seq,
                    purpose = payment.purpose,
                    user_login_id = payment.User != null ? payment.User.login_id : null,
                    user_phone = payment.User != null ? payment.User.phone : null
                };

                if (payment.is_personal == 0)
                {
                    new_model.font_use = "상업";
                }
                if (payment.is_personal == 1)
                {
                    new_model.font_use = "개인";
                }

                var result = new
                {
                    state = "ok",
                    data = new_model
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