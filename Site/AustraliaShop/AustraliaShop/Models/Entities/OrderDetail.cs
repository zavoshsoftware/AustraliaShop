using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class OrderDetail : BaseEntity
    {
        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        [Column(TypeName = "Money")]
        public decimal RowAmount { get; set; }


        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }

        public Guid? ProductSizeId { get; set; }
        public virtual ProductSize ProductSize { get; set; }
        internal class Configuration : EntityTypeConfiguration<OrderDetail>
        {
            public Configuration()
            {
                HasRequired(p => p.Order)
                    .WithMany(j => j.OrderDetails)
                    .HasForeignKey(p => p.OrderId);

                HasRequired(p => p.Product)
                    .WithMany(j => j.OrderDetails)
                    .HasForeignKey(p => p.ProductId);
                HasOptional(p => p.ProductSize)
                    .WithMany(j => j.OrderDetails)
                    .HasForeignKey(p => p.ProductSizeId);
            }
        }



     [NotMapped]
        public string RowAmountStr { get { return RowAmount.ToString("N0"); } }

        [NotMapped]

        public string AmountStr { get { return Amount.ToString("N0"); } }

    }
}
