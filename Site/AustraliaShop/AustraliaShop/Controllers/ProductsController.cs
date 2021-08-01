using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Helpers;
using Models;
using ViewModels;

namespace AustraliaShop.Controllers
{
    public class ProductsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        private readonly int _productPagination = Convert.ToInt32(WebConfigurationManager.AppSettings["productPaginationSize"]);

        #region CRUD

        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.ProductGroup).Where(p => p.ParentId == null && p.IsDeleted == false)
                .OrderByDescending(p => p.CreationDate);

            return View(products.ToList());
        }
        public ActionResult IndexColor(Guid id)
        {
            var products = db.Products.Include(p => p.ProductGroup).Where(p => p.ParentId == id && p.IsDeleted == false)
                .OrderByDescending(p => p.CreationDate);

            ViewBag.parentId = id;
            return View(products.ToList());
        }

        public ActionResult CreateColor(Guid id)
        {
            ViewBag.ColorId = new SelectList(db.Colors.Where(c => c.IsDeleted == false).OrderBy(c => c.Title), "Id", "Title");
            ViewBag.ParentId = new SelectList(db.Products.Where(c => c.ParentId == null), "Id", "Title");
            ViewBag.SupplierId = new SelectList(db.Suppliers.Where(c => c.IsDeleted == false), "Id", "Title");

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateColor(ProductColorCrudViewModel productColor, Guid id)
        {
            if (ModelState.IsValid)
            {
                if (db.Products.Any(c => c.ParentId == id && c.ColorId == productColor.ColorId && c.IsDeleted == false))
                {
                    ModelState.AddModelError("duplicate", "this color submited for this product previously");

                    ViewBag.ColorId = new SelectList(db.Colors.Where(c => c.IsDeleted == false).OrderBy(c => c.Title), "Id", "Title");
                    return View(productColor);
                }
                CodeGenerator codeGenerator = new CodeGenerator();
                Product parentProduct = db.Products.Find(id);
                Product product = new Product()
                {
                    ParentId = id,
                    Id = Guid.NewGuid(),
                    DealOfDayExpireDate = parentProduct.DealOfDayExpireDate,
                    Title = parentProduct.Title,
                    Amount = parentProduct.Amount,
                    AvailableForDealOfDay = parentProduct.AvailableForDealOfDay,
                    Body = parentProduct.Body,
                    Code = codeGenerator.ReturnProductCode(),
                    ColorId = productColor.ColorId,
                    CreationDate = DateTime.Now,
                    DiscountAmount = parentProduct.DiscountAmount,
                    ImageUrl = parentProduct.ImageUrl,
                    IsActive = parentProduct.IsActive,
                    IsAvailable = parentProduct.IsAvailable,
                    IsBestSale = parentProduct.IsBestSale,
                    IsDealOfDay = parentProduct.IsDealOfDay,
                    IsInPromotion = parentProduct.IsInPromotion,
                    IsDeleted = false,
                    IsNewArrival = parentProduct.IsNewArrival,
                    IsSpecialOffer = parentProduct.IsSpecialOffer,
                    IsTopRate = parentProduct.IsTopRate,
                    Order = parentProduct.Order,
                    PageTitle = parentProduct.PageTitle,
                    PageDescription = parentProduct.PageDescription,
                    ProductGroupId = parentProduct.ProductGroupId,
                    SeedStock = parentProduct.SeedStock,
                    Visit = parentProduct.Visit,
                    Stock = parentProduct.Stock,
                    SoldInDealOfDay = parentProduct.SoldInDealOfDay,
                    SellNumber = 0,
                    Summery = parentProduct.Summery,

                };

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("IndexColor", new { id = id });
            }
            ViewBag.ColorId = new SelectList(db.Colors.Where(c => c.IsDeleted == false).OrderBy(c => c.Title), "Id", "Title");

            return View(productColor);
        }


        public ActionResult Create()
        {
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Title");
            ViewBag.ColorId = new SelectList(db.Colors.Where(c => c.IsDeleted == false).OrderBy(c => c.Title), "Id", "Title");
            ViewBag.SupplierId = new SelectList(db.Suppliers.Where(c => c.IsDeleted == false), "Id", "Title");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                string newFilenameUrl = string.Empty;
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/Product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    product.ImageUrl = newFilenameUrl;
                }
                #endregion

                CodeGenerator codeGenerator = new CodeGenerator();
                product.ParentId = null;
                product.IsDeleted = false;
                product.Code = codeGenerator.ReturnProductCode();
                product.SeedStock = product.SeedStock;
                product.CreationDate = DateTime.Now;
                product.Visit = 0;
                product.SellNumber = 0;

                product.Id = Guid.NewGuid();
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ColorId = new SelectList(db.Colors.Where(c => c.IsDeleted == false).OrderBy(c => c.Title), "Id", "Title");
            ViewBag.SupplierId = new SelectList(db.Suppliers.Where(c => c.IsDeleted == false), "Id", "Title");

            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Title", product.ProductGroupId);
            return View(product);
        }

        public ActionResult EditColor(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ProductColorCrudViewModel productColor = new ProductColorCrudViewModel()
            {
                IsActive = product.IsActive,
                ProductId = id
            };
            ViewBag.ColorId = new SelectList(db.Colors.Where(c => c.IsDeleted == false).OrderBy(c => c.Title), "Id", "Title", product.ColorId);

            return View(productColor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditColor(ProductColorCrudViewModel productColor, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {

                Product product = db.Products.FirstOrDefault(c => c.Id == productColor.ProductId);
                product.IsActive = productColor.IsActive;
                product.ColorId = productColor.ColorId;
                product.LastModifiedDate = DateTime.Now;

                db.SaveChanges();
                return RedirectToAction("IndexColor", new { id = product.ParentId });
            }
            ViewBag.ColorId = new SelectList(db.Colors.Where(c => c.IsDeleted == false).OrderBy(c => c.Title), "Id", "Title", productColor.ColorId);

            return View(productColor);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Title", product.ProductGroupId);
            ViewBag.SupplierId = new SelectList(db.Suppliers.Where(c => c.IsDeleted == false), "Id", "Title", product.SupplierId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                string newFilenameUrl = string.Empty;
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/Product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    product.ImageUrl = newFilenameUrl;
                }
                #endregion
                product.IsDeleted = false;
                product.LastModifiedDate = DateTime.Now;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SupplierId = new SelectList(db.Suppliers.Where(c => c.IsDeleted == false), "Id", "Title", product.SupplierId);
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Title", product.ProductGroupId);
            return View(product);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Product product = db.Products.Find(id);
            product.IsDeleted = true;
            product.DeletionDate = DateTime.Now;

            db.SaveChanges();
            if (product.ParentId != null)
                return RedirectToAction("Index", new { id = product.ParentId });

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
        ProductHelper productHelper = new ProductHelper();


        [AllowAnonymous]
        [Route("category/{urlParam}")]
        public ActionResult List(string urlParam, string[] brands, int? pageId, string sortby)
        {

            ProductGroup productGroup = db.ProductGroups.FirstOrDefault(c => c.UrlParam == urlParam);

            if (productGroup == null)
                return Redirect("/");


            //ViewBag.url = GetUrl(brands, urlParam);

            if (pageId == null)
                pageId = 1;

            var products = db.Products.Where(c => c.ParentId == null &&
                 (c.ProductGroupId == productGroup.Id || c.ProductGroup.ParentId == productGroup.Id) &&
                 c.IsDeleted == false && c.IsActive).Select(c => new
                 {
                     c.Id,
                     c.Title,
                     c.ImageUrl,
                     c.Code,
                     c.IsInPromotion,
                     c.Summery,
                     c.Amount,
                     c.DiscountAmount,
                     c.CreationDate
                 }).OrderByDescending(c => c.CreationDate);

            List<ProductItemViewModel> productList = new List<ProductItemViewModel>();

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

            if (products.Count() <= _productPagination)
                ViewBag.isLastPage = "true";
            else
                ViewBag.isLastPage = "false";

            ProductListViewModel result = new ProductListViewModel()
            {
                ProductGroup = productGroup,
                Products = productList.Skip(_productPagination * (pageId.Value - 1)).Take(_productPagination).ToList(),
                BreadcrumbItems = productHelper.GetBreadCrumb(productGroup.Parent).OrderBy(c => c.Order).ToList(),
                PageItems = productHelper.GetPagination(products.Count(), pageId),
            };

            return View(result);
        }

        [AllowAnonymous]
        [Route("product/{code}")]
        public ActionResult Details(string code)
        {
            if (code == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int proCode = Convert.ToInt32(code);

            Product product = db.Products.FirstOrDefault(c => c.Code == proCode);

            if (product == null)
            {
                return HttpNotFound();
            }
            product.Visit += 1;
            db.SaveChanges();

            ProductGroup productGroup = product.ProductGroup;

            ProductDetailViewModel productDetail = new ProductDetailViewModel()
            {
                Product = product,

                ProductComments =
                    db.ProductComments
                        .Where(c => c.ProductId == product.Id && c.IsActive == true && c.IsDeleted == false)
                        .OrderByDescending(c => c.CreationDate).ToList(),

                RelatedProducts = db.Products.Where(c => c.ParentId == null && c.ProductGroupId == product.ProductGroupId && c.IsDeleted == false && c.IsActive)
                    .OrderByDescending(c => c.CreationDate).Take(6).ToList(),

                ProductGroup = productGroup,
                BreadcrumbItems = productHelper.GetBreadCrumb(productGroup).OrderBy(c => c.Order).ToList(),
                ProductImages = productHelper.GetProductImages(product.Id),
                ChildProducts = GetProductChildren(product.Id)
                //ProductSizes = db.ProductSizes.Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive && c.Stock > 0).ToList()
            };
            return View(productDetail);

        }

        public List<ChildProduct> GetProductChildren(Guid parentId)
        {

            List<ChildProduct> childProducts = new List<ChildProduct>();
            var children = db.Products.Where(c => c.ParentId == parentId && c.IsDeleted == false && c.IsActive);

            foreach (var child in children)
            {
                List<ProductSize> productSizes = db.ProductSizes.Where(c =>
                    c.ProductId == child.Id && c.IsDeleted == false && c.IsActive && c.Stock > 0).OrderBy(c => c.Size.Title).ToList();

                if (productSizes.Any())
                {
                    childProducts.Add(new ChildProduct()
                    {
                        Product = child,
                        ProductSizes = productSizes
                    });
                }
            }
            return childProducts;
        }
    }
}
