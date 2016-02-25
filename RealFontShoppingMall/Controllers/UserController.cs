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
    public class UserController : Controller
    {
        RealFontShoppingmallEntities db = new RealFontShoppingmallEntities();
        //
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /User/
        public ActionResult Register()
        {
            return View();
        }

        // #0000 유저 목록 가져오기
        [HttpGet]
        public JsonResult List()
        {
            try
            {
                var user_list = new List<UserViewModel>();
                var query = db.User
                    .Where(u => !u.deleted_at.HasValue && u.role_type == Role.user);

                foreach (var user in query)
                {
                    var model = new UserViewModel
                    {
                        created_at = user.created_at.HasValue ? user.created_at.Value.ToString("yyyy-MM-dd") : null,
                        email = user.email,
                        gender = user.gender.HasValue ? user.gender.Value : 0,
                        login_id = user.login_id,
                        phone = user.phone,
                        user_seq = user.seq,
                        role = user.role_type.HasValue ? user.role_type.Value : Role.user
                    };
                    user_list.Add(model);
                }

                var result = new
                {
                    state = "ok",
                    data = user_list,
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

        // #0000 유저 정보
        [HttpGet]
        public JsonResult Detail(int user_seq)
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

                var data = new UserViewModel
                {
                    created_at = user.created_at.HasValue ? user.created_at.Value.ToString("yyyy-MM-dd") : null,
                    email = user.email,
                    gender = user.gender.HasValue ? user.gender.Value : 0,
                    login_id = user.login_id,
                    phone = user.phone,
                    user_seq = user.seq,
                    role = user.role_type.HasValue ? user.role_type.Value : Role.user
                };

                var result = new
                {
                    state = "ok",
                    data = data
                };

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }

        // #0000 로그인
        [HttpPost]
        public JsonResult Login(string login_id, string password)
        {
            try
            {
                var user = db.User
                    .Where(u => !u.deleted_at.HasValue && u.login_id == login_id)
                    .FirstOrDefault();

                if (user == null)
                {
                    return Json(new { state = "no user" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }

                if (user.password != password)
                {
                    return Json(new { state = "wrong password" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }

                var data = new UserViewModel
                {
                    created_at = user.created_at.HasValue ? user.created_at.Value.ToString("yyyy-MM-dd") : null,
                    email = user.email,
                    login_id = user.login_id,
                    gender = user.gender.HasValue ? user.gender.Value : 0,
                    phone = user.phone,
                    user_seq = user.seq
                };

                var result = new
                {
                    state = "ok",
                    data = data
                };

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }

        // #0000 회원가입
        [HttpPost]
        public JsonResult Register(UserRegisterViewModel dataModel)
        {
            try
            {
                var user_count = db.User
                    .Count(u => !u.deleted_at.HasValue && u.login_id == dataModel.login_id);

                if(user_count > 0)
                {
                    return Json(new { state = "duplicated" }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }

                var new_user = new User
                {
                    created_at = DateTime.Now,
                    email = dataModel.email,
                    gender = dataModel.gender,
                    login_id = dataModel.login_id,
                    password = dataModel.password,
                    phone = dataModel.phone,
                    role_type = Role.user,
                    updated_at = DateTime.Now
                };

                db.User.Add(new_user);
                db.SaveChanges();

                var data = new UserViewModel
                {
                    created_at = new_user.created_at.HasValue ? new_user.created_at.Value.ToString("yyyy-MM-dd") : null,
                    email = new_user.email,
                    login_id = new_user.login_id,
                    gender = new_user.gender.HasValue ? new_user.gender.Value : 0,
                    phone = new_user.phone,
                    user_seq = new_user.seq
                };

                var result = new
                {
                    state = "ok",
                    data = data
                };

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (HttpException e)
            {
                Response.StatusCode = e.GetHttpCode();
                return Json(e.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }

        // #0000 비밀번호 변경
        [HttpPost]
        public JsonResult ChangePassword(int user_seq, string new_password)
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

                user.password = new_password;
                user.updated_at = DateTime.Now;
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

        // #0000 회원정보 변경
        [HttpPost]
        public JsonResult Update(int user_seq, string email, string phone, int gender)
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

                user.email = email;
                user.phone = phone;
                user.gender = gender;
                user.updated_at = DateTime.Now;
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

        // #0000 회원탈퇴
        [HttpPost]
        public JsonResult Unregister(int user_seq)
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
                
                user.deleted_at = DateTime.Now;
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