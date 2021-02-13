﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class CheckOutViewModel : _BaseViewModel
    {
        public bool IsUseDiscount { get; set; }
        public List<ProductInCart> Products { get; set; }
        public string Total { get; set; }
        public string SubTotal { get; set; }
        public string DiscountAmount { get; set; }
        public List<Country> Countries { get; set; }
        public UserInformation UserInformation { get; set; }
        public string ShipmentAmount { get; set; }
        public string OnlinePay { get; set; }
        public string RecievePay { get; set; }
        public string TransferPay { get; set; }
    }

    public class UserInformation
    {
        public bool IsAuthenticate { get; set; }
        public string FullName { get; set; }
        public Guid CountryId { get; set; }
        public string CellNumber { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
    }
}