//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 템플릿에서 생성되었습니다.
//
//     이 파일을 수동으로 변경하면 응용 프로그램에서 예기치 않은 동작이 발생할 수 있습니다.
//     이 파일을 수동으로 변경하면 코드가 다시 생성될 때 변경 내용을 덮어씁니다.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RealFontShoppingMall
{
    using System;
    using System.Collections.Generic;
    
    public partial class Brand
    {
        public Brand()
        {
            this.Font = new HashSet<Font>();
        }
    
        public int seq { get; set; }
        public string name { get; set; }
        public string info { get; set; }
        public string brand_url { get; set; }
        public string photo { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public Nullable<System.DateTime> deleted_at { get; set; }
        public Nullable<int> link_click_count { get; set; }
        public Nullable<int> important_point { get; set; }
    
        public virtual ICollection<Font> Font { get; set; }
    }
}