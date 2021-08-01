using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class PosInsertViewModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal RowAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? TotalAmount { get; set; }
        public string Description { get; set; }
    }
}