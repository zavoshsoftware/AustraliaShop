using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Models;
using ViewModels;

namespace Helpers
{
    public class ProductHelper
    {
        private DatabaseContext db = new DatabaseContext();

        private readonly int _productPagination = Convert.ToInt32(WebConfigurationManager.AppSettings["productPaginationSize"]);

        public List<ProductImageViewModel> GetProductImages(Guid productId)
        {
            List<ProductImageViewModel> result = new List<ProductImageViewModel>();

            var productImages =
                db.ProductImages.Where(c => c.ProductId == productId && c.IsActive && c.IsDeleted == false)
                    .OrderBy(c => c.Order);

            foreach (var image in productImages)
            {
                result.Add(new ProductImageViewModel()
                {
                    ImageUrl = image.ImageUrl,
                    Alt = image.AltText
                });
            }
            
            return result;
        }


        public List<BreadcrumbItem> GetBreadCrumb(ProductGroup currenProductGroup)
        {
            List<BreadcrumbItem> result = new List<BreadcrumbItem>();

            //result.Add(GetBreadcrumbItem(currenProductGroup, 10));

            for (int i = 9; i > 1; i--)
            {
                if (currenProductGroup != null)
                {
                    result.Add(GetBreadcrumbItem(currenProductGroup, i));
                    currenProductGroup = GetRecursive(currenProductGroup);
                }
                else
                {
                    break;
                }
            }

            return result;
        }


        public List<PageItem> GetPagination(int productCount, int? pageId)
        {
            List<PageItem> result = new List<PageItem>();

            int pageNumbers = (int)Math.Ceiling(productCount / (double)_productPagination);

            for (int i = 1; i <= pageNumbers; i++)
            {
                bool isActive = pageId == i;

                PageItem pageItem = new PageItem()
                {
                    PageId = i,
                    IsCurrentPage = isActive
                };
                result.Add(pageItem);
            }

            return result;
        }
        public ProductGroup GetRecursive(ProductGroup currenProductGroup)
        {
            if (currenProductGroup.ParentId != null)
                return currenProductGroup.Parent;

            return null;
        }

        public BreadcrumbItem GetBreadcrumbItem(ProductGroup currentProductGroup, int order)
        {
            BreadcrumbItem breadcrumb = new BreadcrumbItem()
            {
                Title = currentProductGroup.Title,
                UrlParam = currentProductGroup.UrlParam,
                Order = order
            };

            return breadcrumb;
        }

        //public string GetUrl(string[] brands, string urlParam)
        //{
        //    string url = "/category/" + urlParam;
          
        //    if (brands != null)
        //    {
        //        url += "?";
        //        for (int i = 0; i < brands.Length; i++)
        //        {
        //            url += "brands[" + i + "]=" + brands[i];

        //            if (i != brands.Length - 1)
        //                url += "&";
        //        }
              
        //    }


        //    return url;
        //}
    }
}