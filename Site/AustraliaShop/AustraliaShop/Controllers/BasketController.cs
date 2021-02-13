using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Models;

using ViewModels;

namespace AustraliaShop.Controllers
{
    public class BasketController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        BaseViewModelHelper baseViewModel = new BaseViewModelHelper();
        //   ZarinPalHelper zp = new ZarinPalHelper();

        [Route("cart")]
        [HttpPost]
        public ActionResult AddToCart(string code, string qty)
        {
            SetCookie(code, qty);
            return Json("true", JsonRequestBehavior.AllowGet);
        }



        [Route("Basket")]
        public ActionResult Basket()
        {
            CartViewModel cart = new CartViewModel();

            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

            cart.Products = productInCarts;

            decimal subTotal = GetSubtotal(productInCarts);

            cart.SubTotal = subTotal.ToString("n0");

            decimal discountAmount = GetDiscount();

            cart.DiscountAmount = discountAmount.ToString("n0");

            decimal total = subTotal - discountAmount;

            decimal shipmentAmount = GetShipmentAmountByTotal(total);

            cart.ShipmentAmount = shipmentAmount.ToString("N0");

            cart.Total = (total + shipmentAmount).ToString("n0");

            cart.Policy = db.TextItems.FirstOrDefault(c => c.Name == "policy");
            return View(cart);
        }



        public decimal GetShipmentAmountByTotal(decimal totalAmount)
        {
            decimal shipmentAmount = Convert.ToDecimal(WebConfigurationManager.AppSettings["shipmentAmount"]);
            decimal freeShipmentLimitAmount = Convert.ToDecimal(WebConfigurationManager.AppSettings["freeShipmentLimitAmount"]);

            if (totalAmount >= freeShipmentLimitAmount)
                return 0;

            return shipmentAmount;
        }

        [Route("Basket/remove/{code}")]
        public ActionResult RemoveFromBasket(string code)
        {
            string[] coockieItems = GetCookie();


            for (int i = 0; i < coockieItems.Length - 1; i++)
            {
                string[] coockieItem = coockieItems[i].Split('^');

                if (coockieItem[0] == code)
                {
                    string removeArray = coockieItem[0] + "^" + coockieItem[1];
                    coockieItems = coockieItems.Where(current => current != removeArray).ToArray();
                    break;
                }
            }

            string cookievalue = null;
            for (int i = 0; i < coockieItems.Length - 1; i++)
            {
                cookievalue = cookievalue + coockieItems[i] + "/";
            }

            HttpContext.Response.Cookies.Set(new HttpCookie("basket-babakshop")
            {
                Name = "basket-babakshop",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });

            return RedirectToAction("basket");
        }

        [AllowAnonymous]
        public ActionResult DiscountRequestPost(string coupon)
        {
            DiscountCode discount = db.DiscountCodes.FirstOrDefault(current => current.Code == coupon);

            string result = CheckCouponValidation(discount);

            if (result != "true")
                return Json(result, JsonRequestBehavior.AllowGet);

            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();
            decimal subTotal = GetSubtotal(productInCarts);

            decimal total = subTotal;

            DiscountHelper helper = new DiscountHelper();

            decimal discountAmount = helper.CalculateDiscountAmount(discount, total);

            SetDiscountCookie(discountAmount.ToString(), coupon);

            return Json("true", JsonRequestBehavior.AllowGet);
        }

