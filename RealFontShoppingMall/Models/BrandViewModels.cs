using System;
using System.ComponentModel.DataAnnotations;

namespace RealFontShoppingMall.Models
{
    public class BrandViewModel
    {
        public int? brand_seq { get; set; }
        public string name { get; set; }
        public string info { get; set; }
        public string brand_url { get; set; }
        public string photo { get; set; }
        public int font_count { get; set; }
        public int? link_click_count { get; set; }
        public int? important_point { get; set; }
        public string created_at { get; set; }
    }
}
