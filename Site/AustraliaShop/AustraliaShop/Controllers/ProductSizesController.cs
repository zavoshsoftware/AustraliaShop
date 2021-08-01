using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;
using ViewModels;

namespace AustraliaShop.Controllers
{
    public class ProductSizesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index(Guid id)
        {
            var product = db.Products.Find(id);

            if (product != null)
            {
                ViewBag.Title = product.Title + " | color: " + product.Color.Title;
            }
            var productSizes = db.ProductSizes.Include(p => p.Product).Where(p=>p.ProductId==id&& p.IsDeleted==false).OrderByDescending(p=>p.CreationDate);
            return View(productSizes.ToList());
        }
         
        public ActionResult Create(Guid id)
        {
            ViewBag.ProductId = id;
           // ViewBag.SizeId = new SelectList(db.Sizes, "Id", "Title");

            ProductSizeCreateViewModel result=new ProductSizeCreateViewModel()
            {
                Sizes = GetProductSizeLists()
            };
            return View(result);
        }

        public List<SizeCheckboxList> GetProductSizeLists( )
        {
            List<SizeCheckboxList> list = new List<SizeCheckboxList>();

            List<Size> sizes = db.Sizes
                .Where(c => c.IsDeleted == false && c.IsActive ).OrderBy(c => c.Title).ToList();

            foreach (Size size in sizes)
            {
               

                list.Add(new SizeCheckboxList()
                {
                    Id = size.Id,
                    Title = size.Title,
                    IsSelected = false
                });
            }

            return list.OrderBy(c => c.Title).ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductSizeCreateViewModel viewmodel, Guid id)
        {
            if (ModelState.IsValid)
            {
                foreach (var size in viewmodel.Sizes)
                {
                    if (size.IsSelected)
                    {
                        ProductSize productSize = new ProductSize();

                        productSize.SizeId = size.Id;
                        productSize.Stock = viewmodel.Stock;
                        productSize.IsDeleted = false;
                        productSize.CreationDate = DateTime.Now;
                        productSize.ProductId = id;
                        productSize.IsActive = true;
                        productSize.Id = Guid.NewGuid();
                        db.ProductSizes.Add(productSize);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index",new{id=id});
            }

            ViewBag.ProductId = id;
            ProductSizeCreateViewModel result = new ProductSizeCreateViewModel()
            {
                Sizes = GetProductSizeLists()
            };
            return View(result);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSize productSize = db.ProductSizes.Find(id);
            if (productSize == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId =   productSize.ProductId;
            ViewBag.SizeId = new SelectList(db.Sizes, "Id", "Title", productSize.SizeId);
            return View(productSize);
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductSize productSize)
        {
            if (ModelState.IsValid)
            {
				productSize.IsDeleted=false;
					productSize.LastModifiedDate=DateTime.Now;
                db.Entry(productSize).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index",new{id=productSize.ProductId});
            }

            ViewBag.ProductId =  productSize.ProductId;
            ViewBag.SizeId = new SelectList(db.Sizes, "Id", "Title", productSize.SizeId);
            return View(productSize);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSize productSize = db.ProductSizes.Find(id);
            if (productSize == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = productSize.ProductId;

            return View(productSize);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ProductSize productSize = db.ProductSizes.Find(id);
			productSize.IsDeleted=true;
			productSize.DeletionDate=DateTime.Now;
 
            db.SaveChanges();
            return RedirectToAction("Index", new { id = productSize.ProductId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
