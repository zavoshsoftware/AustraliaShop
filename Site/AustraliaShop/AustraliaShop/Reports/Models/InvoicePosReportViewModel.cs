using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ViewModels;

namespace Reports.Models
{
    public class InvoicePosReportViewModel
    {
        public Order Order { get; set; }
        public List<InvoicePosOrderDetailViewModel> OrderDetails { get; set; }

        public string FactorDate { get; set; }

        public string DelliverCellNumber { get; set; }
        public string OrderAddress { get; set; }

        public string DelliverFullName { get; set; }

        public string OrderCode { get; set; }
        public string PaymentType { get; set; }

        public string SubTotal { get; set; }
        public string Total { get; set; }
        public string Discount { get; set; }
        public string Shipping { get; set; }
        public string PaymentDesc { get; set; }
        public string OrderStatus { get; set; }
        public string RefId { get; set; }
        public string IsPay { get; set; }
    }

    public class InvoicePosOrderDetailViewModel
    {
        public string Title { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string Amount { get; set; }
        public string DiscountPercent { get; set; }
        public string TotalAmount { get; set; }
        public string Description { get; set; }
    }
}