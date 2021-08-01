using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ViewModels;

namespace AustraliaShop.Controllers
{
    public class PosController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        // GET: Pos
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups.Where(current => current.IsDeleted == false), "Id", "Title");
            ViewBag.ProductId = new SelectList(db.Products.Where(current => current.IsDeleted == false), "Id", "Title");
            //List<DropDownListViewModel> paymentTypeDropDowns = new List<DropDownListViewModel>()
            //{
            //    new DropDownListViewModel() {Text = "پرداخت آنلاین",Value = "online" },
            //    new DropDownListViewModel() {Text = "پرداخت در محل",Value = "recieve" },
            //    new DropDownListViewModel() {Text = "کارت به کارت",Value = "transfer" },
            //    //........................ and so on
            //};

            //ViewBag.PaymentTypeId =
            //    new SelectList(paymentTypeDropDowns, "Value",
            //        "Text");

            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(current => current.IsDeleted == false), "Id", "Title");
            ViewBag.CustomerTypeId = new SelectList(db.CustomerTypes.Where(current => current.IsDeleted == false), "Id", "Title");
            return View();
        }

        public ActionResult GetProductByProductGroup(Guid id)
        {
            List<DropDownListViewModel> result = new List<DropDownListViewModel>();
            Guid productGroupId = id;
            var products = GetProductListByProductGroupId(id);
            foreach (Product product in products.ToList())
            {
                result.Add(new DropDownListViewModel()
                {
                    Text = product.Title,
                    Value = product.Id.ToString(),
                });
            }
            //foreach (City city in cities)
            //{
            //    cityItems.Add(new CityItemViewModel()
            //    {
            //        Text = city.Title,
            //        Value = city.Id.ToString()
            //    });
            //}
            return Json(result.OrderBy(c => c.Text), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadProductBySelectProduct(Guid id)
        {
            var products = GetProductListBySelectId(id);

            ProductInputViewModel productInput = GetProductList(products);
            //string productJson = JsonConvert.SerializeObject(productInput);
            return Json(productInput, JsonRequestBehavior.AllowGet);
        }

        public List<Product> GetProductListBySelectId(Guid productId)
        {
            List<Product> products = new List<Product>();

            products = db.Products
                .Where(c => c.Id == productId && c.IsDeleted == false).ToList();


            foreach (Product product in products)
            {
                if (!products.Any(c => c.Id == product.Id))
                    products.Add(product);
            }


            return products.OrderByDescending(c => c.Stock).ThenByDescending(c => c.Amount).ToList();
        }

        [AllowAnonymous]
        public ActionResult LoadProductByGroupId(Guid id)
        {

            var products = GetProductListByProductGroupId(id);

            ProductInputViewModel productInput = GetProductList(products);
            //string productJson = JsonConvert.SerializeObject(productInput);
            return Json(productInput, JsonRequestBehavior.AllowGet);
        }

        public ProductInputViewModel GetProductList(List<Product> products)
        {

            ProductInputViewModel productInput =
                new ProductInputViewModel()
                {
                    Products = GetProductViewModel(products),
                };

            return productInput;
        }

        public List<ProductItemsViewModel> GetProductViewModel(List<Product> products)
        {
            Guid? id = null;
            try
            {
                List<ProductItemsViewModel> productItems = new List<ProductItemsViewModel>();

                foreach (Product product in products)
                {
                    id = product.Id;
                    productItems.Add(new ProductItemsViewModel()
                    {
                        Id = product.Id.ToString(),
                        Title = product.Title,
                        Amount = product.AmountStr,
                        ImageUrl = WebConfigurationManager.AppSettings["baseUrl"] + product.ImageUrl,
                    });
                }

                return productItems;
            }
            catch (Exception e)
            {
                Guid? asd = id;
                Console.WriteLine(e);
                throw;
            }
        }

        public List<Product> GetProductListByProductGroupId(Guid productGroupId)
        {
            List<Product> products = new List<Product>();

            List<Product> productGroupRel = db.Products
                .Where(c => (c.ProductGroupId == productGroupId || c.ProductGroup.ParentId == productGroupId
                             || c.ProductGroup.Parent.ParentId == productGroupId) && c.IsDeleted == false).ToList();


            foreach (Product product in productGroupRel)
            {
                if (!products.Any(c => c.Id == product.Id))
                    products.Add(product);
            }


            return products.OrderByDescending(c => c.Stock).ThenByDescending(c => c.Amount).ToList();
        }

        [AllowAnonymous]
        public ActionResult GetBasketInfoByCookie()
        {
            string[] coockieProducts = GetCookie();
            BasketViewModel basket = CalculateBasket(coockieProducts);
            return Json(basket, JsonRequestBehavior.AllowGet);


        }

        [AllowAnonymous]
        public ActionResult GetBasketInfoByCookieWithDiscount(int discount, Guid productId)
        {
            string[] coockieProducts = GetCookie();
            BasketViewModel basket = CalculateBasket(coockieProducts);
            return Json(basket, JsonRequestBehavior.AllowGet);


        }

        public string[] GetCookie()
        {
            if (Request.Cookies["basket"] != null)
            {
                string cookievalue = Request.Cookies["basket"].Value;

                string[] basketItems = cookievalue.Split('/');

                return basketItems;
            }

            return null;
        }

        public BasketViewModel CalculateBasket(string[] coockieProducts)
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            decimal totalAmount = 0;

            foreach (string oProduct in coockieProducts)
            {
                string[] productItems = oProduct.Split('^');

                string productId = productItems[0];




                if (!string.IsNullOrEmpty(productId))
                {

                    Guid id = new Guid(productId);
                    string qty = productItems[1];
                    string discount = productItems[2];
                    string description = productItems[3];

                    Product product = db.Products.Where(c => c.Id == id).FirstOrDefault();

                    decimal amount = product.Amount;
                    decimal rowAmount;
                    decimal amountDiscount = (amount - ((amount * int.Parse(discount)) / 100));
                    if (productItems.Length == 1)
                    {

                        rowAmount = amountDiscount;
                    }
                    else
                    {
                        rowAmount = amountDiscount * Convert.ToInt32(productItems[1]);
                    }

                    totalAmount = Convert.ToDecimal(totalAmount + rowAmount);

                    if (product != null)
                    {

                        //string parentTitle;

                        //if (product.ParentId != null)
                        //    parentTitle = UnitOfWork.ProductRepository.GetById(product.ParentId.Value).Title;
                        //else
                        //    parentTitle = product.Title;

                        if (productItems.Length != 1)

                            qty = productItems[1];

                        if (discount == "")
                            discount = "0";
                        basketItems.Add(new BasketItemViewModel()
                        {
                            Id = productId,
                            Amount = (amount).ToString("n0"),
                            // ChildProducts = GetChildProducts(UnitOfWork.ProductColorRepository.Get(c => c.ProductId == product.Id).ToList(), selectedColor),
                            Title = product.Title,
                            Quantity = qty,
                            RowAmount = rowAmount.ToString("n0"),
                            Description = description,
                            Discount = discount
                        });
                    }
                }
            }

            BasketViewModel basket = GetBasket(basketItems, totalAmount);

            return basket;
        }

        public BasketViewModel CalculateBasket(string[] coockieProducts, int discount, Guid discountId)
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            decimal totalAmount = 0;

            foreach (string oProduct in coockieProducts)
            {
                string[] productItems = oProduct.Split('^');

                string productId = productItems[0];


                if (!string.IsNullOrEmpty(productId))
                {

                    Guid id = new Guid(productId);

                    Product product = db.Products.Where(c => c.Id == id).FirstOrDefault();

                    decimal amount = product.Amount;
                    decimal rowAmount;
                    if (productItems.Length == 1)
                    {
                        if (Guid.Parse(productId) == discountId)
                            rowAmount = (amount - ((amount * discount) / 100));
                        else
                            rowAmount = amount;
                    }

                    else
                    {
                        if (Guid.Parse(productId) == discountId)
                            amount = (amount - ((amount * discount) / 100));
                        rowAmount = amount * Convert.ToInt32(productItems[1]);
                    }

                    totalAmount = Convert.ToDecimal(totalAmount + rowAmount);

                    if (product != null)
                    {

                        //string parentTitle;

                        //if (product.ParentId != null)
                        //    parentTitle = UnitOfWork.ProductRepository.GetById(product.ParentId.Value).Title;
                        //else
                        //    parentTitle = product.Title;
                        string qty = "1";
                        if (productItems.Length != 1)

                            qty = productItems[1];


                        basketItems.Add(new BasketItemViewModel()
                        {
                            Id = productId,
                            Amount = (amount).ToString("n0"),
                            // ChildProducts = GetChildProducts(UnitOfWork.ProductColorRepository.Get(c => c.ProductId == product.Id).ToList(), selectedColor),
                            Title = product.Title,
                            Quantity = qty,
                            RowAmount = rowAmount.ToString("n0"),
                            Description = "",
                            Discount = discount.ToString()
                        });
                    }
                }
            }

            BasketViewModel basket = GetBasket(basketItems, totalAmount);

            return basket;
        }

        public BasketViewModel GetBasket(List<BasketItemViewModel> basketITems, decimal totalAmount)
        {
            BasketViewModel basket = new BasketViewModel()
            {
                Products = basketITems,
                Total = totalAmount.ToString("n0") + " تومان"
            };

            return basket;

        }

        [AllowAnonymous]
        public ActionResult RemoveFromBasket(string id)
        {
            string[] coockieProducts = GetCookie();

            foreach (string productId in coockieProducts)
            {
                if (!string.IsNullOrEmpty(productId))
                {
                    if (productId.Split('^')[0] == id)
                    {
                        coockieProducts = coockieProducts.Where(current => current != productId).ToArray();
                        break;
                    }
                }
            }

            SetCookie(coockieProducts);

            BasketViewModel basket = CalculateBasket(coockieProducts);

            return Json(basket, JsonRequestBehavior.AllowGet);
        }

        public void SetCookie(string[] basket)
        {
            string cookievalue = null;

            Deletecookie();

            foreach (string s in basket)
            {
                if (!string.IsNullOrEmpty(s))
                    cookievalue = cookievalue + s + "/";
            }

            HttpContext.Response.Cookies.Set(new HttpCookie("basket")
            {
                Name = "basket",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });
        }


        public void Deletecookie()
        {
            HttpCookie currentUserCookie = Request.Cookies["basket"];
            Response.Cookies.Remove("basket");
            if (currentUserCookie != null)
            {
                currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                currentUserCookie.Value = null;
                Response.SetCookie(currentUserCookie);
            }
        }

        public ActionResult GetUserFullName(string cellNumber)
        {
            User user = db.Users.Where(current => current.CellNum == cellNumber && current.Role.Name == "customer").FirstOrDefault();

            if (user == null)
                return Json("invalid", JsonRequestBehavior.AllowGet);

            Order order = db.Orders.Where(c => c.UserId == user.Id)
                .OrderByDescending(c => c.CreationDate).FirstOrDefault();

            string latestAddress = "";
            if (order != null)
                latestAddress = order.AddressLine1;
            return Json(user.FullName + "|" + latestAddress, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult PostFinalize(string orderDate, string cellNumber, string fullName, string address, string addedAmount,
            string decreasedAmount, string desc, string paymentAmount, Guid paymentTypeId, Guid customerTypeId, string file, string subtotalAmount, string totalAmount)
        {
            try
            {
                OrderInsertViewModel orderDetails = GetProductIdByCookie();

                User user = GetCurrentUser(cellNumber, fullName);

                //DateTime dtOrderDete = DateTimeHelper.PostPersianDate(orderDate);
                DateTime dtOrderDete = DateTime.Now;
                Order order = InsertOrder(user, dtOrderDete, address,
                    addedAmount, decreasedAmount, desc, paymentAmount, paymentTypeId, customerTypeId,
                    orderDetails.SubTotal, file);

                InsertToOrderDetail(orderDetails, order.Id);
                //InsertToAccount(order.SubAmount + order.AdditiveAmount - order.DiscountAmount, order.PaymentAmount,
                //    order.BranchId, order.UserId, order.Code);

                db.SaveChanges();

                return Json("true-" + order.Code + "*" + order.Id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        public OrderInsertViewModel GetProductIdByCookie()
        {
            string[] coockieProducts = GetCookie();

            List<PosInsertViewModel> productList = new List<PosInsertViewModel>();

            decimal subTotal = 0;

            for (int i = 0; i < coockieProducts.Length - 1; i++)
            {
                string[] productFeatures = coockieProducts[i].Split('^');

                Guid id = new Guid(productFeatures[0]);


                Product oProduct = db.Products.Where(c => c.Id == id).FirstOrDefault();



                int qty = Convert.ToInt32(productFeatures[1]);
                int discount = 0;
                if (productFeatures[2].Length < 5)
                    discount = Convert.ToInt32(productFeatures[2]);

                if (oProduct != null)
                {
                    decimal amount = oProduct.Amount;


                    decimal rowAmount = qty * amount;

                    subTotal = subTotal + rowAmount;


                    PosInsertViewModel input = new PosInsertViewModel()
                    {
                        ProductId = oProduct.Id,
                        Quantity = qty,
                        RowAmount = rowAmount,
                        Amount = amount,
                        DiscountPercent = discount,
                        TotalAmount = rowAmount - ((discount * rowAmount) / 100)
                    };

                    productList.Add(input);

                }
            }

            OrderInsertViewModel orderDetails = new OrderInsertViewModel()
            {
                OrderDetails = productList,
                SubTotal = subTotal
            };

            return orderDetails;
        }

        public User GetCurrentUser(string cellNumber, string fullName)
        {
            User user = db.Users.Where(current => current.CellNum == cellNumber && current.Role.Name == "customer").FirstOrDefault();

            if (user != null)
            {
                user.FullName = fullName;

                db.Entry(user).State = EntityState.Modified;

                return user;
            }

            user = CreateUser(cellNumber, fullName);

            return user;
        }

        public User CreateUser(string cellNumber, string fullName)
        {
            User user = db.Users.FirstOrDefault();
            user = new User()
            {
                CellNum = cellNumber,
                FullName = fullName,
                IsActive = true,
                Password = RandomCode().ToString(),
                RoleId = db.Roles.Where(current => current.Name == "customer").FirstOrDefault().Id,
                IsDeleted = false,
                CreationDate = DateTime.Now
            };
            db.Users.Add(user);
            db.SaveChanges();

            return user;
        }

        public int RandomCode()
        {
            Random generator = new Random();
            String r = generator.Next(0, 100000).ToString("D5");
            return Convert.ToInt32(r);
        }

        public Order InsertOrder(User user, DateTime orderDate, string address, string addedAmount,
            string decreasedAmount, string desc, string paymentAmount, Guid paymentTypeId, Guid customerTypeId, decimal subTotal, string fileUrl)
        {
            decimal additiveAmount = Convert.ToDecimal(addedAmount);
            decimal discountAmount = Convert.ToDecimal(decreasedAmount);
            decimal totalAmount = subTotal + additiveAmount - discountAmount;
            decimal paymentAmountDecimal = Convert.ToDecimal(paymentAmount);
            decimal remainAmountDecimal = totalAmount - paymentAmountDecimal;

            bool isPaid = totalAmount == paymentAmountDecimal;


            Guid orderStatusId = db.OrderStatuses.Where(current => current.Code == 1).FirstOrDefault().Id;


            Order order = new Order()
            {
                Code = ReturnCode(),
                UserId = user.Id,
                SubTotal = subTotal,
                AdditiveAmount = additiveAmount,
                DiscountAmount = 0,
                TotalAmount = totalAmount,
                PaymentAmount = paymentAmountDecimal,
                AddressLine1 = address,
                PaymentDate = orderDate,
                IsPaid = isPaid,
                IsActive = true,
                Description = desc,
                RemainAmount = remainAmountDecimal,
                OrderStatusId = orderStatusId,
                IsPos = true,
                DecreaseAmount = discountAmount,
                DeliverCellNumber = user.CellNum,
                DeliverFullName = user.FullName,
                CreationDate = DateTime.Now,
                IsDeleted = false,
                CustomerTypeId = customerTypeId
            };

            if (remainAmountDecimal == 0)
            {
                //   Guid paymentTypeIdGuid = new Guid(paymentTypeId);

                PaymentType paymentType = db.PaymentTypes.Find(paymentTypeId);

                if (paymentType != null)
                    order.PaymentTypeTitle = paymentType.Title;

                order.IsPaid = true;
            }

            db.Orders.Add(order);
            ViewBag.orderId = order.Id;
            db.SaveChanges();

            if (paymentAmountDecimal > 0)
                InsertToPayment(paymentAmountDecimal, order.Id, totalAmount, paymentTypeId);

            return order;
        }

        public int ReturnCode()
        {
            int orderCode = 1;
            Order order = db.Orders.OrderByDescending(current => current.Code).FirstOrDefault();

            if (order != null)
            {
                orderCode = order.Code + 1;
            }
            return orderCode;
        }
        public void InsertToPayment(decimal payAmount, Guid orderId, decimal totalAmount, Guid paymentTypeId)
        {
            bool isDeposit = payAmount < totalAmount;

            Payment payment = new Payment()
            {
                Id = Guid.NewGuid(),
                Amount = payAmount,
                IsDeposit = isDeposit,
                PaymentTypeId = paymentTypeId,
                OrderId = orderId,
                IsActive = true,
                PaymentDay = DateTime.Now,
                IsDeleted = false,
                CreationDate = DateTime.Now

            };

            db.Payments.Add(payment);
            db.SaveChanges();
        }
        public void InsertToOrderDetail(OrderInsertViewModel orderDetails, Guid orderId)
        {
            foreach (PosInsertViewModel detail in orderDetails.OrderDetails)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = orderId,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    Amount = detail.RowAmount,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.Now,
                    RowAmount = detail.RowAmount
                };

                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
            }
        }

    }
}