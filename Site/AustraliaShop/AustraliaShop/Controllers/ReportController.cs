using Models;
using Reports.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AustraliaShop.Controllers
{
    public class ReportController : Controller
    {
        public ActionResult Invoice(Guid id)
        {

            TempData["id"] = id;
            return View();
        }

        public ActionResult InvoiceRedirect(int id)
        {

            var order = db.Orders.FirstOrDefault(c => c.Code == id);
            return RedirectToAction("Invoice", new { id = order.Id });
        }

        public ActionResult InvoicePos(Guid id)
        {

            TempData["id"] = id;
            return View();
        }


        public ActionResult LoadInvoiceReportSnapshot()
        {
            Guid orderId = new Guid(TempData["id"].ToString());


            Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHn0s4gy0Fr5YoUZ9V00Y0igCSFQzwEqYBh/N77k4f0fWXTHW5rqeBNLkaurJDenJ9o97TyqHs9HfvINK18Uwzsc/bG01Rq+x3H3Rf+g7AY92gvWmp7VA2Uxa30Q97f61siWz2dE5kdBVcCnSFzC6awE74JzDcJMj8OuxplqB1CYcpoPcOjKy1PiATlC3UsBaLEXsok1xxtRMQ283r282tkh8XQitsxtTczAJBxijuJNfziYhci2jResWXK51ygOOEbVAxmpflujkJ8oEVHkOA/CjX6bGx05pNZ6oSIu9H8deF94MyqIwcdeirCe60GbIQByQtLimfxbIZnO35X3fs/94av0ODfELqrQEpLrpU6FNeHttvlMc5UVrT4K+8lPbqR8Hq0PFWmFrbVIYSi7tAVFMMe2D1C59NWyLu3AkrD3No7YhLVh7LV0Tttr/8FrcZ8xirBPcMZCIGrRIesrHxOsZH2V8t/t0GXCnLLAWX+TNvdNXkB8cF2y9ZXf1enI064yE5dwMs2fQ0yOUG/xornE";

            //Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkcgIvwL0jnpsDqRpWg5FI5kt2G7A0tYIcUygBh1sPs7koivWV0htru4Pn2682yhdY3+9jxMCVTKcKAjiEjgJzqXgLFCpe62hxJ7/VJZ9Hq5l39md0pyydqd5Dc1fSWhCtYqC042BVmGNkukYJQN0ufCozjA/qsNxzNMyEql26oHE6wWE77pHutroj+tKfOO1skJ52cbZklqPm8OiH/9mfU4rrkLffOhDQFnIxxhzhr2BL5pDFFCZ7axXX12y/4qzn5QLPBn1AVLo3NVrSmJB2KiwGwR4RL4RsYVxGScsYoCZbwqK2YrdbPHP0t5vOiLjBQ+Oy6F4rNtDYHn7SNMpthfkYiRoOibqDkPaX+RyCany0Z+uz8bzAg0oprJEn6qpkQ56WMEppdMJ9/CBnEbTFwn1s/9s8kYsmXCvtI4iQcz+RkUWspLcBzlmj0lJXWjTKMRZz+e9PmY11Au16wOnBU3NHvRc9T/Zk0YFh439GKd/fRwQrk8nJevYU65ENdAOqiP5po7Vnhif5FCiHRpxgF";

            var path = System.Web.HttpContext.Current.Server.MapPath("~/Content/license.key");

            Stimulsoft.Base.StiLicense.LoadFromFile(path);

            var report = new StiReport();
            report.Load(Server.MapPath("~/Reports/MRT/Invoice.mrt"));
            report.RegBusinessObject("Invoice", GetInvoice(orderId));
            //  report.Dictionary.Variables.Add("today", DateTime.Today());
            return StiMvcViewer.GetReportResult(report);
        }

        //public ActionResult LoadInvoicePosReportSnapshot()
        //{
        //    Guid orderId = new Guid(TempData["id"].ToString());


        //    Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHn0s4gy0Fr5YoUZ9V00Y0igCSFQzwEqYBh/N77k4f0fWXTHW5rqeBNLkaurJDenJ9o97TyqHs9HfvINK18Uwzsc/bG01Rq+x3H3Rf+g7AY92gvWmp7VA2Uxa30Q97f61siWz2dE5kdBVcCnSFzC6awE74JzDcJMj8OuxplqB1CYcpoPcOjKy1PiATlC3UsBaLEXsok1xxtRMQ283r282tkh8XQitsxtTczAJBxijuJNfziYhci2jResWXK51ygOOEbVAxmpflujkJ8oEVHkOA/CjX6bGx05pNZ6oSIu9H8deF94MyqIwcdeirCe60GbIQByQtLimfxbIZnO35X3fs/94av0ODfELqrQEpLrpU6FNeHttvlMc5UVrT4K+8lPbqR8Hq0PFWmFrbVIYSi7tAVFMMe2D1C59NWyLu3AkrD3No7YhLVh7LV0Tttr/8FrcZ8xirBPcMZCIGrRIesrHxOsZH2V8t/t0GXCnLLAWX+TNvdNXkB8cF2y9ZXf1enI064yE5dwMs2fQ0yOUG/xornE";

        //    //Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkcgIvwL0jnpsDqRpWg5FI5kt2G7A0tYIcUygBh1sPs7koivWV0htru4Pn2682yhdY3+9jxMCVTKcKAjiEjgJzqXgLFCpe62hxJ7/VJZ9Hq5l39md0pyydqd5Dc1fSWhCtYqC042BVmGNkukYJQN0ufCozjA/qsNxzNMyEql26oHE6wWE77pHutroj+tKfOO1skJ52cbZklqPm8OiH/9mfU4rrkLffOhDQFnIxxhzhr2BL5pDFFCZ7axXX12y/4qzn5QLPBn1AVLo3NVrSmJB2KiwGwR4RL4RsYVxGScsYoCZbwqK2YrdbPHP0t5vOiLjBQ+Oy6F4rNtDYHn7SNMpthfkYiRoOibqDkPaX+RyCany0Z+uz8bzAg0oprJEn6qpkQ56WMEppdMJ9/CBnEbTFwn1s/9s8kYsmXCvtI4iQcz+RkUWspLcBzlmj0lJXWjTKMRZz+e9PmY11Au16wOnBU3NHvRc9T/Zk0YFh439GKd/fRwQrk8nJevYU65ENdAOqiP5po7Vnhif5FCiHRpxgF";

        //    var path = System.Web.HttpContext.Current.Server.MapPath("~/Content/license.key");

        //    Stimulsoft.Base.StiLicense.LoadFromFile(path);

        //    var report = new StiReport();
        //    report.Load(Server.MapPath("~/Reports/MRT/InvoicePos.mrt"));
        //    report.RegBusinessObject("KhoshdastInvoicePos", GetInvoicePos(orderId));
        //    //  report.Dictionary.Variables.Add("today", DateTime.Today());
        //    return StiMvcViewer.GetReportResult(report);
        //}

        public virtual ActionResult ViewerEvent()
        {
            return StiMvcViewer.ViewerEventResult();
        }

        public virtual ActionResult PrintReport()
        {
            return StiMvcViewer.PrintReportResult();
        }

        public virtual ActionResult ExportReport()
        {
            return StiMvcViewer.ExportReportResult();
        }

        private DatabaseContext db = new DatabaseContext();

        public InvoiceReportViewModel GetInvoice(Guid id)
        {
            Order order = db.Orders.Find(id);
            //Exit exit = UnitOfWork.ExitRepository.GetById(exitId);


            //if (exit != null)
            //{
            //    string paymentAmount = "0";
            //    if (exit.PaymentAmount != null)
            //        paymentAmount = exit.PaymentAmount.Value.ToString("N0");


            //    string remainAmount = "0";
            //    if (exit.RemainAmount != null)
            //        remainAmount = exit.RemainAmount.Value.ToString("N0");

            string shippingAmount = "0";
            if (order.ShippingAmount != null)
                shippingAmount = order.ShippingAmount.Value.ToString("N0");

            string pay = "پرداخت شده";
            if (!order.IsPaid)
                pay = "پرداخت نشده";

         

            InvoiceReportViewModel invoice = new InvoiceReportViewModel()
            {
                CustomerName = order.User.FullName,
                CustomerCellNumber = order.User.CellNum,
                OrderCode = order.Code.ToString(),
                Date = order.OrderDate.ToString(),
                Products = GetProductList(id),
                OrderStatus = order.OrderStatus.Title,
                SubTotal = order.SubTotalStr,
                Total = order.TotalAmountStr,
                Shipping = shippingAmount,
                CustomerAddress = order.AddressLine1,
                PostalCode = order.PostalCode,
                Discount = order.DiscountAmountStr,
                IsPay = pay,
                PaymentType = order.PaymentTypeTitle,
                RefId = order.SaleReferenceId,
                PaymentDesc = order.PaymentDesc
            };

            return invoice;
            //}

            //return new InvoiceReportViewModel();
        }

        public List<InvoiceProductViewModel> GetProductList(Guid id)
        {
            List<OrderDetail> orderDetails = db.OrderDetails.Where(c => c.OrderId == id).ToList();

            List<InvoiceProductViewModel> result = new List<InvoiceProductViewModel>();

            foreach (OrderDetail orderDetail in orderDetails)
            {
                string discount = "-";
                result.Add(new InvoiceProductViewModel()
                {
                    Amount = orderDetail.Amount.ToString(),
                    Quantity = orderDetail.Quantity.ToString(),
                    RowAmount = orderDetail.RowAmount.ToString(),
                    ProductTitle = orderDetail.Product.Title,
                    Discount = discount,
                    Description = orderDetail.Description
                });
            }

            return result;
        }

        public InvoicePosReportViewModel GetInvoicePos(Guid id)
        {
            Order order = db.Orders.Find(id);

            string shippingAmount = "0";
            if (order.ShippingAmount != null)
                shippingAmount = order.ShippingAmount.Value.ToString("N0");

            string pay = "پرداخت شده";
            if (!order.IsPaid)
                pay = "پرداخت نشده";

            InvoicePosReportViewModel invoice = new InvoicePosReportViewModel()
            {
                DelliverFullName = order.User.FullName,
                DelliverCellNumber = order.User.CellNum,
                OrderCode = order.Code.ToString(),
                FactorDate = order.OrderDate.ToString(),
                OrderDetails = GetOrderList(id),
                OrderStatus = order.OrderStatus.Title,
                SubTotal = order.SubTotal.ToString(),
                Total = order.TotalAmount.ToString(),
                Shipping = shippingAmount,
                OrderAddress = order.AddressLine1,
                PaymentType = order.PaymentTypeTitle,
                PaymentDesc = order.PaymentDesc,
                IsPay = pay,
                RefId = order.SaleReferenceId
            };

            return invoice;
            //}

            //return new InvoiceReportViewModel();
        }

        public List<InvoicePosOrderDetailViewModel> GetOrderList(Guid id)
        {
            List<OrderDetail> orderDetails = db.OrderDetails.Where(c => c.OrderId == id).ToList();

            List<InvoicePosOrderDetailViewModel> result = new List<InvoicePosOrderDetailViewModel>();

            foreach (OrderDetail orderDetail in orderDetails)
            {
                result.Add(new InvoicePosOrderDetailViewModel()
                {
                    Title = orderDetail.Product.Title,
                    Quantity = orderDetail.Quantity.ToString(),
                    Amount = orderDetail.Amount.ToString("N0"),
                    TotalAmount = orderDetail.RowAmount.ToString("N0"),
                    Description = orderDetail.Description
                });
            }

            return result;
        }
    }
}