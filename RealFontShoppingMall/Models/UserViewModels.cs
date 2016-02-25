using System;
using System.ComponentModel.DataAnnotations;

namespace RealFontShoppingMall.Models
{
    public class UserViewModel
    {
        public int user_seq { get; set; }
        public string login_id { get; set; }
        public int gender { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string created_at { get; set; }
        public int role { get; set; }
    }

    public class UserRegisterViewModel
    {
        public string login_id { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int gender { get; set; }
    }


}