        public decimal GetSubtotal(List<ProductInCart> orderDetails)
        {
            decimal subTotal = 0;

            foreach (ProductInCart orderDetail in orderDetails)
            {
                decimal amount = orderDetail.Product.Amount;

                if (orderDetail.Product.IsInPromotion)
                {
                    if (orderDetail.Product.DiscountAmount != null)
                        amount = orderDetail.Product.DiscountAmount.Value;
                }

                subTotal = subTotal + (amount * orderDetail.Quantity);
            }

            return subTotal;
        }
        public List<ProductInCart> GetProductInBasketByCoockie()
        {
            List<ProductInCart> productInCarts = new List<ProductInCart>();

            string[] basketItems = GetCookie();

            if (basketItems != null)
            {
                for (int i = 0; i < basketItems.Length - 1; i++)
                {
                    string[] productItem = basketItems[i].Split('^');

                    int productCode = Convert.ToInt32(productItem[0]);

                
                    Product product = db.Products.FirstOrDefault(current =>
                        current.IsDeleted == false && current.Code == productCode);
                     
                    Guid? productSizeId = null;
                    string sizeTitle = "-";
                    if (productItem[2] != "nosize")
                    {
                        productSizeId = new Guid(productItem[2]);

                        ProductSize productSize =
                            db.ProductSizes.FirstOrDefault(c => c.Id == productSizeId && c.Product.ParentId == product.Id);

                        if (productSize != null)
                        {
                            sizeTitle = productSize.Size.Title;
                            product = productSize.Product;
                        }
                    }


                    productInCarts.Add(new ProductInCart()
                    {
                        Product = product,
                        Quantity = Convert.ToInt32(productItem[1]),
                        ProductSizeId = productSizeId,
                        SizeTitle = sizeTitle

                    });
                }
            }

            return productInCarts;
        }
        public void SetCookie(string code, string quantity)
        {
            string cookievalue = null;

            if (Request.Cookies["basket-babakshop"] != null)
            {
                var test = Request.Cookies["basket-babakshop"].Value;
                bool changeCurrentItem = false;

                cookievalue = Request.Cookies["basket-babakshop"].Value;

                string[] coockieItems = cookievalue.Split('/');

                string size = "nosize";


                for (int i = 0; i < coockieItems.Length - 1; i++)
                {
                    if (Request.Cookies["sizecookie"] != null)
                    {
                        string[] sizeArray = Request.Cookies["sizecookie"].Value.Split('/');

                        if (sizeArray[0] == code)
                            size = sizeArray[1];
                    }

                    string[] coockieItem = coockieItems[i].Split('^');

                    if (coockieItem[0] == code && coockieItem[2] == size)
                    {
                        coockieItem[1] = (Convert.ToInt32(coockieItem[1]) + 1).ToString();
                        changeCurrentItem = true;
                        coockieItems[i] = coockieItem[0] + "^" + coockieItem[1] + "^" + coockieItem[2];
                        break;
                    }
                }

                if (changeCurrentItem)
                {
                    cookievalue = null;
                    for (int i = 0; i < coockieItems.Length - 1; i++)
                    {
                        cookievalue = cookievalue + coockieItems[i] + "/";
                    }

                }
                else
                    cookievalue = cookievalue + code + "^" + quantity + "^" + size + "/";

            }
            else
            {
                string size = "nosize";
                if (Request.Cookies["sizecookie"] != null)
                {
                    string[] sizeArray = Request.Cookies["sizecookie"].Value.Split('/');

                    if (sizeArray[0] == code)
                        size = sizeArray[1];
                }

                cookievalue = code + "^" + quantity + "^" + size + "/";
            }
            HttpContext.Response.Cookies.Set(new HttpCookie("basket-babakshop")
            {
                Name = "basket-babakshop",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });
        }



        public string[] GetCookie()
        {
            if (Request.Cookies["basket-babakshop"] != null)
            {
                string cookievalue = Request.Cookies["basket-babakshop"].Value;

                string[] basketItems = cookievalue.Split('/');

                return basketItems;
            }

            return null;
        }

        [AllowAnonymous]
        public string CheckCouponValidation(DiscountCode discount)
        {
            if (discount == null)
                return "Invald";

            if (!discount.IsMultiUsing)
            {
                if (db.Orders.Any(current => current.DiscountCodeId == discount.Id))
                    return "Used";
            }

            if (discount.ExpireDate < DateTime.Today)
                return "Expired";

            return "true";
        }


