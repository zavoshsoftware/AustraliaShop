using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class ProductDetailViewModel : _BaseViewModel
    {
        public Product Product { get; set; }
        public List<ProductComment> ProductComments { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public ProductGroup ProductGroup { get; set; }
        public List<BreadcrumbItem> BreadcrumbItems { get; set; }
        public List<ProductImageViewModel> ProductImages { get; set; }
        //public List<ProductSize> ProductSizes { get; set; }
        public List<ChildProduct> ChildProducts { get; set; }
        public int CommentQty
        {
            get { return ProductComments.Count(); }
        }
    }

    public class ProductImageViewModel
    {
        public string ImageUrl { get; set; }
        public string Alt { get; set; }
    }

    public class ChildProduct
    {
        public Product Product { get; set; }
        public List<ProductSize> ProductSizes { get; set; }
    }

}