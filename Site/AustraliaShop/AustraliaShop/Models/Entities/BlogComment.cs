using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Models
{
    public class BlogComment:BaseEntity
    {
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }

        [DataType(DataType.MultilineText)]
        public string Response { get; set; }

        public string Website { get; set; }

        public Guid BlogId { get; set; }
        public virtual Blog Blog { get; set; }

        internal class Configuration : EntityTypeConfiguration<BlogComment>
        {
            public Configuration()
            {
                HasRequired(p => p.Blog)
                    .WithMany(j => j.BlogComments)
                    .HasForeignKey(p => p.BlogId);
            }
        }
    }
}