using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class ProductSizeCreateViewModel
    {
        public List<SizeCheckboxList> Sizes { get; set; }
        public int Stock { get; set; }

    }

    public class SizeCheckboxList
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsSelected { get; set; }
    }
}