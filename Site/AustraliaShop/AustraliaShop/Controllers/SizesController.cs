using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;

namespace AustraliaShop.Controllers
{
    public class SizesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Sizes
        public ActionResult Index()
        {
            return View(db.Sizes.Where(a=>a.IsDeleted==false).OrderByDescending(a=>a.CreationDate).ToList());
        }
        
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Order,Title,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] Size size)
        {
            if (ModelState.IsValid)
            {
				size.IsDeleted=false;
				size.CreationDate= DateTime.Now; 
					
                size.Id = Guid.NewGuid();
                db.Sizes.Add(size);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(size);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Size size = db.Sizes.Find(id);
            if (size == null)
            {
                return HttpNotFound();
            }
            return View(size);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Order,Title,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] Size size)
        {
            if (ModelState.IsValid)
            {
				size.IsDeleted=false;
					size.LastModifiedDate=DateTime.Now;
                db.Entry(size).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(size);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Size size = db.Sizes.Find(id);
            if (size == null)
            {
                return HttpNotFound();
            }
            return View(size);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Size size = db.Sizes.Find(id);
			size.IsDeleted=true;
			size.DeletionDate=DateTime.Now;
 
            db.SaveChanges();
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
    }
}
