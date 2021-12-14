using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class ProductInputViewModel
    {
        public List<ProductItemsViewModel> Products { get; set; }
    }

    public class ProductItemsViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Amount { get; set; }
        public string ImageUrl { get; set; }
    }
}