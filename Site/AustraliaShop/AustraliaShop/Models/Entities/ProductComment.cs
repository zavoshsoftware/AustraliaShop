using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Models
{
    public class ProductComment : BaseEntity
    {
        public string Name { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

        [DataType(DataType.MultilineText)]
        public string Response { get; set; }
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int? Rate { get; set; }
        internal class Configuration : EntityTypeConfiguration<ProductComment>
        {
            public Configuration()
            {
                HasRequired(p => p.Product)
                    .WithMany(j => j.ProductComments)
                    .HasForeignKey(p => p.ProductId);
            }
        }
    }
}