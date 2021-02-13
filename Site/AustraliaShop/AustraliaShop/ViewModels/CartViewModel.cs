using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class CartViewModel : _BaseViewModel
    {
        public List<ProductInCart> Products { get; set; }
        public string SubTotal { get; set; }
        public string Total { get; set; }
        public string DiscountAmount { get; set; }
        public TextItem Policy { get; set; }
        public string ShipmentAmount { get; set; }

    }

    public class ProductInCart
    {
        public Product Product { get; set; }
        public Guid? ProductSizeId { get; set; }
        public string SizeTitle { get; set; }
        public int Quantity { get; set; }
        public string RowAmount
        {
            get
            {
                if (Product.IsInPromotion && Product.DiscountAmount != null)
                    return (Product.DiscountAmount.Value * Quantity).ToString("n0") +"$";

                return (Product.Amount * Quantity).ToString("n0") + "$";
            }
        }

        public string Amount
        {
            get
            {
                if (Product.IsInPromotion && Product.DiscountAmount != null)
                    return (Product.DiscountAmount.Value).ToString("n0") + "$";

                return (Product.Amount).ToString("n0") + "$";
            }
        }


    }
}