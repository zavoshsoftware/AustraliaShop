using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class ProductListViewModel : _BaseViewModel
    {
        public ProductGroup ProductGroup { get; set; }
        public List<ProductItemViewModel> Products { get; set; }
        public List<BreadcrumbItem> BreadcrumbItems { get; set; }
        public List<PageItem> PageItems { get; set; }
       
    }

    public class ProductItemViewModel
    {
        public string Title { get; set; }
        public int Code { get; set; }
        public Guid Id { get; set; }
        public int CommentCount { get; set; }
        public string ImageUrl { get; set; }
        public string Summery { get; set; }
        public string Amount { get; set; }
        public string DiscountAmount { get; set; }
        public bool IsInPromotion { get; set; }
  
    }
    public class DealOfDayProductItemViewModel
    {
        public string Title { get; set; }
        public int Code { get; set; }
        public Guid Id { get; set; }
        public int CommentCount { get; set; }
        public string ImageUrl { get; set; }
        public string Summery { get; set; }
        public string Amount { get; set; }
        public string DiscountAmount { get; set; }
        public bool IsInPromotion { get; set; }
        public DateTime? DealOfDayExpireDate { get; set; }

        public string DealOfDayExpireDateStr
        {
            get
            {
                if (DealOfDayExpireDate == null)
                    return String.Empty;

                else
                {
                    string[] dateAndTime = DealOfDayExpireDate.ToString().Split(' ');

                    string[] dateSplit = dateAndTime[0].Split('/');

                    return dateSplit[2] + "/" + dateSplit[1] + "/" + dateSplit[0] + " " + dateAndTime[1];
                }
            }
        }

        public int AvailableForDealOfDay { get; set; }
        public int SoldInDealOfDay { get; set; }

        public int AvailableForDealOfDayPercent
        {
            get
            {
                double per = (double)(SoldInDealOfDay) / (double)AvailableForDealOfDay;
                return Convert.ToInt32(per * 100);
            }
        }
    }

    public class BreadcrumbItem
    {
        public string Title { get; set; }
        public string UrlParam { get; set; }
        public int Order { get; set; }
    }

    public class PageItem
    {
        public int PageId { get; set; }
        public bool IsCurrentPage { get; set; }
    }
}