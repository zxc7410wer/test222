using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealFontShoppingMall.Models
{
    public class PaymentViewModel
    {
        public int payment_seq { get; set; }
        public string font_name { get; set; }
        public string user_login_id { get; set; }
        public string user_phone { get; set; }
        public string font_use { get; set; }
        public string created_at_date { get; set; }

        public string company_name { get; set; }
        public string company_number { get; set; }
        public string company_mail { get; set; }
        public string company_phone { get; set; }
        public string purpose { get; set; }
        public int? is_personal { get; set; }
    }
}
