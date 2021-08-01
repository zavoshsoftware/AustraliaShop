using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Models
{
    public class Product:BaseEntity
    {
        public Product()
        {
            OrderDetails=new List<OrderDetail>();
            ProductComments = new List<ProductComment>();
            ProductImages = new List<ProductImage>();
            ProductSizes = new List<ProductSize>();
            Products = new List<Product>();
        }

        public virtual ICollection<ProductComment> ProductComments { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductSize> ProductSizes { get; set; }
        public virtual ICollection<Product> Products { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public int Order { get; set; }

        public int Code { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(256, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string PageTitle { get; set; }

        [StringLength(1000, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        [DataType(DataType.MultilineText)]
        public string PageDescription { get; set; }

        [Display(Name = "Image")]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string ImageUrl { get; set; }
           
        [DataType(DataType.MultilineText)]
        public string Summery { get; set; }

        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string Body { get; set; }

        [Required()]
        public decimal Amount { get; set; }
        [Display(Name = "Toman Amount")]
        public decimal? IrAmount { get; set; }

        public decimal? DiscountAmount { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [Display(Name = "InPromotion")]
        public bool IsInPromotion { get; set; }

        [Display(Name = "TopRate")]
        public bool IsTopRate { get; set; }

        [Display(Name = "BestSale")]
        public bool IsBestSale { get; set; }
        [Display(Name = "SpecialOffer")]
        public bool IsSpecialOffer { get; set; }
        [Display(Name = "NewArrival")]
        public bool IsNewArrival{ get; set; }
        [Display(Name = "DealOfDay")]
        public bool IsDealOfDay { get; set; }

        public int Stock { get; set; }

        public int SeedStock { get; set; }

        public int Visit { get; set; }

        public int SellNumber { get; set; }
    
        public bool IsAvailable { get; set; }
     
        public Guid  ProductGroupId { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }

        public Guid? ColorId { get; set; }
        public virtual Color Color { get; set; }

        public DateTime? DealOfDayExpireDate { get; set; }

        public int AvailableForDealOfDay { get; set; }
        public int SoldInDealOfDay { get; set; }

        public Guid? ParentId { get; set; }
        public virtual Product Parent { get; set; }
        public Guid? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        #region NotMapped

        [NotMapped]
        [Display(Name = "Amount")]
        public string AmountStr
        {
            get { return Amount.ToString("n0"); }
        }

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

        #endregion
        internal class configuration : EntityTypeConfiguration<Product>
        {
            public configuration()
            {
                HasRequired(p => p.ProductGroup).WithMany(t => t.Products).HasForeignKey(p => p.ProductGroupId);
                HasOptional(p => p.Color).WithMany(t => t.Products).HasForeignKey(p => p.ColorId);
                HasOptional(p => p.Parent).WithMany(t => t.Products).HasForeignKey(p => p.ParentId);
                HasOptional(p => p.Supplier).WithMany(t => t.Products).HasForeignKey(p => p.SupplierId);
            }
        }
    }
}