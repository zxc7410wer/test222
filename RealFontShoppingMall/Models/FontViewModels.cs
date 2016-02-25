using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealFontShoppingMall.Models
{
    public class FontViewModel
    {
        public int font_seq { get; set; }
        public BrandViewModel brand_info { get; set; }
        public string name { get; set; }
        public int is_free { get; set; }
        public int is_personal { get; set; }
        public int price { get; set; }
        public int price_commercial { get; set; }
        public int period { get; set; }
        public List<string> photos { get; set; }
        public string seller_name { get; set; }
        public string info { get; set; }
        public string intro { get; set; }
        public string format { get; set; }
        public string os { get; set; }
        public string created_at { get; set; }
        public string file_url { get; set; }
        public string link_url { get; set; }
        public int? click_count { get; set; }
        public int? important_point { get; set; }

        public SimpleFontViewModel prev_font { get; set; }
        public SimpleFontViewModel next_font { get; set; }
    }

    public class SimpleFontViewModel
    {
        public int font_seq { get; set; }
        public string name { get; set; }
        public string created_at { get; set; }
        public int is_personal { get; set; }
    }

    //public class MyFontViewModel
    //{
    //    public FontViewModel font_info { get; set; }
    //    public UserViewModel user_info { get; set; }
    //    public DateTime? purchased_at { get; set; }
    //}

    public class MyFontViewModel
    {
        public int my_font_seq { get; set; }
        public int user_seq { get; set; }
        public int? is_personal { get; set; }
        public FontViewModel font_info { get; set; }
        public string purchased_at { get; set; }
    }
}
