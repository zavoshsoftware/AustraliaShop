using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class ProductGroup:BaseEntity
    {
        public ProductGroup()
        {
            ProductGroups=new List<ProductGroup>();
            Products = new List<Product>();
        }
        [Required]
        public int Order { get; set; }

        [Required]
        [StringLength(256)]
        [Display(Name="Product Group")]
        public string Title { get; set; }


        public string UrlParam { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string Body { get; set; }
 

        public Guid? ParentId  { get; set; }
        public virtual ProductGroup Parent { get; set; }
        public virtual ICollection<ProductGroup> ProductGroups { get; set; }

        public bool IsInHome { get; set; }

   
        internal class Configuration : EntityTypeConfiguration<ProductGroup>
        {
            public Configuration()
            {
                HasOptional(p => p.Parent).WithMany(j => j.ProductGroups).HasForeignKey(p => p.ParentId);
            }
        }


        public virtual ICollection<Product> Products { get; set; }
    }
}