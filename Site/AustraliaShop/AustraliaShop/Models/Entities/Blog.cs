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
    public class Blog : BaseEntity
    {
        public Blog()
        {
            BlogComments=new List<BlogComment>();
        }

        [Required]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string Summery { get; set; }

        [Display(Name= "Image")]
        public string ImageUrl { get; set; }

        [Display(Name= "Url Parameter")]
        public string UrlParam { get; set; }

 
        [Display(Name= "Visit")]
        public int Visit { get; set; }

        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        [Required]
        public string Body { get; set; }

        [Display(Name= "Blog Group")]
        [Required]
        public Guid BlogGroupId { get; set; }
        public virtual BlogGroup BlogGroup { get; set; }

        public virtual ICollection<BlogComment> BlogComments { get; set; }
        internal class Configuration : EntityTypeConfiguration<Blog>
        {
            public Configuration()
            {
                HasRequired(p => p.BlogGroup)
                    .WithMany(j => j.Blogs)
                    .HasForeignKey(p => p.BlogGroupId);
            }
        }
         
    }
}