        public void SetDiscountCookie(string discountAmount, string discountCode)
        {
            HttpContext.Response.Cookies.Set(new HttpCookie("discount")
            {
                Name = "discount",
                Value = discountAmount + "/" + discountCode,
                Expires = DateTime.Now.AddDays(1)
            });
        }
        public decimal GetDiscount()
        {
            if (Request.Cookies["discount"] != null)
            {
                try
                {
                    string cookievalue = Request.Cookies["discount"].Value;

                    string[] basketItems = cookievalue.Split('/');
                    return Convert.ToDecimal(basketItems[0]);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            return 0;
        }


        [Route("checkout")]
        //[Authorize(Roles = "Customer")]
        public ActionResult CheckOut()
        {
            CheckOutViewModel checkOut = new CheckOutViewModel();

            if (User.Identity.IsAuthenticated)
            {

                var identity = (System.Security.Claims.ClaimsIdentity) User.Identity;
                string role = identity.FindFirst(System.Security.Claims.ClaimTypes.Role).Value;
                string id = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                Guid userId = new Guid(id);

                Models.User user = db.Users.Find(userId);

                if (user != null)
                {
                    UserInformation userInformation = new UserInformation()
                    {
                        FullName = user.FullName,
                        CellNumber = user.CellNum,
                        Email = user.Email,
                        IsAuthenticate = true
                    };

                    checkOut.UserInformation = userInformation;
                }

                if (role != "Customer")
                {
                    return Redirect("/login?ReturnUrl=checkout");
                }

            }
            else
            {
                UserInformation userInformation = new UserInformation()
                {
                    IsAuthenticate = false
                };

                checkOut.UserInformation = userInformation;
            }

            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

            checkOut.Products = productInCarts;

            decimal subTotal = GetSubtotal(productInCarts);

            checkOut.SubTotal = subTotal.ToString("n0");


            decimal discountAmount = GetDiscount();

            checkOut.DiscountAmount = discountAmount.ToString("n0");

            decimal total = subTotal - discountAmount;

            decimal shipmentAmount = GetShipmentAmountByTotal(total);

            checkOut.ShipmentAmount = shipmentAmount.ToString("N0");

            checkOut.Total = (total + shipmentAmount).ToString("n0");


            checkOut.Countries = db.Countries.OrderBy(current => current.Title).ToList();


            return View(checkOut);


        }

        public string GetTextByName(string name)
        {
            var textItem = db.TextItems.Where(c => c.Name == name).Select(c => c.Summery).FirstOrDefault();

            if (textItem != null)
                return textItem;
            return string.Empty;
        }


        public ActionResult Finalize(string notes, string cellnumber, string zipcode, string email, string address2,
                                    string address, string fullname, string country, string cityTown,
                                    string state, string createAccount, string password)
        {
            try
            {
                List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

                if (productInCarts.Count == 0)
                {
                    return Json("emptyBasket", JsonRequestBehavior.AllowGet);

                }
                if (createAccount=="true")
                {
                    if(!CheckEmailDuplicate(email))
                    return Json("duplicateEmail", JsonRequestBehavior.AllowGet);

                }
                Order order = ConvertCoockieToOrder(productInCarts);

                if (order != null)
                {
                    order.DeliverFullName = fullname;
                    order.DeliverCellNumber = cellnumber;
                    order.AddressLine1 = address;
                    order.AddressLine2 = address2;
                    order.PostalCode = zipcode;
                    order.CustomerDesc = notes;
                    order.CountryId = Convert.ToInt32(country);
                    order.DeliverEmail = email;
                    order.City = cityTown;
                    order.State = state;

                    order.TotalAmount = GetTotalAmount(order.SubTotal, order.DiscountAmount, order.ShippingAmount);
                }

                if (User.Identity.IsAuthenticated)
                {
                    var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;

                    string name = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;

                    Guid userId = new Guid(name);

                    if (order != null)
                        order.UserId = userId;

                }

                else
                {
                    if (createAccount == "true")
                    {
                        User user = new User()
                        {
                            Id = Guid.NewGuid(),
                            FullName =fullname,
                            CellNum =cellnumber,
                            Password = password,
                            Email = email,
                            CreationDate = DateTime.Now,
                            IsDeleted = false,
                            IsActive = true,
                            RoleId = new Guid("F2471901-E2A0-4D0B-B268-8DB58742D5AB")
                        };
                        db.Users.Add(user);

                        LoginNewUser(user.Email, user.Password, user.Id.ToString(), user.FullName);

                        if (order != null)
                            order.UserId = user.Id;

                    }

                
                }

                db.SaveChanges();

                string res = "paymentGateway";

                return Json(res, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);

            }
        }

        public bool CheckEmailDuplicate(string email)
        {
            if (db.Users.Any(c => c.Email == email && c.IsDeleted == false))
                return false;
            return true;
        }

        public ActionResult CheckoutLogin(string loginPassword, string loginEmail)
        {
            try
            {

                User user = db.Users.FirstOrDefault(c =>
                    c.Email == loginEmail && c.Password == loginPassword && c.IsDeleted == false && c.IsActive);

                if (user != null)
                {
                    LoginNewUser(user.Email, user.Password, user.Id.ToString(), user.FullName);
                    return Json("true", JsonRequestBehavior.AllowGet);

                }
                return Json("false", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }


        #region Finalize

        public void LoginNewUser(string email, string password,string id,string fullName)
        {
            var ident = new ClaimsIdentity(
                new[] { 
                    // adding following 2 claim just for supporting default antiforgery provider
                    new Claim(ClaimTypes.NameIdentifier, email),
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

                    new Claim(ClaimTypes.Name,id),

                    // optionally you could add roles if any
                    new Claim(ClaimTypes.Role, "Customer"),
                    new Claim(ClaimTypes.Surname,fullName)

                },
                DefaultAuthenticationTypes.ApplicationCookie);

            HttpContext.GetOwinContext().Authentication.SignIn(
                new AuthenticationProperties { IsPersistent = true }, ident);
        }


        public decimal GetTotalAmount(decimal? subtotal, decimal? discount, decimal? shippment)
        {
            decimal discountAmount = 0;
            if (discount != null)
                discountAmount = (decimal)discount;

            decimal shipmentAmount = 0;
            if (shippment != null)
                shipmentAmount = (decimal)shippment;

            if (subtotal == null)
                subtotal = 0;

            return (decimal)subtotal - discountAmount + shipmentAmount;
        }
        public Order ConvertCoockieToOrder(List<ProductInCart> products)
        {
            try
            {
                CodeGenerator codeCreator = new CodeGenerator();

                Order order = new Order();

                order.Id = Guid.NewGuid();
                order.IsActive = true;
                order.IsDeleted = false;
                order.IsPaid = false;
                order.CreationDate = DateTime.Now;
                order.LastModifiedDate = DateTime.Now;
                order.Code = codeCreator.ReturnOrderCode();
                order.OrderStatusId = db.OrderStatuses.FirstOrDefault(current => current.Code == 1).Id;
                order.SubTotal = GetSubtotal(products);

                order.DiscountAmount = GetDiscount();
                order.DiscountCodeId = GetDiscountId();
                order.ShippingAmount =
                    GetShipmentAmountByTotal(Convert.ToDecimal(order.SubTotal - order.DiscountAmount));

                order.TotalAmount = Convert.ToDecimal(order.SubTotal + order.ShippingAmount - order.DiscountAmount);


                db.Orders.Add(order);

                foreach (ProductInCart product in products)
                {
                    decimal amount = product.Product.Amount;

                    if (product.Product.IsInPromotion)
                    {
                        if (product.Product.DiscountAmount != null)
                            amount = product.Product.DiscountAmount.Value;
                    }

                    OrderDetail orderDetail = new OrderDetail()
                    {
                        ProductId = product.Product.Id,
                        Quantity = product.Quantity,
                        RowAmount = amount * product.Quantity,
                        IsDeleted = false,
                        IsActive = true,
                        CreationDate = DateTime.Now,
                        OrderId = order.Id,
                        Amount = amount,
                        ProductSizeId = product.ProductSizeId
                    };

                    db.OrderDetails.Add(orderDetail);
                }

                return order;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Guid? GetDiscountId()
        {
            if (Request.Cookies["discount"] != null)
            {
                try
                {
                    string cookievalue = Request.Cookies["discount"].Value;

                    string[] basketItems = cookievalue.Split('/');

                    DiscountCode discountCode =
                        db.DiscountCodes.FirstOrDefault(current => current.Code == basketItems[1]);

                    return discountCode?.Id;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }
        public User CreateUser(string fullName, string cellNumber)
        {
            CodeGenerator codeCreator = new CodeGenerator();

            Random rnd = new Random();

            User oUser =
                db.Users.FirstOrDefault(current => current.CellNum == cellNumber && current.IsDeleted == false);

            if (oUser == null)
            {
                User user = new User()
                {
                    CellNum = cellNumber,
                    FullName = fullName,
                    Password = rnd.Next(1000, 9999).ToString(),
                    //     Code = 1000,
                    IsDeleted = false,
                    IsActive = true,
                    CreationDate = DateTime.Now,
                    RemainCredit = 0,
                    RoleId = db.Roles.FirstOrDefault(current => current.Name.ToLower() == "customer").Id,

                };

                db.Users.Add(user);
                db.SaveChanges();
                return user;
            }
            return oUser;
        }


        public void RemoveCookie()
        {
            if (Request.Cookies["basket-babakshop"] != null)
            {
                Response.Cookies["basket-babakshop"].Expires = DateTime.Now.AddDays(-1);
            }
        }
        #endregion

        private String MerchantId = WebConfigurationManager.AppSettings["MerchantId"];



        //[Route("callback")]
        //public ActionResult CallBack(string authority, string status)
        //{
        //    String Status = status;
        //    CallBackViewModel callBack = new CallBackViewModel();
        //    ZarinPalHelper zarinPal = new ZarinPalHelper();
        //    if (Status != "OK")
        //    {
        //        callBack.IsSuccess = false;
        //        callBack.RefrenceId = "414";
        //        Order order = zarinPal.GetOrderByAuthority(authority);
        //        if (order != null)
        //        {
        //            callBack.Order = order;
        //            callBack.OrderDetails = db.OrderDetails
        //                        .Where(c => c.OrderId == order.Id && c.IsDeleted == false).Include(c => c.Product).ToList();
        //        }
        //    }

        //    else
        //    {
        //        try
        //        {
        //            var zarinpal = ZarinPal.ZarinPal.Get();
        //            zarinpal.DisableSandboxMode();
        //            String Authority = authority;
        //            long Amount = zarinPal.GetAmountByAuthority(Authority);

        //            var verificationRequest = new ZarinPal.PaymentVerification(MerchantId, Amount, Authority);
        //            var verificationResponse = zarinpal.InvokePaymentVerification(verificationRequest);
        //            if (verificationResponse.Status == 100 || verificationResponse.Status == 101)
        //            {
        //                Order order = zarinPal.GetOrderByAuthority(authority);
        //                if (order != null)
        //                {

        //                    UpdateOrder(order.Id, verificationResponse.RefID);

        //                    callBack.Order = order;
        //                    callBack.IsSuccess = true;
        //                    callBack.OrderCode = order.Code.ToString();
        //                    callBack.RefrenceId = verificationResponse.RefID;

        //                    callBack.OrderDetails = db.OrderDetails
        //                        .Where(c => c.OrderId == order.Id && c.IsDeleted == false).Include(c => c.Product).ToList();
        //                    foreach (OrderDetail orderDetail in callBack.OrderDetails)
        //                    {
        //                        Product product = orderDetail.Product;
        //                        product.Stock = orderDetail.Product.Stock - 1;

        //                        if (product.Stock <= 0)
        //                        {
        //                            product.IsAvailable = false;
        //                        }
        //                        db.SaveChanges();
        //                    }
        //                    RemoveCookie();

        //                   SendSms.SendCommonSms(order.User.CellNum,"کاربر گرامی با تشکر از خرید شما. سفارش شما در سایت رنگ و ابزار خوشدست با شماره پیگیری "+ verificationResponse.RefID + " با موفقیت ثبت گردید.");
        //                }
        //                else
        //                {
        //                    callBack.IsSuccess = false;
        //                    callBack.RefrenceId = "سفارش پیدا نشد";
        //                }
        //            }
        //            else
        //            {
        //                callBack.IsSuccess = false;
        //                callBack.RefrenceId = verificationResponse.Status.ToString();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            callBack.IsSuccess = false;
        //            callBack.RefrenceId = "خطا سیستمی. لطفا با پشتیبانی سایت تماس بگیرید";
        //        }
        //    }

        //    return View(callBack);
        //}


        //public void ChangeStockAfterPayment(Guid orderId)
        //{
        //    List<OrderDetail> orderDetails = db.OrderDetails.Where(current =>
        //        current.OrderId == orderId && current.IsDeleted == false && current.IsActive == true).ToList();

        //    foreach (OrderDetail orderDetail in orderDetails)
        //    {
        //        Product product = db.Products.FirstOrDefault(current => current.Id == orderDetail.ProductId);

        //        if (product != null)
        //        {
        //            product.Stock = product.Stock
        //                            - orderDetail.Quantity;
        //        }
        //    }
        //}

        //public void UpdateOrder(Guid orderId, string refId)
        //{
        //    Order order = db.Orders.Find(orderId);

        //    order.IsPaid = true;
        //    order.PaymentDate = DateTime.Now;
        //    order.SaleReferenceId = refId;


        //    //OrderStatus orderStatus = db.OrderStatuses.FirstOrDefault(current => current.Code == 2);
        //    //if (orderStatus != null)
        //    //    order.OrderStatusId = orderStatus.Id;

        //    order.LastModifiedDate = DateTime.Now;


        //    db.SaveChanges();
        //}
    }
}