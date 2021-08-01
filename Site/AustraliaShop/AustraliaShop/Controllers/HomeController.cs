using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using ViewModels;

namespace AustraliaShop.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            HomeViewModel home = new HomeViewModel()
            {
                Sliders = db.Sliders.Where(c => c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList(),
                NewProducts = GetProductList("NewProducts"),
                BestSaleProducts = GetProductList("BestSaleProducts"),
                SpecialOfferProducts = GetProductList("SpecialOfferProducts"),
                TopRatedProducts = GetProductList("TopRatedProducts"),
                DealOfDayProducts = GetDealOfDayProductList(),
                ProductGroups = db.ProductGroups.Where(c=>c.IsInHome&&c.IsDeleted==false&&c.IsActive).Take(3).ToList()
            };
            return View(home);
        }

        public List<DealOfDayProductItemViewModel> GetDealOfDayProductList( )
        {
            List<DealOfDayProductItemViewModel> productList=new List<DealOfDayProductItemViewModel>();
           var products = db.Products.Where(c =>
                c.IsDealOfDay && c.IsDeleted == false && c.IsActive).Select(c => new
            {
                c.Id,
                c.Title,
                c.ImageUrl,
                c.Code,
                c.IsInPromotion,
                c.Summery,
                c.Amount,
                c.DiscountAmount,
                c.DealOfDayExpireDate,
                c.AvailableForDealOfDay,
                c.SoldInDealOfDay
            });


            foreach (var product in products)
            {
                productList.Add(new DealOfDayProductItemViewModel()
                {
                    Title = product.Title,
                    ImageUrl = product.ImageUrl,
                    Amount = product.Amount.ToString("N0"),
                    CommentCount = db.ProductComments.Count(c => c.ProductId == product.Id && c.IsActive && c.IsDeleted == false),
                    Id = product.Id,
                    DiscountAmount = product.DiscountAmount?.ToString("N0"),
                    Summery = product.Summery,
                    IsInPromotion = product.IsInPromotion,
                    Code = product.Code,
                    DealOfDayExpireDate = product.DealOfDayExpireDate,
                    AvailableForDealOfDay = product.AvailableForDealOfDay,
                    SoldInDealOfDay = product.SoldInDealOfDay
                });
            }

            return productList;
        }

        public List<ProductItemViewModel> GetProductList(string typeTitle)
        {
            List<ProductItemViewModel> productList = new List<ProductItemViewModel>();

            if (typeTitle == "NewProducts")
            {
                var products = db.Products.Where(c =>
                    c.IsNewArrival&&c.ParentId==null && c.IsDeleted == false && c.IsActive).Select(c => new
                    {
                        c.Id,
                        c.Title,
                        c.ImageUrl,
                        c.Code,
                        c.IsInPromotion,
                        c.Summery,
                        c.Amount,
                        c.DiscountAmount,
                        c.DealOfDayExpireDate
                    }).Take(8);


                foreach (var product in products)
                {
                    productList.Add(new ProductItemViewModel()
                    {
                        Title = product.Title,
                        ImageUrl = product.ImageUrl,
                        Amount = product.Amount.ToString("N0"),
                        CommentCount = db.ProductComments.Count(c => c.ProductId == product.Id && c.IsActive && c.IsDeleted == false),
                        Id = product.Id,
                        DiscountAmount = product.DiscountAmount?.ToString("N0"),
                        Summery = product.Summery,
                        IsInPromotion = product.IsInPromotion,
                        Code = product.Code,
                    });
                }
            }

           else if (typeTitle == "BestSaleProducts")
            {
                var products = db.Products.Where(c =>
                    c.IsBestSale && c.ParentId == null && c.IsDeleted == false && c.IsActive).Select(c => new
                    {
                        c.Id,
                        c.Title,
                        c.ImageUrl,
                        c.Code,
                        c.IsInPromotion,
                        c.Summery,
                        c.Amount,
                        c.DiscountAmount
                    }).Take(8);


                foreach (var product in products)
                {
                    productList.Add(new ProductItemViewModel()
                    {
                        Title = product.Title,
                        ImageUrl = product.ImageUrl,
                        Amount = product.Amount.ToString("N0"),
                        CommentCount = db.ProductComments.Count(c => c.ProductId == product.Id && c.IsActive && c.IsDeleted == false),
                        Id = product.Id,
                        DiscountAmount = product.DiscountAmount?.ToString("N0"),
                        Summery = product.Summery,
                        IsInPromotion = product.IsInPromotion,
                        Code = product.Code,

                    });
                }
            }
            else if (typeTitle == "SpecialOfferProducts")
            {
                var products = db.Products.Where(c =>
                    c.IsSpecialOffer && c.ParentId == null && c.IsDeleted == false && c.IsActive).Select(c => new
                    {
                        c.Id,
                        c.Title,
                        c.ImageUrl,
                        c.Code,
                        c.IsInPromotion,
                        c.Summery,
                        c.Amount,
                        c.DiscountAmount
                    }).Take(8);


                foreach (var product in products)
                {
                    productList.Add(new ProductItemViewModel()
                    {
                        Title = product.Title,
                        ImageUrl = product.ImageUrl,
                        Amount = product.Amount.ToString("N0"),
                        CommentCount = db.ProductComments.Count(c => c.ProductId == product.Id && c.IsActive && c.IsDeleted == false),
                        Id = product.Id,
                        DiscountAmount = product.DiscountAmount?.ToString("N0"),
                        Summery = product.Summery,
                        IsInPromotion = product.IsInPromotion,
                        Code = product.Code,

                    });
                }
            }

            else if (typeTitle == "TopRatedProducts")
            {
                var products = db.Products.Where(c =>
                    c.IsTopRate && c.ParentId == null && c.IsDeleted == false && c.IsActive).Select(c => new
                    {
                        c.Id,
                        c.Title,
                        c.ImageUrl,
                        c.Code,
                        c.IsInPromotion,
                        c.Summery,
                        c.Amount,
                        c.DiscountAmount
                    }).Take(8);


                foreach (var product in products)
                {
                    productList.Add(new ProductItemViewModel()
                    {
                        Title = product.Title,
                        ImageUrl = product.ImageUrl,
                        Amount = product.Amount.ToString("N0"),
                        CommentCount = db.ProductComments.Count(c => c.ProductId == product.Id && c.IsActive && c.IsDeleted == false),
                        Id = product.Id,
                        DiscountAmount = product.DiscountAmount?.ToString("N0"),
                        Summery = product.Summery,
                        IsInPromotion = product.IsInPromotion,
                        Code = product.Code,

                    });
                }
            }

           
            return productList;
        }
        [Route("about")]
        public ActionResult About()
        {
            AboutViewModel home = new AboutViewModel();
            return View(home);
        }
        [Route("contact")]
        public ActionResult Contact()
        {
            ContactViewModel home = new ContactViewModel();
            return View(home);
        }
        public ActionResult Basket()
        {
            return View();
        }
        public ActionResult CheckOut()
        {
            return View();
        }
        [Route("paymentGateway")]
        public ActionResult PaymentGateway()
        {
            return View();
        }

    }
}