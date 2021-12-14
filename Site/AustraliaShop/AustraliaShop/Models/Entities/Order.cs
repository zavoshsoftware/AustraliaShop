using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Web.Mvc.Html;

namespace Models
{

    public class Order : BaseEntity
    {
        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }

        [Required]
        public int Code { get; set; }

        public Guid? UserId { get; set; }

        public int? CountryId { get; set; }

        public virtual Country  Country { get; set; }
        public string City { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        [Column(TypeName = "Money")]
        public decimal TotalAmount { get; set; }

        [NotMapped]
        [Display(Name = "TotalAmount")]
        public string TotalAmountStr
        {
            get { return TotalAmount.ToString("n0"); } 
        }


        [Display(Name = "Order Status")]
        [Required]
        public Guid OrderStatusId { get; set; }

        //public Guid? CityId { get; set; }

        [Display(Name = "Bank Reference Code")]
        public string SaleReferenceId { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [Display(Name = "success payment")]
        public bool IsPaid { get; set; }


        public Guid? DiscountCodeId { get; set; }

        public virtual DiscountCode DiscountCode { get; set; }
        [Display(Name = "Shipping Amount")]
        public decimal? ShippingAmount { get; set; }

        public decimal? SubTotal { get; set; }

        [NotMapped]
        [Display(Name = "SubTotal")]
        public string SubTotalStr
        {
            get
            {
                if (SubTotal != null)
                    return SubTotal.Value.ToString("n0") ;

                return string.Empty;
            }
        }

        public decimal? DiscountAmount { get; set; }


        [NotMapped]
        [Display(Name = "DiscountAmount")]
        public string DiscountAmountStr
        {
            get
            {
                if (DiscountAmount != null)
                    return DiscountAmount.Value.ToString("n0");

                return string.Empty;
            }
        }

        internal class Configuration : EntityTypeConfiguration<Order>
        {
            public Configuration()
            {
                HasOptional(p => p.User)
                    .WithMany(j => j.Orders)
                    .HasForeignKey(p => p.UserId);

                HasRequired(p => p.OrderStatus)
                    .WithMany(j => j.Orders)
                    .HasForeignKey(p => p.OrderStatusId);
                 

                HasRequired(p => p.DiscountCode)
                    .WithMany(j => j.Orders)
                    .HasForeignKey(p => p.DiscountCodeId);
            }
        }

        public string DeliverFullName { get; set; }
        public string DeliverCellNumber { get; set; }
        public string DeliverEmail { get; set; }
        public string PostalCode { get; set; }
        public DateTime? PaymentDate { get; set; }

        public string PaymentTypeTitle { get; set; }
        [Display(Name = "Register By Operator")]
        public bool IsPos { get; set; }
        public decimal AdditiveAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal RemainAmount { get; set; }
        public decimal DecreaseAmount { get; set; }


        //[Display(Name="نحوه پرداخت")]
        //public string PaymentTypeTitleTranslate {
        //    get
        //    {
        //        if (PaymentTypeTitle == "online")
        //            return "پرداخت آنلاین";

        //        if (PaymentTypeTitle == "recieve")
        //            return "پرداخت در محل";

        //        if (PaymentTypeTitle == "transfer")
        //            return "کارت به کارت";

        //        return "پرداخت آنلاین";
        //    }
        //}




        [Display(Name= "Customer Description")]
        [DataType(DataType.MultilineText)]
        public string CustomerDesc { get; set; }

        [Display(Name="Payment Description By Admin")]
        [DataType(DataType.MultilineText)]
        public string PaymentDesc { get; set; }
        public string State { get; set; }
        public DateTime? OrderDate { get; set; }

        [Display(Name = "Customer Type")]
        public Guid? CustomerTypeId { get; set; }
        public virtual CustomerType CustomerType { get; set; }

    }
}
