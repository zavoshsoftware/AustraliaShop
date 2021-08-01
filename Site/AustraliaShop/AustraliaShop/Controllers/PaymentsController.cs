
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eWAY.Rapid;
using eWAY.Rapid.Enums;
using eWAY.Rapid.Models;
using Models;
using ViewModels;

namespace AustraliaShop.Controllers
{
    public class PaymentsController : Controller
    {
        //sandbox or production
        private DatabaseContext db = new DatabaseContext();

        [Route("ewaypayment/{id:Guid}")]
        public ActionResult EwayPayment(Guid id)
        {
            Order order = db.Orders.Find(id);
            IRapidClient ewayClient = RapidClientFactory.NewRapidClient("A1001CVrAI37H9WS3YIGWBcE8ghI1TE51WRfM8oiZdnCQj/j6RTEuWrnL/ZowuX3t1eszH", "UXtJqeBN", "Sandbox");

            Transaction transaction = new Transaction()
            {
                PaymentDetails = new PaymentDetails()
                {
                    TotalAmount = Convert.ToInt32(order.TotalAmount * 100),
                    InvoiceNumber = id.ToString(),
                },
                RedirectURL = "http://babak.rassamenergy.com/payments/callback",
                TransactionType = TransactionTypes.Purchase
            };

            CreateTransactionResponse response = ewayClient.Create(PaymentMethod.TransparentRedirect, transaction);


            if (response.Errors == null)
            {
                string accessCode = response.AccessCode;
                string formUrl = response.FormActionUrl;

                ViewBag.accessCode = accessCode;
                ViewBag.formUrl = formUrl;
                //  return RedirectToAction("Pay", new { formUrl = formUrl, accessCode = accessCode });
                //  Redirect(formUrl);
            }
            else
            {
                foreach (string errorCode in response.Errors)
                {
                    //    result += RapidClientFactory.UserDisplayMessage(errorCode, "EN");
                    // Console.WriteLine("Response Messages: " + );
                }
            }
            PaymentViewModel result = new PaymentViewModel()
            {
                Amount = order.TotalAmount
            };
            return View(result);
        }
        public ActionResult Index()
        {
            string result = "";

            //IRapidClient ewayClient = RapidClientFactory.NewRapidClient("60CF3Chj08y4T1e/fspEtkEIqvuoSVmNDpWty/VeX6Qk7O++QuzX3kXxcYidmoJMbFkIRr", "Qpv6p8lI", "Sandbox");

            //Transaction transaction = new Transaction()
            //{
            //    Customer = new Customer()
            //    {
            //        CardDetails = new CardDetails()
            //        {
            //            Name = "John Smith",
            //            Number = "4444333322221111",
            //            ExpiryMonth = "11",
            //            ExpiryYear = "25",
            //            CVN = "123"
            //        }
            //    },
            //    PaymentDetails = new PaymentDetails()
            //    {
            //        TotalAmount = 1000
            //    },
            //    TransactionType = TransactionTypes.Purchase
            //};

            //CreateTransactionResponse response = ewayClient.Create(PaymentMethod.Direct, transaction);

            //if (response.Errors != null)
            //{
            //    foreach (string errorCode in response.Errors)
            //    {
            //        result += RapidClientFactory.UserDisplayMessage(errorCode, "EN");
            //    }
            //}
            //else
            //{
            //    if ((bool)response.TransactionStatus.Status)
            //    {
            //        result += response.TransactionStatus.TransactionID;;
            //    }
            //}

            IRapidClient ewayClient = RapidClientFactory.NewRapidClient("A1001CVrAI37H9WS3YIGWBcE8ghI1TE51WRfM8oiZdnCQj/j6RTEuWrnL/ZowuX3t1eszH", "UXtJqeBN", "Sandbox");

            Transaction transaction = new Transaction()
            {
                PaymentDetails = new PaymentDetails()
                {
                    TotalAmount = 5000,
                    InvoiceNumber = "1",

                },
                RedirectURL = "http://babak.rassamenergy.com/payments/callback",
                TransactionType = TransactionTypes.Purchase
            };

            CreateTransactionResponse response = ewayClient.Create(PaymentMethod.TransparentRedirect, transaction);


            if (response.Errors == null)
            {
                string accessCode = response.AccessCode;
                string formUrl = response.FormActionUrl;
                return RedirectToAction("Pay", new { formUrl = formUrl, accessCode = accessCode });
                //  Redirect(formUrl);
            }
            else
            {
                foreach (string errorCode in response.Errors)
                {
                    result += RapidClientFactory.UserDisplayMessage(errorCode, "EN");
                    // Console.WriteLine("Response Messages: " + );
                }
            }

            return View(result);
        }

        public ActionResult Pay(string formUrl, string accessCode)
        {
            ViewBag.accessCode = accessCode;
            ViewBag.formUrl = formUrl;
            return View();
        }
        public ActionResult Callback(string accessCode)
        {
            CallBackViewModel callback = new CallBackViewModel();
            IRapidClient ewayClient = RapidClientFactory.NewRapidClient("A1001CVrAI37H9WS3YIGWBcE8ghI1TE51WRfM8oiZdnCQj/j6RTEuWrnL/ZowuX3t1eszH", "UXtJqeBN", "Sandbox");

            QueryTransactionResponse response = ewayClient.QueryTransaction(accessCode);
            string result = "";
            if ((bool)response.TransactionStatus.Status)
            {
                Guid orderId = new Guid(response.Transaction.PaymentDetails.InvoiceNumber);

                Order order = db.Orders.Find(orderId);

                if (order == null)
                {
                    callback.IsSuccess = false;
                    callback.Message = "Invalid Order Id";
                    return View(callback);
                }

                order.IsPaid = true;
                order.SaleReferenceId = response.TransactionStatus.TransactionID.ToString();
                order.LastModifiedDate = DateTime.Now;
                order.PaymentDate = DateTime.Now;

                ChangProductsStock(order);

                db.SaveChanges();

                result = "Payment successful!";
                callback.RefrenceId = response.TransactionStatus.TransactionID.ToString();
                callback.IsSuccess = true;
            }
            else
            {
              
                List<string> errorCodes = response.TransactionStatus.ProcessingDetails.getResponseMessages();
                result = "Response Message: ";

                foreach (string errorCode in errorCodes)
                {
                    callback.IsSuccess = false;

                    result += RapidClientFactory.UserDisplayMessage(errorCode, "EN");
                    // Console.WriteLine("Response Message: " + RapidClientFactory.UserDisplayMessage(errorCode, "EN"));
                }
            }

            callback.Message = result;

            return View(callback);

        }

        public void ChangProductsStock(Order order)
        {
            var orderDetails = db.OrderDetails.Where(c => c.OrderId == order.Id).Select(c => new
            {
                c.Id,
                c.ProductId,
                c.ProductSizeId,
                c.Quantity
            });

            foreach (var orderDetail in orderDetails)
            {
                if (orderDetail.ProductSizeId != null)
                {
                    ProductSize productSize = db.ProductSizes.Find(orderDetail.ProductSizeId);

                    productSize.Stock = productSize.Stock - orderDetail.Quantity;
                    productSize.LastModifiedDate = DateTime.Now;
                }
                else
                {
                    Product product = db.Products.Find(orderDetail.ProductId);

                    product.Stock = product.Stock - orderDetail.Quantity;
                    product.LastModifiedDate = DateTime.Now;

                    if (product.Stock <= 0)
                        product.IsAvailable = false;
                }
            }
        }
    }
}