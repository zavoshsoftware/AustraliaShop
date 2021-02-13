using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Models;
using ViewModels;

namespace AustraliaShop.Controllers
{
    public class BlogsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Blogs
        public ActionResult Index()
        {
            var blogs = db.Blogs.Include(b => b.BlogGroup).Where(b=>b.IsDeleted==false).OrderByDescending(b=>b.CreationDate);
            return View(blogs.ToList());
        }

        // GET: Blogs/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }
        
        public ActionResult Create()
        {
            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Blog blog, HttpPostedFileBase fileupload)
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

                    newFilenameUrl = "/Uploads/Blog/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    blog.ImageUrl = newFilenameUrl;
                }
                #endregion

                blog.IsDeleted=false;
				blog.CreationDate= DateTime.Now;
                blog.Visit = 0;

                blog.Id = Guid.NewGuid();
                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title", blog.BlogGroupId);
            return View(blog);
        }

        // GET: Blogs/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title", blog.BlogGroupId);
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Blog blog, HttpPostedFileBase fileupload)
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

                    newFilenameUrl = "/Uploads/Blog/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    blog.ImageUrl = newFilenameUrl;
                }
                #endregion
                blog.IsDeleted=false;
					blog.LastModifiedDate=DateTime.Now;
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title", blog.BlogGroupId);
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Blog blog = db.Blogs.Find(id);
			blog.IsDeleted=true;
			blog.DeletionDate=DateTime.Now;
 
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

        [Route("blog")]
        [AllowAnonymous]
        public ActionResult List()
        {
            List<Blog> blogs = db.Blogs.Where(c => c.IsDeleted == false && c.IsActive)
                .OrderByDescending(c => c.CreationDate).ToList();

            BlogListViewModel blogList = new BlogListViewModel()
            {
                Blogs = blogs
            };
            return View(blogList);
        }


        [Route("blog/{urlParam}")]
        [AllowAnonymous]
        public ActionResult ListByGroup(string urlParam)
        {
            BlogGroup blogGroup = db.BlogGroups.FirstOrDefault(c => c.UrlParam == urlParam && c.IsActive);

            if (blogGroup == null)
                return Redirect("/blog");

            List<Blog> blogs = db.Blogs.Where(c => c.BlogGroupId == blogGroup.Id && c.IsDeleted == false && c.IsActive)
                .OrderByDescending(c => c.CreationDate).ToList();

            BlogListViewModel blogList = new BlogListViewModel()
            {
                Blogs = blogs,
                BlogGroup = blogGroup
            };
            return View(blogList);
        }


        [Route("blog/post/{urlParam}")]
        [AllowAnonymous]
        public ActionResult Details(string urlParam)
        {

            Blog blog = db.Blogs.FirstOrDefault(c => c.UrlParam == urlParam);
            if (blog == null)
            {
                return Redirect("/blog");
            }

            blog.Visit++;
            db.SaveChanges();

            BlogDetailViewModel detail = new BlogDetailViewModel()
            {
                Blog = blog,
                BlogComments = db.BlogComments.Where(c => c.BlogId == blog.Id && c.IsActive && c.IsDeleted == false).ToList(),
                SidebarBlogGroups = GetSidebarBlogGroups(),
                SidebarBlogs = GetSidebarBlogs(),
                RelatedBlog = GetRelatedBlogs(blog.BlogGroupId),

            };

            return View(detail);
        }

        public List<BlogGroupItemViewModel> GetSidebarBlogGroups()
        {
            List<BlogGroupItemViewModel> result = new List<BlogGroupItemViewModel>();

            var blogGroups = db.BlogGroups.Where(c => c.IsDeleted == false && c.IsActive).Select(c => new
            {
                c.Id,
                c.Title,
                c.UrlParam
            });

            foreach (var blogGroup in blogGroups)
            {
                int blogCount =
                    db.Blogs.Count(c => c.BlogGroupId == blogGroup.Id && c.IsDeleted == false && c.IsActive);

                result.Add(new BlogGroupItemViewModel()
                {
                    Title = blogGroup.Title,
                    UrlParam = blogGroup.UrlParam,
                    BlogCount = blogCount
                });
            }
            return result;
        }
        public List<BlogItemViewModel> GetSidebarBlogs()
        {
            List<BlogItemViewModel> result = new List<BlogItemViewModel>();

            var blogs = db.Blogs.Where(c => c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate)
                .Select(c => new
                {
                    c.CreationDate,
                    c.Id,
                    c.ImageUrl,
                    c.Title,
                    c.UrlParam, 
                }).Take(4);

            foreach (var blog in blogs)
            {
                result.Add(new BlogItemViewModel()
                {
                    Id = blog.Id,
                    Title = blog.Title,
                    UrlParam = blog.UrlParam,
                    ImageUrl = blog.ImageUrl,
                    CreationDateStr = blog.CreationDate.ToShortDateString()
                });
            }

            return result;
        }

        public List<BlogItemViewModel> GetRelatedBlogs(Guid groupId)
        {
            List<BlogItemViewModel> result = new List<BlogItemViewModel>();

            var blogs = db.Blogs.Where(c => c.BlogGroupId == groupId && c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate)
                .Select(c => new
                {
                    c.CreationDate,
                    c.Id,
                    c.ImageUrl,
                    c.Title,
                    c.UrlParam,
                    c.Summery
                }).Take(2);

            foreach (var blog in blogs)
            {
                result.Add(new BlogItemViewModel()
                {
                    Id = blog.Id,
                    Title = blog.Title,
                    UrlParam = blog.UrlParam,
                    ImageUrl = blog.ImageUrl,
                    CreationDateStr = blog.CreationDate.ToShortDateString(),
                    Summery = blog.Summery
                });
            }

            return result;
        }
    }
}
