using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealFontShoppingMall.Models
{
    public class BlogViewModel
    {
        public int blog_seq { get; set; }
        public string name { get; set; }
        public string info { get; set; }
        public string link_url { get; set; }
        public List<string> photos { get; set; }
        public string photo { get;set; }
        public string tags { get; set; }
        public string color { get; set; }
        public int? important_point { get; set; }
        public string created_at { get; set; }
    }
}
