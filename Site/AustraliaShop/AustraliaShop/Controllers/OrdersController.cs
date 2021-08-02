using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Helpers;
using Models;
using ViewModels;

namespace AustraliaShop.Controllers
{
    //[Authorize(Roles = "Administrator")]
    public class OrdersController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Orders
        public ActionResult Index(Guid? id)
        {
            List<Order> orders = new List<Order>();

            if (id != null)
            {
                if (id == new Guid("11869F4D-D6D1-434C-A2F1-7945227CD3BB"))
                {
                    Guid status2 = new Guid("EC934A7E-0061-4B09-BD44-CA5120CF6200");
                    Guid status3 = new Guid("7DBF85F4-7835-4D21-8269-26695D0C7E0F");
                    orders = db.Orders.Include(o => o.Country).Where(o =>
                            (o.OrderStatusId == id || o.OrderStatusId == status3 || o.OrderStatusId == status2) &&
                            o.IsDeleted == false)
                        .OrderByDescending(o => o.CreationDate).Include(o => o.DiscountCode).Include(o => o.OrderStatus)
                        .Include(o => o.User).ToList();
                }
                else
                {
                    orders = db.Orders.Include(o => o.Country).Where(o =>
                           o.OrderStatusId == id && o.IsDeleted == false)
                        .OrderByDescending(o => o.CreationDate).Include(o => o.DiscountCode).Include(o => o.OrderStatus)
                        .Include(o => o.User).ToList();
                }
            }
            else
            {
                if (TempData["pos"] == "pos")
                {
                    orders = db.Orders.Include(o => o.Country).Where(o => o.IsDeleted == false && o.IsPos)
                   .OrderByDescending(o => o.CreationDate).Include(o => o.DiscountCode).Include(o => o.OrderStatus)
                   .Include(o => o.User).ToList();
                }
                else
                {
                    orders = db.Orders.Include(o => o.Country).Where(o => o.IsDeleted == false)
                    .OrderByDescending(o => o.CreationDate).Include(o => o.DiscountCode).Include(o => o.OrderStatus)
                    .Include(o => o.User).ToList();
                }

            }
            return View(orders);
        }

        public ActionResult Pos()
        {
            List<Order> orders = new List<Order>();


            TempData["pos"] = "pos";

            return RedirectToAction("Index");
        }

        // GET: OrderDetails/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            List<OrderDetail> orderDetails = db.OrderDetails.Include(current => current.Product).Include(c=>c.ProductSize).Where(current => current.OrderId == order.Id).ToList();

            OrderDetailViewModel orderDetailViewModel = new OrderDetailViewModel()
            {
                Order = order,
                OrderDetails = orderDetails
            };
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses.OrderBy(c=>c.Code), "Id", "Title", order.OrderStatusId);

            return View(orderDetailViewModel);
        }

      
        
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Order order = db.Orders.Find(id);
            order.IsDeleted = true;
            order.DeletionDate = DateTime.Now;

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



        [AllowAnonymous]
        public ActionResult ChangeOrderStatus(string code, string statusId)
        {

            Guid id = new Guid(statusId);
            int orderCode = Convert.ToInt32(code);
            Order order = db.Orders.FirstOrDefault(c => c.Code == orderCode);

            if (order != null)
            {
                //اگر وضعیت از ثبت اولیه به درحال اماده سازی یا در حال ارسال یا ارسال شده تغییر کرد از موجودی کسر بشه
                if (order.PaymentTypeTitle != "online")
                    ChangeStock(order, id);

                order.OrderStatusId = id;
                order.LastModifiedDate = DateTime.Now;

                db.SaveChanges();

            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        public void ChangeStock(Order order, Guid newstatusId)
        {
            if (order.OrderStatusId == new Guid("11869F4D-D6D1-434C-A2F1-7945227CD3BB") &&
                (newstatusId == new Guid("7DBF85F4-7835-4D21-8269-26695D0C7E0F") || newstatusId == new Guid("68490C34-A666-44DA-8D9F-7B6DEE2E4DE0") ||
                 newstatusId == new Guid("EC934A7E-0061-4B09-BD44-CA5120CF6200")))
            {
                List<OrderDetail> orderDetails = db.OrderDetails.Where(c => c.OrderId == order.Id).ToList();

                foreach (OrderDetail orderDetail in orderDetails)
                {
                    orderDetail.Product.Stock = orderDetail.Product.Stock - orderDetail.Quantity;
                    if (orderDetail.Product.Stock <= 0)
                        orderDetail.Product.IsAvailable = false;
                }
            }
        }

        [AllowAnonymous]
        public ActionResult ChangeOrderPaymentStatus(string code, string statusId, string paymentDesc)
        {

            bool isPay = Convert.ToBoolean(statusId);
            int orderCode = Convert.ToInt32(code);
            Order order = db.Orders.FirstOrDefault(c => c.Code == orderCode);

            if (order != null)
            {
                order.IsPaid = isPay;
                order.PaymentDesc = paymentDesc;
                order.LastModifiedDate = DateTime.Now;
                db.SaveChanges();

            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult SendSmsToUser(string code, string userSms)
        {


            int orderCode = Convert.ToInt32(code);
            Order order = db.Orders.FirstOrDefault(c => c.Code == orderCode);

            if (order != null)
            {
                //SendSms.SendCommonSms(order.User.CellNum, userSms);

            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult SubmitOrderDesc(string code, string desc)
        {


            int orderCode = Convert.ToInt32(code);
            Order order = db.Orders.FirstOrDefault(c => c.Code == orderCode);

            if (order != null)
            {
                order.Description = desc;
                order.LastModifiedDate = DateTime.Now;
                db.SaveChanges();

            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        //public string SerOrderStatus()
        //{
        //    Guid id = new Guid("7dbf85f4-7835-4d21-8269-26695d0c7e0f");
        //    List<Order> orders = db.Orders.Where(c => c.OrderStatusId == id).ToList();

        //    foreach (Order order in orders)
        //    {
        //        order.OrderStatusId = new Guid("11869f4d-d6d1-434c-a2f1-7945227cd3bb");

        //    }

        //    db.SaveChanges();
        //    return string.Empty;
        //}

        public void UpdateOrderCodes()
        {
            var orders = db.Orders.Where(c => c.IsDeleted == false).OrderBy(c => c.CreationDate).ToList();

            int i = 0;
            foreach (var order in orders)
            {
                order.Code = 100000 + i;
                i++;
            }
            db.SaveChanges();
        }
    }
}
